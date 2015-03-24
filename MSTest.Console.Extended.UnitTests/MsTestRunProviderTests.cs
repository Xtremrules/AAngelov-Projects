using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSTest.Console.Extended.Interfaces;
using Telerik.JustMock;
using log4net;
using System.IO;
using MSTest.Console.Extended.Infrastructure;
using MSTest.Console.Extended.Data;

namespace MSTest.Console.Extended.UnitTests
{
    [TestClass]
    public class MsTestRunProviderTests
    {
        [TestMethod]
        public void GenerateAdditionalArgumentsForFailedTestsRun_ShouldAddOneTestArgument_WhenOneFailedTestPresent()
        {
            var log = Mock.Create<ILog>();
            Mock.Arrange(() => log.Info(Arg.AnyString));
            var consoleArgumentsProvider = Mock.Create<IConsoleArgumentsProvider>();
            string newTestResultsPath = Path.GetTempFileName();
            Mock.Arrange(() => consoleArgumentsProvider.StandardArguments).Returns(@"/resultsfile:""C:\Results.trx""");
            Mock.Arrange(() => consoleArgumentsProvider.ResultsFilePath).Returns(@"C:\Results.trx");
            var fileSystemProvider = new FileSystemProvider(consoleArgumentsProvider);
            var testRun = fileSystemProvider.DeserializeTestRun("Resources\\Exceptions.trx");

            var microsoftTestTestRunProvider = new MsTestTestRunProvider(consoleArgumentsProvider, fileSystemProvider, log);
            var failedTests = microsoftTestTestRunProvider.GetAllNotPassedTests(testRun.Results.ToList());
            string additionalArguments = microsoftTestTestRunProvider.GenerateAdditionalArgumentsForFailedTestsRun(failedTests, newTestResultsPath);
            Assert.AreEqual<string>(string.Format(@"/resultsfile:""{0}"" /test:TestConsoleExtended", newTestResultsPath), additionalArguments);
        }

        [TestMethod]
        public void GenerateAdditionalArgumentsForFailedTestsRun_ShouldAddTwoTestArguments_WhenTwoFailedTestsPresent()
        {
            var log = Mock.Create<ILog>();
            Mock.Arrange(() => log.Info(Arg.AnyString));
            var consoleArgumentsProvider = Mock.Create<IConsoleArgumentsProvider>();
            string newTestResultsPath = Path.GetTempFileName();
            Mock.Arrange(() => consoleArgumentsProvider.StandardArguments).Returns(@"/resultsfile:""C:\Results.trx""");
            Mock.Arrange(() => consoleArgumentsProvider.ResultsFilePath).Returns(@"C:\Results.trx");
            var fileSystemProvider = new FileSystemProvider(consoleArgumentsProvider);
            var testRun = fileSystemProvider.DeserializeTestRun("Resources\\Exceptions.trx");
           
            var microsoftTestTestRunProvider = new MsTestTestRunProvider(consoleArgumentsProvider, fileSystemProvider, log);
            string additionalArguments = microsoftTestTestRunProvider.GenerateAdditionalArgumentsForFailedTestsRun(testRun.Results.ToList(), newTestResultsPath);
            Assert.AreEqual<string>(string.Format(@"/resultsfile:""{0}"" /test:TestConsoleExtended /test:TestConsoleExtended_Second", newTestResultsPath), additionalArguments);
        }

        [TestMethod]
        public void GetAllFailedTests_ShouldReturnNoTests_WhenAllTestsArePassed()
        {
            var log = Mock.Create<ILog>();
            Mock.Arrange(() => log.Info(Arg.AnyString));
            var consoleArgumentsProvider = Mock.Create<IConsoleArgumentsProvider>();
            string newFileName = Path.GetTempFileName();
            Mock.Arrange(() => consoleArgumentsProvider.NewResultsFilePath).Returns(newFileName);
            var fileSystemProvider = new FileSystemProvider(consoleArgumentsProvider);
            var testRun = fileSystemProvider.DeserializeTestRun("Resources\\NoExceptions.trx");

            var microsoftTestTestRunProvider = new MsTestTestRunProvider(consoleArgumentsProvider, fileSystemProvider, log);
            var failedTests = microsoftTestTestRunProvider.GetAllNotPassedTests(testRun.Results.ToList());
            Assert.AreEqual<int>(0, failedTests.Count);
        }

