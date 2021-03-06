﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestExampleC;
using System.Collections.Generic;
// ReSharper disable UnusedVariable

namespace UnitTestProject {
    [TestClass]
    public class CurrencyExchangeTest {
        private TestContext _testContextInstance;

        public TestContext TestContext {
            get { return _testContextInstance; }
            set { _testContextInstance = value; }
        }

        #region Constructor tests
        [TestMethod]
        public void TestCurrencyExchange_Constructor_CurrenciesEmpty_Exception() {
            //arrange
            List<string> currencies = new List<string>();

            //act, assert
            Assert.ThrowsException<ArgumentException>(() => { CurrencyExchange exchange = new CurrencyExchange(currencies); });
        }

        [TestMethod]
        public void TestCurrencyExchange_Constructor_CurrenciesNull_Exception() {
            //arrange
            List<string> currencies = null;

            //act, assert
            Assert.ThrowsException<ArgumentException>(() => { CurrencyExchange exchange = new CurrencyExchange(currencies); });
        }

        [TestMethod]
        public void TestCurrencyExchange_Constructor_CurrenciesInvalidLength_Exception() {
            //arrange
            List<string> currencies = new List<string>();
            currencies.Add("AAA");
            currencies.Add("BBB");
            currencies.Add("CCCC");
            currencies.Add("DDD");

            //act, assert
            Assert.ThrowsException<ArgumentException>(() => { CurrencyExchange exchange = new CurrencyExchange(currencies); });
        }
        #endregion

