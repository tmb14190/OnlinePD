using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace OnlinePD.Controllers.HandHistory
{
	public interface IGameType
    {
		public string Name { get; }
		public LimitType LimitType { get; set; }
		public char Currency { get; set; }
	}

	public abstract class StandardBlinds
    {
		public LimitType LimitType { get; set; }
		public char Currency { get; set; }
		public double SmallBlind { get; set; }
		public double BigBlind { get; set; }
		public StandardBlinds(string input)
		{

			Regex stakeRegex = new Regex(@"\((.*?)\)"); // Regex to get string between '(' and ')'
			string stake = stakeRegex.Match(input).Groups[1].Value;

			this.LimitType = LimitTypeFactory.Build(input);
			this.Currency = stake[0];
			this.SmallBlind = double.Parse(stake.Split("/")[0][1..]);
			this.BigBlind = double.Parse(stake.Split("/")[1][1..]);
		}
	}

	public class Holdem : StandardBlinds, IGameType
	{
		public string Name => "Holdem";
		public Holdem(string input) : base(input) { }

	}

	public class Omaha : StandardBlinds, IGameType
	{
		public string Name => "Omaha";

		public Omaha(string input): base(input) { }

	}

	public class ShortDeck : IGameType
	{
		public string Name => "ShortDeck";
		public LimitType LimitType { get; set; }
		public char Currency { get; set; }
		public double BigBlind { get; set; }
		public double Ante { get; set; }
		public ShortDeck(string[] input)
		{
			string metaDataLine = input.First(); // data necessary on first line of hand text
			Regex gameTypeRegex = new Regex(@":(.*?)-"); // Regex to get string between '(' and ')'

			this.LimitType = LimitTypeFactory.Build(metaDataLine);

			Regex stakeRegex = new Regex(@"\((.*?)\)"); // Regex to get string between '(' and ')'
			string stake = stakeRegex.Match(metaDataLine).Groups[1].Value;

			this.Currency = stake[0];

			this.BigBlind = double.Parse(stake.Split("/")[1][1..].Split(" ")[0]);

			int index = Array.FindIndex(input, line => line.Contains("ante")); // find index of line with ante information
			if (index == -1) throw new Exception("No Ante in Short Deck Game"); 
			else this.Ante = double.Parse(input[index].Split(";")[1].Split(" ")[3][1..]);
		}
	}

	public enum LimitType
	{
		No,
		Pot,
		Limit
	}

	public class LimitTypeFactory
    {
		public static LimitType Build(string input)
        {
			switch (input)
			{
				case string a when a.Contains("No"): return LimitType.No;
				case string a when a.Contains("Pot"): return LimitType.Pot;
				case string a when a.Contains("Limit"): return LimitType.Limit;
				default: throw new Exception("Unable to parse Limit Type");
			}
		}
    }

	public class GameTypeFactory
	{
		public IGameType Build(string[] hand)
		{
			string metaDataLine = hand.First(); // data necessary on first line of hand text

			switch (metaDataLine)
			{
				case string a when a.Contains("Short Deck"): return new ShortDeck(hand);
				case string a when a.Contains("Hold'em"): return new Holdem(metaDataLine);
				case string a when a.Contains("Omaha"): return new Omaha(metaDataLine);
				default: throw new Exception("Unable to parse GameType");
			}
		}
	}
}
