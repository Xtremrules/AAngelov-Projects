using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSTest.Console.Extended.Infrastructure;

namespace MSTest.Console.Extended.UnitTests
{
    [TestClass]
    public class ConsoleArgumentsProviderTests
    {
        [TestMethod]
        public void Constructor_ShouldSetCorrectRetriesCount_WhenRetriesCountArgumentExists()
        {
            string[] args = 
            {
                @"/resultsfile:C:\Results.trx",
                @"/testcontainer:C:\Frontend\Tests.dll",
                "/nologo",
                "/category:MSTestConsoleExtendedTEST",
                "/retriesCount:3",
                "/deleteOldResultsFiles:true",
                @"/newResultsfile:C:\ResultsNew.trx"
            };
            var consoleArgumentsProvider = new ConsoleArgumentsProvider(args);
            Assert.AreEqual<int>(3, consoleArgumentsProvider.RetriesCount);
        }

        [TestMethod]
        public void Constructor_ShouldSetRetriesCountToZero_WhenRetriesCountArgumentDoesNotExist()
        {
            string[] args = 
            {
                @"/resultsfile:C:\Results.trx",
                @"/testcontainer:C:\Frontend\Tests.dll",
                "/nologo",
                "/category:MSTestConsoleExtendedTEST",
                "/deleteOldResultsFiles:true",
                @"/newResultsfile:C:\ResultsNew.trx"
            };
            var consoleArgumentsProvider = new ConsoleArgumentsProvider(args);
            Assert.AreEqual<int>(0, consoleArgumentsProvider.RetriesCount);
        }

        [TestMethod]
        public void Constructor_ShouldSetCorrectResultsFilePath()
        {
            string[] args = 
            {
                @"/resultsfile:C:\Results.trx",
                @"/testcontainer:C:\Frontend\Tests.dll",
                "/nologo",
                "/category:MSTestConsoleExtendedTEST",
                "/retriesCount:3",
                "/deleteOldResultsFiles:true",
                @"/newResultsfile:C:\ResultsNew.trx"
            };
            var consoleArgumentsProvider = new ConsoleArgumentsProvider(args);
            Assert.AreEqual<string>(@"C:\Results.trx", consoleArgumentsProvider.ResultsFilePath);
        }

        [TestMethod]
        public void Constructor_ShouldSetCorrectResultsFilePath_WhenResultsFilePathContainsUnderscore()
        {
            string[] args = 
            {
                @"/resultsfile:C:\Results_FF.trx",
                @"/testcontainer:C:\Frontend\Tests.dll",
                "/nologo",
                "/category:MSTestConsoleExtendedTEST",
                "/retriesCount:3",
                "/deleteOldResultsFiles:true",
                @"/newResultsfile:C:\ResultsNew.trx"
            };
            var consoleArgumentsProvider = new ConsoleArgumentsProvider(args);
            Assert.AreEqual<string>(@"C:\Results_FF.trx", consoleArgumentsProvider.ResultsFilePath);
        }

        [TestMethod]
        public void Constructor_ShouldSetCorrectResultsFilePath_WhenResultsFilePathContainsDigit()
        {
            string[] args = 
            {
                @"/resultsfile:C:\Results1.trx",
                @"/testcontainer:C:\Frontend\Tests.dll",
                "/nologo",
                "/category:MSTestConsoleExtendedTEST",
                "/retriesCount:3",
                "/deleteOldResultsFiles:true",
                @"/newResultsfile:C:\ResultsNew.trx"
            };
            var consoleArgumentsProvider = new ConsoleArgumentsProvider(args);
            Assert.AreEqual<string>(@"C:\Results1.trx", consoleArgumentsProvider.ResultsFilePath);
        }

        [TestMethod]
        [ExpectedException(exceptionType: typeof(ArgumentException), noExceptionMessage: "You need to specify path to test results.")]
        public void Constructor_ShouldThrowArgumentException_WhenResultsFileArgumentIsMissing()
        {
            string[] args = 
            {
                @"/testcontainer:C:\Frontend\Tests.dll",
                "/nologo",
                "/category:MSTestConsoleExtendedTEST",
                "/retriesCount:3",
                "/deleteOldResultsFiles:true",
                @"/newResultsfile:C:\ResultsNew.trx"
            };
            var consoleArgumentsProvider = new ConsoleArgumentsProvider(args);
            Assert.AreEqual<string>(@"C:\Results1.trx", consoleArgumentsProvider.ResultsFilePath);
        }

        [TestMethod]
        public void Constructor_ShouldSetNewResultsFilePathEqualToResultsFilePath_WhenNewResultsFileArgumentIsMissing()
        {
            string[] args = 
            {
                @"/resultsfile:C:\Results.trx",
                @"/testcontainer:C:\Frontend\Tests.dll",
                "/nologo",
                "/category:MSTestConsoleExtendedTEST",
                "/retriesCount:3",
                "/deleteOldResultsFiles:true"
            };
            var consoleArgumentsProvider = new ConsoleArgumentsProvider(args);
            Assert.AreEqual<string>(@"C:\Results.trx", consoleArgumentsProvider.ResultsFilePath);
        }