        #region Property tests
        [TestMethod]
        public void TestCurrencyExchange_Currencies_GetOnce() {
            //arrange
            CurrencyExchange exchange = new CurrencyExchange(new List<string> { "AAA", "BBB", "CCC" });
            List<string> expectedResult = new List<string> { "AAA", "BBB", "CCC" };

            //act
            List<string> actualResult = exchange.Currencies;

            //assert
            CollectionAssert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void TestCurrencyExchange_CurrencyCrosses_GetOnce() {
            //arrange
            CurrencyExchange exchange = new CurrencyExchange(new List<string> { "AAA", "BBB", "CCC" });
            List<string> expectedResult = new List<string> { "AAABBB", "AAACCC", "BBBCCC" };

            //act
            List<string> actualResult = exchange.CurrencyCrosses;

            //assert
            CollectionAssert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void TestCurrencyExchange_ExchangeRates_GetOnce() {
            //arrange
            CurrencyExchange exchange = new CurrencyExchange(new List<string> { "AAA", "BBB", "CCC" });
            Dictionary<string, double> expectedResult = new Dictionary<string, double>();
            expectedResult.Add("AAABBB", 6.50);
            expectedResult.Add("AAACCC", 0.50);

            //act
            exchange.SpecifyExchangeRate("AAABBB", 6.50);
            exchange.SpecifyExchangeRate("AAACCC", 0.50);
            Dictionary<string, double> actualResult = exchange.ExchangeRates;

            //assert
            CollectionAssert.AreEqual(expectedResult, actualResult);
        }
        #endregion

        #region SpecifyExchangeRate method tests
        [TestMethod]
        public void TestCurrencyExchange_ExchangeRateCurrencyCrossEmpty_Exception() {
            //arrange
            CurrencyExchange exchange = new CurrencyExchange(new List<string> { "AAA", "BBB", "CCC" });
            
            //act, assert
            Assert.ThrowsException<ArgumentException>(() => exchange.SpecifyExchangeRate("", 6.50));
        }

        [TestMethod]
        public void TestCurrencyExchange_ExchangeRateCurrencyCrossDoesNotExist_Exception() {
            //arrange
            CurrencyExchange exchange = new CurrencyExchange(new List<string> { "AAA", "BBB", "CCC" });

            //act, assert
            Assert.ThrowsException<ArgumentException>(() => exchange.SpecifyExchangeRate("AAADDD", 3.45));
        }

        [TestMethod]
        public void TestCurrencyExchange_ExchangeRateNegativeRate_Exception() {
            //arrange
            CurrencyExchange exchange = new CurrencyExchange(new List<string> { "AAA", "BBB", "CCC" });

            //act, assert
            Assert.ThrowsException<ArgumentException>(() => exchange.SpecifyExchangeRate("AAABBB", -5.50));
        }

        [TestMethod]
        public void TestCurrencyExchange_ExchangeRateChange() {
            //arrange
            CurrencyExchange exchange = new CurrencyExchange(new List<string> { "AAA", "BBB", "CCC" });
            double expectedResult = 2.50;

            //act
            exchange.SpecifyExchangeRate("AAABBB", 6.50);
            exchange.SpecifyExchangeRate("AAABBB", 2.50);
            double actualResult = exchange.ExchangeRates["AAABBB"];

            //assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void TestCurrencyExchange_ExchangeRateMixAutomatic_NoException() {
            //arrange
            CurrencyExchange exchange = new CurrencyExchange(new List<string> { "AAA", "BBB", "CCC" });
            double expectedResult = 2.50 / 6.50;

            //act
            exchange.SpecifyExchangeRate("AAABBB", 2.50);
            exchange.SpecifyExchangeRate("AAACCC", 6.50);
            //double actualResult = exchange.ExchangeRates["BBBCCC"];

            //assert
            string output = "";
            foreach (string s in exchange.ExchangeRates.Keys) {
                TestContext.WriteLine(s);
            }
            
            //Assert.AreEqual(expectedResult, actualResult);
        }
        #endregion

        #region CalculateExchangedRate method tests
        [TestMethod]
        public void TestCurrencyExchange_CalculateExchangedRateCrossEmpty_Exception() {
            //arrange
            CurrencyExchange exchange = new CurrencyExchange(new List<string> { "AAA", "BBB", "CCC" });

            //act
            exchange.SpecifyExchangeRate("AAABBB", 6.50);

            //assert
            Assert.ThrowsException<ArgumentException>(() => exchange.CalculateExchangedRate("", "AAA", 200));
        }

        [TestMethod]
        public void TestCurrencyExchange_CalculateExchangedRateCrossNoMatch_Exception() {
            //arrange
            CurrencyExchange exchange = new CurrencyExchange(new List<string> { "AAA", "BBB", "CCC" });

            //act
            exchange.SpecifyExchangeRate("AAABBB", 6.50);

            //assert
            Assert.ThrowsException<ArgumentException>(() => exchange.CalculateExchangedRate("AAADDD", "AAA", 200));
        }

        [TestMethod]
        public void TestCurrencyExchange_CalculateExchangedRateCurrencyNoMatch_Exception() {
            //arrange
            CurrencyExchange exchange = new CurrencyExchange(new List<string> { "AAA", "BBB", "CCC" });

            //act
            exchange.SpecifyExchangeRate("AAABBB", 6.50);

            //assert
            Assert.ThrowsException<ArgumentException>(() => exchange.CalculateExchangedRate("AAABBB", "DDD", 200));
        }

        [TestMethod]
        public void TestCurrencyExchange_CalculateExchangedRateExchangeRateNoMatch_Exception() {
            //arrange
            CurrencyExchange exchange = new CurrencyExchange(new List<string> { "AAA", "BBB", "CCC" });

            //act
            exchange.SpecifyExchangeRate("AAABBB", 6.50);

            //assert
            Assert.ThrowsException<ArgumentException>(() => exchange.CalculateExchangedRate("AAACCC", "AAA", 200));
        }

        [TestMethod]
        public void TestCurrencyExchange_CalculateExchangedRateAmountNotPositive_Exception() {
            //arrange
            CurrencyExchange exchange = new CurrencyExchange(new List<string> { "AAA", "BBB", "CCC" });

            //act
            exchange.SpecifyExchangeRate("AAABBB", 6.50);

            //assert
            Assert.ThrowsException<ArgumentException>(() => exchange.CalculateExchangedRate("AAABBB", "AAA", -200));
        }

        [TestMethod]
        public void TestCurrencyExchange_CalculateExchangedRate_CheckOne() {
            //arrange
            CurrencyExchange exchange = new CurrencyExchange(new List<string> { "AAA", "BBB", "CCC" });
            Dictionary<string, double> expectedResult = new Dictionary<string, double>();
            expectedResult.Add("AAABBB", 6.50);

            //act
            exchange.SpecifyExchangeRate("AAABBB", 6.50);
            Dictionary<string, double> actualResult = exchange.ExchangeRates;

            //assert
            CollectionAssert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void TestCurrencyExchange_CalculateExchangedRate_CheckMultiple() {
            //arrange
            CurrencyExchange exchange = new CurrencyExchange(new List<string> { "AAA", "BBB", "CCC" });
            Dictionary<string, double> expectedResult = new Dictionary<string, double>();
            expectedResult.Add("AAABBB", 6.50);
            expectedResult.Add("AAACCC", 0.50);

            //act
            exchange.SpecifyExchangeRate("AAABBB", 6.50);
            exchange.SpecifyExchangeRate("AAACCC", 0.50);
            Dictionary<string, double> actualResult = exchange.ExchangeRates;

            //assert
            CollectionAssert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void TestCurrencyExchange_CalculateExchangedRate_CheckAfterChange() {
            //arrange
            CurrencyExchange exchange = new CurrencyExchange(new List<string> { "AAA", "BBB", "CCC" });
            Dictionary<string, double> expectedResult = new Dictionary<string, double>();
            expectedResult.Add("AAABBB", 2.50);

            //act
            exchange.SpecifyExchangeRate("AAABBB", 6.50);
            exchange.SpecifyExchangeRate("AAABBB", 2.50);
            Dictionary<string, double> actualResult = exchange.ExchangeRates;

            //assert
            CollectionAssert.AreEqual(expectedResult, actualResult);
        }
        #endregion
    }
}