        [TestMethod]
        public void GetAllFailedTests_ShouldReturnAllFailedTests_WhenFailedTestspresent()
        {
            var log = Mock.Create<ILog>();
            Mock.Arrange(() => log.Info(Arg.AnyString));
            var consoleArgumentsProvider = Mock.Create<IConsoleArgumentsProvider>();
            string newFileName = Path.GetTempFileName();
            Mock.Arrange(() => consoleArgumentsProvider.NewResultsFilePath).Returns(newFileName);
            var fileSystemProvider = new FileSystemProvider(consoleArgumentsProvider);
            var testRun = fileSystemProvider.DeserializeTestRun("Resources\\Exceptions.trx");

            var microsoftTestTestRunProvider = new MsTestTestRunProvider(consoleArgumentsProvider, fileSystemProvider, log);
            var failedTests = microsoftTestTestRunProvider.GetAllNotPassedTests(testRun.Results.ToList());
            Assert.AreEqual<int>(1, failedTests.Count);
        }

        [TestMethod]
        public void CalculatedFailedTestsPercentage_ShouldReturnZero_WhenNoFailedTestsPresent()
        {
            var log = Mock.Create<ILog>();
            Mock.Arrange(() => log.Info(Arg.AnyString));
            var consoleArgumentsProvider = Mock.Create<IConsoleArgumentsProvider>();
            var fileSystemProvider = new FileSystemProvider(consoleArgumentsProvider);
            var microsoftTestTestRunProvider = new MsTestTestRunProvider(consoleArgumentsProvider, fileSystemProvider, log);

            List<TestRunUnitTestResult> failedTests = new List<TestRunUnitTestResult>();
            List<TestRunUnitTestResult> allTests = new List<TestRunUnitTestResult>()
            {
                new TestRunUnitTestResult()
            };
            var failedTestsPercentage = microsoftTestTestRunProvider.CalculatedFailedTestsPercentage(failedTests, allTests);

            Assert.AreEqual<int>(0, failedTestsPercentage);
        }

        [TestMethod]
        public void CalculatedFailedTestsPercentage_ShouldReturn50_WhenOneFailedTestPresentOfTwo()
        {
            var log = Mock.Create<ILog>();
            Mock.Arrange(() => log.Info(Arg.AnyString));
            var consoleArgumentsProvider = Mock.Create<IConsoleArgumentsProvider>();
            var fileSystemProvider = new FileSystemProvider(consoleArgumentsProvider);
            var microsoftTestTestRunProvider = new MsTestTestRunProvider(consoleArgumentsProvider, fileSystemProvider, log);

            List<TestRunUnitTestResult> failedTests = new List<TestRunUnitTestResult>()
            {
                new TestRunUnitTestResult()
            };

            List<TestRunUnitTestResult> allTests = new List<TestRunUnitTestResult>()
            {
                new TestRunUnitTestResult(),
                new TestRunUnitTestResult()
            };

            var failedTestsPercentage = microsoftTestTestRunProvider.CalculatedFailedTestsPercentage(failedTests, allTests);

            Assert.AreEqual<int>(50, failedTestsPercentage);
        }

        [TestMethod]
        public void CalculatedFailedTestsPercentage_ShouldReturnZero_WhenNoTestsPresent()
        {
            var log = Mock.Create<ILog>();
            Mock.Arrange(() => log.Info(Arg.AnyString));
            var consoleArgumentsProvider = Mock.Create<IConsoleArgumentsProvider>();
            var fileSystemProvider = new FileSystemProvider(consoleArgumentsProvider);
            var microsoftTestTestRunProvider = new MsTestTestRunProvider(consoleArgumentsProvider, fileSystemProvider, log);

            List<TestRunUnitTestResult> failedTests = new List<TestRunUnitTestResult>();
            List<TestRunUnitTestResult> allTests = new List<TestRunUnitTestResult>();
            var failedTestsPercentage = microsoftTestTestRunProvider.CalculatedFailedTestsPercentage(failedTests, allTests);

            Assert.AreEqual<int>(0, failedTestsPercentage);
        }
    }
}
