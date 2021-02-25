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
}
