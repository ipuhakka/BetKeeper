using System;
using System.Dynamic;
using System.Collections.Generic;
using Betkeeper.Models;
using NUnit.Framework;

namespace Test.Betkeeper.Models
{
    public class BetTests
    {
        [Test]
        public void ValidateBetRequest_MissingParameter_ReturnsFalse()
        {
            var withoutOdd = new
            {
                stake = 2.2,
                betWon = 1
            };

            var withoutStake = new
            {
                odd = 2.2,
                betWon = 0
            };

            var withoutBetWon = new
            {
                stake = 2.2,
                odd = 2.2
            };

            Assert.IsFalse(Bet.ValidateBetRequest(withoutBetWon));
            Assert.IsFalse(Bet.ValidateBetRequest(withoutOdd));
            Assert.IsFalse(Bet.ValidateBetRequest(withoutStake));
        }

        [Test]
        public void ValidateBetRequest_NotValidParameterTypes_ReturnsFalse()
        {
            var invalidStake = new
            {
                odd = 2.2,
                betWon = 0,
                stake = "invalid"
            };

            var invalidOdd = new
            {
                odd = "invalid",
                betWon = 0,
                stake = 2.2
            };

            var invalidBetWon = new
            {
                odd = 2.2,
                betWon = false,
                stake = 2.2
            };

            Assert.IsFalse(Bet.ValidateBetRequest(invalidBetWon));
            Assert.IsFalse(Bet.ValidateBetRequest(invalidOdd));
            Assert.IsFalse(Bet.ValidateBetRequest(invalidStake));
        }

        [Test]
        public void ValidateBetRequest_betWonNotInRange_ReturnsFalse()
        {
            dynamic betWonUnderRange = new ExpandoObject();

            betWonUnderRange.odd = 2.2;
            betWonUnderRange.stake = 2.2;
            betWonUnderRange.betWon = -2;

            dynamic betWonOverRange = new ExpandoObject();

            betWonOverRange.odd = 2.2;
            betWonOverRange.stake = 2.2;
            betWonOverRange.betWon = 2;

            var testCases = new[]
            {
                betWonUnderRange,
                betWonOverRange
            };

            foreach(var testCase in testCases)
            {
                Assert.IsFalse(Bet.ValidateBetRequest(testCase));
            }

            throw new NotImplementedException();
        }
    }
}
