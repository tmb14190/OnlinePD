using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace OnlinePD.Controllers.HandHistory
{
    public class Hand
    {
        public string[] HandText { get; set; }
        public string ID { get; set; }
        public string TableName { get; set; }
        public IGameType GameType { get; set; }
        public string HandDateTime { get; set; }
        public int TableMax { get; set; }
        public double NetReturn { get; set; }

        public static Hand Parse(string[] hand)
        {

            var id = HandParser.ParseID(hand); 
            var tableName = HandParser.ParseTableUserName(hand);
            var gameType = HandParser.ParseGameType(hand);
            var dateTime = HandParser.ParseDateTime(hand);
            var tableMax = HandParser.ParseTableMax(hand);
            var netReturn = HandParser.ParseNetReturn(hand);

            return new HandBuilder()
                .WithHandText(hand)
                .WithID(id)
                .WithTableUserName(tableName)
                .WithGameType(gameType)
                .WithDateTime(dateTime)
                .WithTableMax(tableMax)
                .WithNetReturn(netReturn)
                .Build();
        }
    }

    public class HandBuilder
    {
        private string[] handText;
        private string tableName;
        private string id;
        private IGameType gameType;
        private string dateTime;
        private int tableMax;
        private double netReturn;


        public HandBuilder() { }
        public HandBuilder WithHandText(string[] hand)
        {
            this.handText = hand;
            return this;
        }
        public HandBuilder WithID(string id)
        {
            this.id = id;
            return this;
        }
        public HandBuilder WithTableUserName(string tableName)
        {
            this.tableName = tableName;
            return this;
        }
        public HandBuilder WithGameType(IGameType gameType)
        {
            this.gameType = gameType;
            return this;
        }
        public HandBuilder WithDateTime(string dateTime)
        {
            this.dateTime = dateTime;
            return this;
        }
        public HandBuilder WithTableMax(int tableMax)
        {
            this.tableMax = tableMax;
            return this;
        }
        public HandBuilder WithNetReturn(double netReturn)
        {
            this.netReturn = netReturn;
            return this;
        }
        public Hand Build()
        {
            return new Hand
            {
                HandText = this.handText,
                ID = this.id,
                TableName = this.tableName,
                GameType = this.gameType,
                HandDateTime = this.dateTime,
                TableMax = this.tableMax,
                NetReturn = this.netReturn
            };
        }
    }

    public static class HandParser
    {
        /* holds the unique ID plus site (e.g. "Pokerstars #0000000000001") */
        public static string ParseID(string[] hand)
        {
            string metaDataLine = hand.First(); // data necessary on first line of hand text
            Regex idRegex = new Regex(@"#(.*?):"); // Regex to get string between '#' and ':'
            return metaDataLine.Split(" ")[0] + " #" + idRegex.Match(metaDataLine).Groups[1].Value;
        }

        /* Holds the user's accounts name in the hand, found in line below *** HOLE CARDS *** (e.g. "Dealt to USER [As Ac]" */
        public static string ParseTableUserName(string[] hand)
        {
            int index = Array.FindIndex(hand, line => line.Contains("*** HOLE CARDS ***")) + 1; // data necessary on line after this
            return hand[index].Split(" ")[2];
        }

        /* Holds game being played, the bet limit, currency, and stake information (e.g. "Hold'em No Limit ($1.00/$2.00)") */
        public static IGameType ParseGameType(string[] hand)
        {
            return new GameTypeFactory().Build(hand);
        }

        /* Holds date (yyyy/mm/dd) + time (23:59:59) + timezone in string format (WET) */
        public static string ParseDateTime(string[] hand)
        {
            string metaDataLine = hand.First(); // data necessary on first line of hand text
            Regex dateTimeRegex = new Regex(@"-(.*?)\["); // Regex to get string between '-' and '['
            return dateTimeRegex.Match(metaDataLine).Groups[1].Value.Trim();
        }

        /* Holds table max players as int, does not ake into account if fewer players than the maximum may be in the hand (e.g. "6") */
        public static int ParseTableMax(string[] hand)
        {
            string tableDataLine = hand[1]; // data necessary on second line of hand text
            return Int32.Parse(tableDataLine.Split(" ")[2].Substring(0, 1));
        }

        /* Holds total won or lost during the hand, stored as a double, currency must be gained from the gametype record */
        public static double ParseNetReturn(string[] hand)
        {
            // gets table name to be able to find players actions
            string tableUserName = ParseTableUserName(hand);

            // Times a user invest money into a pot will be either, posting a blind, "USER: bets $X", "USER: calls $X", or "USER: raises $X to $X"
            // Times a user collects money from a pot will be when "Uncalled bet ($X) returned to USER", or "USER collected $X from pot"

            double net = 0;
            double street = 0; // need to go by street due to how we are otherwise naive to when we have raised over chips we have already invested
            Regex uncalledBetRegex = new Regex(@"\$(.*?)\)"); // Regex to get string between '-' and '['
            foreach (string line in hand)
            {
                if (line.Contains(tableUserName))
                {
                    // "USER: posts small blind $X" => split at ":" to get "posts small blind $X", then split at " " to get $X, then remove the "$" by removing the first character
                    if (line.Contains("blind")) street -= double.Parse(line.Split(":")[1].Trim().Split(" ")[3][1..]);
                    // "USER: bets/calls $X" => split at ":" to get "bets/calls $X", then split at " " to get $X, then remove the "$" by removing the first character
                    if (line.Contains("bets") || line.Contains("calls")) street -= double.Parse(line.Split(":")[1].Trim().Split(" ")[1][1..]);
                    // "USER: raises $X to $Y" => split at ":" to get " raises $X to $Y", then split at " " to get $X and $Y, then remove the "$" by removing the first character
                    // $Y is always thew total amount the USER has invested for on each street
                    if (line.Contains("raises")) street = -double.Parse(line.Split(":")[1].Trim().Split(" ")[3][1..]);
                    // "Uncalled bet ($X) returned to USER" => use regex to get match of string between "$" and ")"
                    if (line.Contains("Uncalled")) net += double.Parse(uncalledBetRegex.Match(line).Groups[1].Value);
                    // "USER: collected $X from pot" =>split at ":" to get "collected $X from pot", then split at " " to get $X, then remove the "$" by removing the first character
                    if (line.Contains("collected")) net += double.Parse(line.Split(" ")[2][1..]);
                }
                else if (line.Contains("FLOP") || line.Contains("TURN") || line.Contains("RIVER"))
                {
                    net += street;
                    street = 0;
                }
            }

            return net;
        }
    }
}
