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
            Assert.AreEqual<string>(string.Format(@"/resultsfile:""{0}"" /test:TestConsoleExtended /unique", newTestResultsPath), additionalArguments);
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
            Assert.AreEqual<string>(string.Format(@"/resultsfile:""{0}"" /test:TestConsoleExtended /test:TestConsoleExtended_Second /unique", newTestResultsPath), additionalArguments);
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

        [TestMethod]
        public void UpdateTestRun_ShouldUpdateRunCorrectly()
        {
            string testId = "testId";
            string passedOutcome = "Passed";
            string failedOutcome = "Failed";
            var updatedStartTime = new DateTime(2015, 3, 24, 14, 00, 00);
            var updatedEndTime = new DateTime(2015, 3, 24, 14, 01, 00);

            var sourceResult = new TestRunUnitTestResult();
            sourceResult.TestId = testId;
            sourceResult.ExecutionId = "source123";
            sourceResult.RelativeResultsDirectory = "sourceResultRelativeDir";
            sourceResult.Outcome = passedOutcome;

            var sourceResultInnerResult = new TestRunUnitTestResultInnerUnitTestResult();
            sourceResultInnerResult.Outcome = passedOutcome;
            sourceResultInnerResult.StartTime = updatedStartTime;
            sourceResultInnerResult.EndTime = updatedEndTime;

            sourceResult.InnerResults = new TestRunUnitTestResultInnerUnitTestResult[] { sourceResultInnerResult };
            var sourceRun = new TestRun();
            sourceRun.Results = new TestRunUnitTestResult[] { sourceResult };


            string targetResultRelativeDir = "targetResultRelativeDir";
            string targetResultExecutionId = "target123";
            var targetResult = new TestRunUnitTestResult();
            targetResult.TestId = testId;
            targetResult.ExecutionId = targetResultExecutionId;
            targetResult.RelativeResultsDirectory = targetResultRelativeDir;
            targetResult.Outcome = failedOutcome;

            var targetResultInnerResult = new TestRunUnitTestResultInnerUnitTestResult();
            targetResultInnerResult.Outcome = failedOutcome;
            targetResultInnerResult.StartTime = new DateTime(2015, 3, 24, 13, 58, 25);
            targetResultInnerResult.EndTime = new DateTime(2015, 3, 24, 13, 59, 01);

            targetResult.InnerResults = new TestRunUnitTestResultInnerUnitTestResult[] { targetResultInnerResult };
            var targetRun = new TestRun();
            targetRun.Results = new TestRunUnitTestResult[] { targetResult };
            targetRun.ResultSummary = new TestRunResultSummary();
            targetRun.ResultSummary.Counters = new TestRunResultSummaryCounters();
            targetRun.ResultSummary.Counters.Failed = 2;
            targetRun.ResultSummary.Counters.Passed = 0;
            targetRun.ResultSummary.Counters.Executed = 2;
            targetRun.ResultSummary.Outcome = failedOutcome;

            var log = Mock.Create<ILog>();
            Mock.Arrange(() => log.Info(Arg.AnyString));

            var fileSystemProvider = Mock.Create<IFileSystemProvider>();
            Mock.Arrange(() => fileSystemProvider.ReplaceTestResultsFiles(Arg.IsAny<TestRun>(), Arg.IsAny<TestRun>())).DoNothing();

            var consoleArgumentsProvider = Mock.Create<IConsoleArgumentsProvider>();
            var msTestRunProvider = new MsTestTestRunProvider(consoleArgumentsProvider, fileSystemProvider, log);
            msTestRunProvider.UpdateTestRun(sourceRun, targetRun);

            Mock.Assert(() => fileSystemProvider.ReplaceTestResultsFiles(sourceRun, targetRun), Occurs.Once());
            Assert.AreEqual(passedOutcome, targetRun.ResultSummary.Outcome);
            Assert.AreEqual(2, targetRun.ResultSummary.Counters.Passed);
            Assert.AreEqual(2, targetRun.ResultSummary.Counters.Executed);
            Assert.AreEqual(0, targetRun.ResultSummary.Counters.Failed);
            Assert.AreEqual(1, targetRun.Results.Length);

            var actualResult = targetRun.Results[0];
            Assert.AreEqual(passedOutcome, actualResult.Outcome);
            Assert.AreEqual(testId, actualResult.TestId);
            Assert.AreEqual(targetResultExecutionId, actualResult.ExecutionId);
            Assert.AreEqual(targetResultRelativeDir, actualResult.RelativeResultsDirectory);
            Assert.AreEqual(1, actualResult.InnerResults.Length);

            var actualInnerResult = actualResult.InnerResults[0];
            Assert.AreEqual(passedOutcome, actualInnerResult.Outcome);
            Assert.AreEqual(updatedStartTime, actualInnerResult.StartTime);
            Assert.AreEqual(updatedEndTime, actualInnerResult.EndTime);
        }
    }
}
