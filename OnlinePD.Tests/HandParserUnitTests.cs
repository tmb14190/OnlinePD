using OnlinePD.Controllers.HandHistory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace OnlinePD.Tests
{
    public class HandParserUnitTests
    {
        private ITestOutputHelper output;

        public void MyTestClass(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void HandParserParseIDTest()
        {
            // arrange
            string[] hand = File.ReadAllText("exampleHH.txt").Split("\n");
            string expectedID = "Pokerstars #197638875182";

            // act
            var result = HandParser.ParseID(hand);

            // assert
            Assert.Equal(result, expectedID);

        }

        [Fact]
        public void HandParserParseTableUserName()
        {
            // arrange
            string[] hand = File.ReadAllText("exampleHH.txt").Split("\n");
            string expectedName = "USER";

            // act
            var result = HandParser.ParseTableUserName(hand);

            // assert
            Assert.Equal(result, expectedName);

        }

        [Fact]
        public void HandParserParseDateTime()
        {
            // arrange
            string[] hand = File.ReadAllText("exampleHH.txt").Split("\n");
            string expectedDate = "2020/05/30 20:08:15 WET";

            // act
            var result = HandParser.ParseDateTime(hand);

            // assert
            Assert.Equal(result, expectedDate);

        }

        [Fact]
        public void HandParserParseTableMax()
        {
            // arrange
            string[] hand = File.ReadAllText("exampleHH.txt").Split("\n");
            int expectedMax = 6;

            // act
            var result = HandParser.ParseTableMax(hand);

            // assert
            Assert.Equal(result, expectedMax);

        }

        [Fact]
        public void HandParserParseNetReturn()
        {
            // arrange
            string[] hand = File.ReadAllText("exampleHH.txt").Split("\n");
            double expectedNet = 25.25;

            // act
            var result = HandParser.ParseNetReturn(hand);

            // assert
            Assert.Equal(result, expectedNet);

        }
    }

    public class HandIntegrationTests
    {
        [Fact]
        public void HandIntegrationTest()
        {
            // arrange
            string rawLines = File.ReadAllText("exampleHHMultiple.txt");
            IList<string> rawHands = rawLines.Split("\r\n\r\n\r\n");
            int expectedNumber = 3;
            string expectedID0 = "Pokerstars #197638875182";
            string expectedID1 = "Pokerstars #197638875183";
            string expectedID2 = "Pokerstars #197638875184";

            // act
            IList<Hand> handsResult = rawHands.Select(hand => Hand.Parse(hand.Split("\n"))).ToList();

            // assert
            Assert.Equal(handsResult.Count, expectedNumber);
            Assert.Equal(handsResult[0].ID, expectedID0);
            Assert.Equal(handsResult[1].ID, expectedID1);
            Assert.Equal(handsResult[2].ID, expectedID2);

        }
    }
}