        [TestMethod]
        public void Constructor_ShouldSetCorrectNewResultsFilePath_WhenNewResultsFileArgumentExists()
        {
            string[] args = 
            {
                @"/resultsfile:C:\Results.trx",
                @"/testcontainer:C:\Frontend\Tests.dll",
                "/nologo",
                "/category:MSTestConsoleExtendedTEST",
                "/retriesCount:3",
                "/deleteOldResultsFiles:true",
                @"/newResultsfile:C:\ResultsNew.trx"
            };
            var consoleArgumentsProvider = new ConsoleArgumentsProvider(args);
            Assert.AreEqual<string>(@"C:\ResultsNew.trx", consoleArgumentsProvider.NewResultsFilePath);
        }

        [TestMethod]
        public void Constructor_ShouldSetCorrectNewResultsFilePath_WhenNewResultsFilePathContainsUnderscore()
        {
            string[] args = 
            {
                @"/resultsfile:C:\Results_FF.trx",
                @"/testcontainer:C:\Frontend\Tests.dll",
                "/nologo",
                "/category:MSTestConsoleExtendedTEST",
                "/retriesCount:3",
                "/deleteOldResultsFiles:true",
                @"/newResultsfile:C:\Results_New.trx"
            };
            var consoleArgumentsProvider = new ConsoleArgumentsProvider(args);
            Assert.AreEqual<string>(@"C:\Results_New.trx", consoleArgumentsProvider.NewResultsFilePath);
        }

        [TestMethod]
        public void Constructor_ShouldSetCorrectNewResultsFilePath_WhenNewResultsFilePathContainsDigit()
        {
            string[] args = 
            {
                @"/resultsfile:C:\Results1.trx",
                @"/testcontainer:C:\Frontend\Tests.dll",
                "/nologo",
                "/category:MSTestConsoleExtendedTEST",
                "/retriesCount:3",
                "/deleteOldResultsFiles:true",
                @"/newResultsfile:C:\ResultsNew1.trx"
            };
            var consoleArgumentsProvider = new ConsoleArgumentsProvider(args);
            Assert.AreEqual<string>(@"C:\ResultsNew1.trx", consoleArgumentsProvider.NewResultsFilePath);
        }

        [TestMethod]
        public void Constructor_ShouldSetDeleteOldResultsFilesToTrue_WhenDeleteOldResultsFilesArgumentIsTrue()
        {
            string[] args = 
            {
                @"/resultsfile:C:\Results1.trx",
                @"/testcontainer:C:\Frontend\Tests.dll",
                "/nologo",
                "/category:MSTestConsoleExtendedTEST",
                "/retriesCount:3",
                "/deleteOldResultsFiles:true",
                @"/newResultsfile:C:\ResultsNew1.trx"
            };
            var consoleArgumentsProvider = new ConsoleArgumentsProvider(args);
            Assert.AreEqual<bool>(true, consoleArgumentsProvider.ShouldDeleteOldResultsFiles);
        }

        [TestMethod]
        public void Constructor_ShouldSetDeleteOldResultsFilesToFalse_WhenDeleteOldResultsFilesArgumentIsFalse()
        {
            string[] args = 
            {
                @"/resultsfile:C:\Results1.trx",
                @"/testcontainer:C:\Frontend\Tests.dll",
                "/nologo",
                "/category:MSTestConsoleExtendedTEST",
                "/retriesCount:3",
                @"/newResultsfile:C:\ResultsNew1.trx"
            };
            var consoleArgumentsProvider = new ConsoleArgumentsProvider(args);
            Assert.AreEqual<bool>(false, consoleArgumentsProvider.ShouldDeleteOldResultsFiles);
        }

        [TestMethod]
        public void Constructor_ShouldExcludeDeleteOldResultsFilesArgumentFromStandardArguments_WhenDeleteOldResultsFilesArgumentExists()
        {
            string[] args = 
            {
                @"/resultsfile:C:\Results1.trx",
                @"/testcontainer:C:\Frontend\Tests.dll",
                "/nologo",
                "/category:MSTestConsoleExtendedTEST",
                "/retriesCount:3",
                "/deleteOldResultsFiles:true",
                @"/newResultsfile:C:\ResultsNew1.trx"
            };
            var consoleArgumentsProvider = new ConsoleArgumentsProvider(args);
            Assert.IsFalse(consoleArgumentsProvider.StandardArguments.Contains("/deleteOldResultsFiles:true"));
        }

        [TestMethod]
        public void Constructor_ShouldExcludeRetriesCountArgumentFromStandardArguments_WhenRetriesCountArgumentExists()
        {
            string[] args = 
            {
                @"/resultsfile:C:\Results1.trx",
                @"/testcontainer:C:\Frontend\Tests.dll",
                "/nologo",
                "/category:MSTestConsoleExtendedTEST",
                "/retriesCount:3",
                "/deleteOldResultsFiles:true",
                @"/newResultsfile:C:\ResultsNew1.trx"
            };
            var consoleArgumentsProvider = new ConsoleArgumentsProvider(args);
            Assert.IsFalse(consoleArgumentsProvider.StandardArguments.Contains("/retriesCount:3"));
        }

