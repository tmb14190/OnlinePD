using OnlinePD.Controllers.HandHistory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OnlinePD.Tests
{
    public class GameTypeTests
    {
        [Fact]
        public void GameTypeFactoryIntegrationTest()
        {
            // arrange
            string[] hand = File.ReadAllText("exampleHH.txt").Split("\n");
            string expectedName = "Holdem";
            char expectedCurrency = '$';
            LimitType limitType = LimitType.No;
            double expectedSmallBlind = 1.00;
            double expectedBigBlind = 2.00;

            // act
            var result = new GameTypeFactory().Build(hand);

            // assert
            Assert.Equal(result.Name, expectedName);
            Assert.Equal(result.Currency, expectedCurrency);
            Assert.Equal(result.LimitType, limitType);

            Holdem castedResult;

            castedResult = (Holdem)result;

            Assert.Equal(castedResult.SmallBlind, expectedSmallBlind);
            Assert.Equal(castedResult.BigBlind, expectedBigBlind);
        }
    }
}
