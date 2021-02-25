using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using OnlinePD.Controllers.HandHistory;
using System.IO;
using Xunit.Sdk;
using System.Reflection;
using System.Linq;

namespace OnlinePD.Tests
{
    public class HandTests
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

    [Fact]
        public void HandIntegrationTestWithDatabaseUse()
        {
            // arrange
            HandHistoryService handHistoryService = new HandHistoryService();
            string user = "USER";
            string filepath = "exampleHHMultiple.txt";
            int expectedNumber = 3;
            string expectedID0 = "Pokerstars #197638875182";
            string expectedID1 = "Pokerstars #197638875183";
            string expectedID2 = "Pokerstars #197638875184";

            // act
            handHistoryService.UploadHandHistoryFileToDatabase(user, filepath);
            IList<Hand> handsResult = handHistoryService.GetHandsByUser(user);

            // assert
            Assert.Equal(handsResult.Count, expectedNumber);
            Assert.Equal(handsResult[0].ID, expectedID0);
            Assert.Equal(handsResult[1].ID, expectedID1);
            Assert.Equal(handsResult[2].ID, expectedID2);

        }

    }
}