        [TestMethod]
        public void Constructor_ShouldExcludeNewResultsFileArgumentFromStandardArgyments_WhenNewResultsFileArgumentExists()
        {
            string[] args = 
            {
                @"/resultsfile:C:\Results1.trx",
                @"/testcontainer:C:\Frontend\Tests.dll",
                "/nologo",
                "/category:MSTestConsoleExtendedTEST",
                "/retriesCount:3",
                "/deleteOldResultsFiles:true",
                @"/newResultsfile:C:\ResultsNew1.trx"
            };
            var consoleArgumentsProvider = new ConsoleArgumentsProvider(args);
            Assert.IsFalse(consoleArgumentsProvider.StandardArguments.Contains(@"/newResultsfile:C:\ResultsNew1.trx"));
        }

        [TestMethod]
        public void Constructor_ShouldWrapArgumentValuesInQuotes_WhenArgumentValuesContainSpace()
        {
            string[] args = 
            {
                @"/resultsfile:C:\Results1.trx",
                @"/testcontainer:C:\Frontend\Tests.dll",
                "/nologo",
                "/category:MSTestConsoleExtendedTEST",
                "/retriesCount:3",
                "/deleteOldResultsFiles:true",
                @"/newResultsfile:C:\Results New1.trx"
            };
            var consoleArgumentsProvider = new ConsoleArgumentsProvider(args);
            Assert.IsTrue(consoleArgumentsProvider.StandardArguments.Contains(@"/newResultsfile:""C:\Results New1.trx"""));
        }

        [TestMethod]
        public void Constructor_ShouldSetCorrectThreshold_WhenThresholdArgumentExistsAndIsOneDigitNumber()
        {
            string[] args = 
            {
                @"/resultsfile:C:\Results.trx",
                @"/testcontainer:C:\Frontend\Tests.dll",
                "/nologo",
                "/category:MSTestConsoleExtendedTEST",
                "/retriesCount:3",
                "/deleteOldResultsFiles:true",
                @"/newResultsfile:C:\ResultsNew.trx",
                "/threshold:5"
            };
            var consoleArgumentsProvider = new ConsoleArgumentsProvider(args);
            Assert.AreEqual<int>(5, consoleArgumentsProvider.FailedTestsThreshold);
        }

        [TestMethod]
        public void Constructor_ShouldSetCorrectThreshold_WhenThresholdArgumentExistsAndIsTwoDigitsNumber()
        {
            string[] args = 
            {
                @"/resultsfile:C:\Results.trx",
                @"/testcontainer:C:\Frontend\Tests.dll",
                "/nologo",
                "/category:MSTestConsoleExtendedTEST",
                "/retriesCount:3",
                "/deleteOldResultsFiles:true",
                @"/newResultsfile:C:\ResultsNew.trx",
                "/threshold:5"
            };
            var consoleArgumentsProvider = new ConsoleArgumentsProvider(args);
            Assert.AreEqual<int>(5, consoleArgumentsProvider.FailedTestsThreshold);
        }

        [TestMethod]
        public void Constrcutor_ShouldSetThresholdTo10_WhenThresholdArgumentDoesNotExist()
        {
            string[] args = 
            {
                @"/resultsfile:C:\Results.trx",
                @"/testcontainer:C:\Frontend\Tests.dll",
                "/nologo",
                "/category:MSTestConsoleExtendedTEST",
                "/deleteOldResultsFiles:true",
                @"/newResultsfile:C:\ResultsNew.trx"
            };
            var consoleArgumentsProvider = new ConsoleArgumentsProvider(args);
            Assert.AreEqual<int>(10, consoleArgumentsProvider.FailedTestsThreshold);
        }

        [TestMethod]
        public void Constrcutor_ShouldNotStandardArgumentsInCaseInsensitiveWay_WhenNotStandardArgumentsHaveDifferentCasing()
        {
            string[] args = 
            {
                @"/resultsfile:C:\Results.trx",
                @"/testcontainer:C:\Frontend\Tests.dll",
                "/nologo",
                "/category:MSTestConsoleExtendedTEST",
                "/deleteoldRESULTsfiles:true",
                @"/NEWresultsfILe:C:\ResultsNew.trx",
                "/threshold:5",
                "/REtrieScount:2"
            };

            var consoleArgumentsProvider = new ConsoleArgumentsProvider(args);
            Assert.AreEqual<bool>(true, consoleArgumentsProvider.ShouldDeleteOldResultsFiles);
            Assert.AreEqual<string>(@"C:\ResultsNew.trx", consoleArgumentsProvider.NewResultsFilePath);
            Assert.AreEqual<int>(5, consoleArgumentsProvider.FailedTestsThreshold);
            Assert.AreEqual<int>(2, consoleArgumentsProvider.RetriesCount);
        }
    }
}
