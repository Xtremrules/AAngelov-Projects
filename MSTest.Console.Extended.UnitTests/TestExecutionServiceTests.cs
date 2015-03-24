using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using log4net;
using MSTest.Console.Extended.Interfaces;
using System.IO;
using MSTest.Console.Extended.Data;
using System.Collections.Generic;
using MSTest.Console.Extended.Services;

namespace MSTest.Console.Extended.UnitTests
{
    [TestClass]
    public class TestExecutionServiceTests
    {
        [TestMethod]
        public void ExecuteWithRetry_ShouldExecuteOnlyOneRun_WhenRetriestCountSetToOneAndNoFailedTestsPresent()
        {
            var log = Mock.Create<ILog>();
            Mock.Arrange(() => log.Info(Arg.AnyString));
            var consoleArgumentsProvider = Mock.Create<IConsoleArgumentsProvider>();
            string newFileName = Path.GetTempFileName();
            Mock.Arrange(() => consoleArgumentsProvider.NewResultsFilePath).Returns(newFileName);
            Mock.Arrange(() => consoleArgumentsProvider.ResultsFilePath).Returns("Exceptions.trx");
            Mock.Arrange(() => consoleArgumentsProvider.StandardArguments).Returns("ipconfig");
            Mock.Arrange(() => consoleArgumentsProvider.RetriesCount).Returns(1);
            Mock.Arrange(() => consoleArgumentsProvider.FailedTestsThreshold).Returns(10);
            var processExecutionProvider = Mock.Create<IProcessExecutionProvider>();
            Mock.Arrange(() => processExecutionProvider.Execute(string.Empty)).DoNothing();
            Mock.Arrange(() => processExecutionProvider.WaitForCurrentProcessExit()).DoNothing();
            var fileSystemProvider = Mock.Create<IFileSystemProvider>();
            Mock.Arrange(() => fileSystemProvider.SerializeTestRun(Arg.IsAny<TestRun>())).DoNothing();
            TestRun testRun = new TestRun();
            var testRunUnitTestResult = new TestRunUnitTestResult()
            {
                Outcome = "Passed"
            };
            testRun.Results = new TestRunUnitTestResult[]
            {
                testRunUnitTestResult,
                testRunUnitTestResult
            };
            testRun.ResultSummary = new TestRunResultSummary();
            testRun.ResultSummary.Outcome = "Passed";
            Mock.Arrange(() => fileSystemProvider.DeserializeTestRun(Arg.AnyString)).Returns(testRun);
            var microsoftTestRunProvider = Mock.Create<IMsTestTestRunProvider>();

            Mock.Arrange(() => microsoftTestRunProvider.UpdateTestRun(Arg.IsAny<TestRun>(), Arg.IsAny<TestRun>())).DoNothing();
            Mock.Arrange(() => microsoftTestRunProvider.GetAllNotPassedTests(Arg.IsAny<List<TestRunUnitTestResult>>())).Returns(new List<TestRunUnitTestResult>() { });
            Mock.Arrange(() => microsoftTestRunProvider.GenerateAdditionalArgumentsForFailedTestsRun(Arg.IsAny<List<TestRunUnitTestResult>>(), Arg.AnyString)).DoNothing();

            var engine = new TestExecutionService(
                microsoftTestRunProvider,
                fileSystemProvider,
                processExecutionProvider,
                consoleArgumentsProvider,
                log);
            int result = engine.ExecuteWithRetry();
            Mock.Assert(() => processExecutionProvider.Execute(Arg.AnyString), Occurs.Once());
            Mock.Assert(() => fileSystemProvider.DeserializeTestRun(Arg.AnyString), Occurs.Once());
            Mock.Assert(() => microsoftTestRunProvider.GenerateAdditionalArgumentsForFailedTestsRun(Arg.IsAny<List<TestRunUnitTestResult>>(), Arg.AnyString), Occurs.Never());
            Mock.Arrange(() => microsoftTestRunProvider.UpdateTestRun(Arg.IsAny<TestRun>(), Arg.IsAny<TestRun>())).OccursNever();
            Mock.Arrange(() => fileSystemProvider.SerializeTestRun(Arg.IsAny<TestRun>())).OccursNever();
            Assert.AreEqual<int>(0, result);
        }

        [TestMethod]
        public void ExecuteWithRetry_ShouldExecuteTwoRuns_WhenRetriesCountSetToOneAndFailedTestsPresent()
        {
            var log = Mock.Create<ILog>();
            Mock.Arrange(() => log.Info(Arg.AnyString));
            var consoleArgumentsProvider = Mock.Create<IConsoleArgumentsProvider>();
            string newFileName = Path.GetTempFileName();
            Mock.Arrange(() => consoleArgumentsProvider.NewResultsFilePath).Returns(newFileName);
            Mock.Arrange(() => consoleArgumentsProvider.ResultsFilePath).Returns("Exceptions.trx");
            Mock.Arrange(() => consoleArgumentsProvider.StandardArguments).Returns("ipconfig");
            Mock.Arrange(() => consoleArgumentsProvider.RetriesCount).Returns(1);
            Mock.Arrange(() => consoleArgumentsProvider.FailedTestsThreshold).Returns(10);
            var processExecutionProvider = Mock.Create<IProcessExecutionProvider>();
            Mock.Arrange(() => processExecutionProvider.Execute(string.Empty)).DoNothing();
            Mock.Arrange(() => processExecutionProvider.WaitForCurrentProcessExit()).DoNothing();
            var fileSystemProvider = Mock.Create<IFileSystemProvider>();
            Mock.Arrange(() => fileSystemProvider.SerializeTestRun(Arg.IsAny<TestRun>())).DoNothing();
            TestRun testRun = new TestRun();
            testRun.Results = new TestRunUnitTestResult[]
            {
                new TestRunUnitTestResult(),
                new TestRunUnitTestResult()
            };
            testRun.ResultSummary = new TestRunResultSummary();
            testRun.ResultSummary.Outcome = "Failed";
            Mock.Arrange(() => fileSystemProvider.DeserializeTestRun(Arg.AnyString)).Returns(testRun);
            var microsoftTestRunProvider = Mock.Create<IMsTestTestRunProvider>();

            Mock.Arrange(() => microsoftTestRunProvider.UpdateTestRun(Arg.IsAny<TestRun>(), Arg.IsAny<TestRun>())).DoNothing();
            Mock.Arrange(() => microsoftTestRunProvider.GetAllNotPassedTests(Arg.IsAny<List<TestRunUnitTestResult>>())).Returns(new List<TestRunUnitTestResult>() { new TestRunUnitTestResult(), new TestRunUnitTestResult() });
            Mock.Arrange(() => microsoftTestRunProvider.GenerateAdditionalArgumentsForFailedTestsRun(Arg.IsAny<List<TestRunUnitTestResult>>(), Arg.AnyString)).DoNothing();

            var engine = new TestExecutionService(
                microsoftTestRunProvider,
                fileSystemProvider,
                processExecutionProvider,
                consoleArgumentsProvider,
                log);
            int result = engine.ExecuteWithRetry();

            Mock.Assert(() => processExecutionProvider.Execute(Arg.AnyString), Occurs.Exactly(2));
            Mock.Assert(() => fileSystemProvider.DeserializeTestRun(Arg.AnyString), Occurs.Exactly(2));
            Mock.Assert(() => microsoftTestRunProvider.GenerateAdditionalArgumentsForFailedTestsRun(Arg.IsAny<List<TestRunUnitTestResult>>(), Arg.AnyString), Occurs.Once());
            Mock.Arrange(() => microsoftTestRunProvider.UpdateTestRun(Arg.IsAny<TestRun>(), Arg.IsAny<TestRun>())).OccursOnce();
            Mock.Arrange(() => fileSystemProvider.SerializeTestRun(Arg.IsAny<TestRun>())).OccursNever();
            Assert.AreEqual<int>(1, result);
        }

        [TestMethod]
        public void ExecuteWithRetry_ShouldExecuteOnlyOneRun_WhenRetriestCountSetToTwoAndNoFailedTestsPresent()
        {
            var log = Mock.Create<ILog>();
            Mock.Arrange(() => log.Info(Arg.AnyString));
            var consoleArgumentsProvider = Mock.Create<IConsoleArgumentsProvider>();
            string newFileName = Path.GetTempFileName();
            Mock.Arrange(() => consoleArgumentsProvider.NewResultsFilePath).Returns(newFileName);
            Mock.Arrange(() => consoleArgumentsProvider.ResultsFilePath).Returns("NoExceptions.trx");
            Mock.Arrange(() => consoleArgumentsProvider.StandardArguments).Returns("ipconfig");
            Mock.Arrange(() => consoleArgumentsProvider.RetriesCount).Returns(2);
            Mock.Arrange(() => consoleArgumentsProvider.FailedTestsThreshold).Returns(10);
            var processExecutionProvider = Mock.Create<IProcessExecutionProvider>();
            Mock.Arrange(() => processExecutionProvider.Execute(string.Empty)).DoNothing();
            Mock.Arrange(() => processExecutionProvider.WaitForCurrentProcessExit()).DoNothing();
            var fileSystemProvider = Mock.Create<IFileSystemProvider>();
            Mock.Arrange(() => fileSystemProvider.SerializeTestRun(Arg.IsAny<TestRun>())).DoNothing();
            TestRun testRun = new TestRun();
            var testRunUnitTestResult = new TestRunUnitTestResult()
            {
                Outcome = "Passed"
            };
            testRun.Results = new TestRunUnitTestResult[]
            {
                testRunUnitTestResult,
                testRunUnitTestResult
            };
            testRun.ResultSummary = new TestRunResultSummary();
            testRun.ResultSummary.Outcome = "Passed";
            Mock.Arrange(() => fileSystemProvider.DeserializeTestRun(Arg.AnyString)).Returns(testRun);
            var microsoftTestRunProvider = Mock.Create<IMsTestTestRunProvider>();

            Mock.Arrange(() => microsoftTestRunProvider.UpdateTestRun(Arg.IsAny<TestRun>(), Arg.IsAny<TestRun>())).DoNothing();
            Mock.Arrange(() => microsoftTestRunProvider.GetAllNotPassedTests(Arg.IsAny<List<TestRunUnitTestResult>>())).Returns(new List<TestRunUnitTestResult>() { });
            Mock.Arrange(() => microsoftTestRunProvider.GenerateAdditionalArgumentsForFailedTestsRun(Arg.IsAny<List<TestRunUnitTestResult>>(), Arg.AnyString)).DoNothing();

            var engine = new TestExecutionService(
                microsoftTestRunProvider,
                fileSystemProvider,
                processExecutionProvider,
                consoleArgumentsProvider,
                log);
            int result = engine.ExecuteWithRetry();

            Mock.Assert(() => processExecutionProvider.Execute(Arg.AnyString), Occurs.Once());
            Mock.Assert(() => fileSystemProvider.DeserializeTestRun(Arg.AnyString), Occurs.Once());
            Mock.Assert(() => microsoftTestRunProvider.GenerateAdditionalArgumentsForFailedTestsRun(Arg.IsAny<List<TestRunUnitTestResult>>(), Arg.AnyString), Occurs.Never());
            Mock.Arrange(() => microsoftTestRunProvider.UpdateTestRun(Arg.IsAny<TestRun>(), Arg.IsAny<TestRun>())).OccursNever();
            Mock.Arrange(() => fileSystemProvider.SerializeTestRun(Arg.IsAny<TestRun>())).OccursNever();
            Assert.AreEqual<int>(0, result);
        }

        [TestMethod]
        public void ExecuteWithRetry_ShouldExecuteThreeRuns_WhenRetriestCountSetToTwoAndFailedTestsPresent()
        {
            var log = Mock.Create<ILog>();
            Mock.Arrange(() => log.Info(Arg.AnyString));
            var consoleArgumentsProvider = Mock.Create<IConsoleArgumentsProvider>();
            string newFileName = Path.GetTempFileName();
            Mock.Arrange(() => consoleArgumentsProvider.NewResultsFilePath).Returns(newFileName);
            Mock.Arrange(() => consoleArgumentsProvider.ResultsFilePath).Returns("Exceptions.trx");
            Mock.Arrange(() => consoleArgumentsProvider.StandardArguments).Returns("ipconfig");
            Mock.Arrange(() => consoleArgumentsProvider.RetriesCount).Returns(2);
            Mock.Arrange(() => consoleArgumentsProvider.FailedTestsThreshold).Returns(100);
            var processExecutionProvider = Mock.Create<IProcessExecutionProvider>();
            Mock.Arrange(() => processExecutionProvider.Execute(string.Empty)).DoNothing();
            Mock.Arrange(() => processExecutionProvider.WaitForCurrentProcessExit()).DoNothing();
            var fileSystemProvider = Mock.Create<IFileSystemProvider>();
            Mock.Arrange(() => fileSystemProvider.SerializeTestRun(Arg.IsAny<TestRun>())).DoNothing();
            TestRun testRun = new TestRun();
            testRun.Results = new TestRunUnitTestResult[]
            {
                new TestRunUnitTestResult(),
                new TestRunUnitTestResult()
            };
            testRun.ResultSummary = new TestRunResultSummary();
            testRun.ResultSummary.Outcome = "Failed";
            Mock.Arrange(() => fileSystemProvider.DeserializeTestRun(Arg.AnyString)).Returns(testRun);
            var microsoftTestRunProvider = Mock.Create<IMsTestTestRunProvider>();

            Mock.Arrange(() => microsoftTestRunProvider.UpdateTestRun(Arg.IsAny<TestRun>(), Arg.IsAny<TestRun>())).DoNothing();
            Mock.Arrange(() => microsoftTestRunProvider.GetAllNotPassedTests(Arg.IsAny<List<TestRunUnitTestResult>>())).Returns(new List<TestRunUnitTestResult>() { new TestRunUnitTestResult(), new TestRunUnitTestResult() });
            Mock.Arrange(() => microsoftTestRunProvider.GenerateAdditionalArgumentsForFailedTestsRun(Arg.IsAny<List<TestRunUnitTestResult>>(), Arg.AnyString)).DoNothing();

            var engine = new TestExecutionService(
                microsoftTestRunProvider,
                fileSystemProvider,
                processExecutionProvider,
                consoleArgumentsProvider,
                log);
            int result = engine.ExecuteWithRetry();

            Mock.Assert(() => processExecutionProvider.Execute(Arg.AnyString), Occurs.Exactly(3));
            Mock.Assert(() => fileSystemProvider.DeserializeTestRun(Arg.AnyString), Occurs.Exactly(3));
            Mock.Assert(() => microsoftTestRunProvider.GenerateAdditionalArgumentsForFailedTestsRun(Arg.IsAny<List<TestRunUnitTestResult>>(), Arg.AnyString), Occurs.Exactly(2));
            Mock.Arrange(() => microsoftTestRunProvider.UpdateTestRun(Arg.IsAny<TestRun>(), Arg.IsAny<TestRun>())).Occurs(2);
            Mock.Arrange(() => fileSystemProvider.SerializeTestRun(Arg.IsAny<TestRun>())).OccursNever();
            Assert.AreEqual<int>(1, result);
        }

        [TestMethod]
        public void ExecuteWithRetry_ShouldExecuteOnlyOneRun_WhenThresholdSmallerThanFailedTestsPercentage()
        {
            var log = Mock.Create<ILog>();
            Mock.Arrange(() => log.Info(Arg.AnyString));
            var consoleArgumentsProvider = Mock.Create<IConsoleArgumentsProvider>();
            string newFileName = Path.GetTempFileName();
            Mock.Arrange(() => consoleArgumentsProvider.NewResultsFilePath).Returns(newFileName);
            Mock.Arrange(() => consoleArgumentsProvider.ResultsFilePath).Returns("Exceptions.trx");
            Mock.Arrange(() => consoleArgumentsProvider.StandardArguments).Returns("ipconfig");
            Mock.Arrange(() => consoleArgumentsProvider.RetriesCount).Returns(2);
            Mock.Arrange(() => consoleArgumentsProvider.FailedTestsThreshold).Returns(50);
            var processExecutionProvider = Mock.Create<IProcessExecutionProvider>();
            Mock.Arrange(() => processExecutionProvider.Execute(string.Empty)).DoNothing();
            Mock.Arrange(() => processExecutionProvider.WaitForCurrentProcessExit()).DoNothing();
            var fileSystemProvider = Mock.Create<IFileSystemProvider>();
            Mock.Arrange(() => fileSystemProvider.SerializeTestRun(Arg.IsAny<TestRun>())).DoNothing();
            TestRun testRun = new TestRun();
            testRun.Results = new TestRunUnitTestResult[]
            {
                new TestRunUnitTestResult(),
                new TestRunUnitTestResult()
            };
            testRun.ResultSummary = new TestRunResultSummary();
            testRun.ResultSummary.Outcome = "Failed";
            Mock.Arrange(() => fileSystemProvider.DeserializeTestRun(Arg.AnyString)).Returns(testRun);
            var microsoftTestRunProvider = Mock.Create<IMsTestTestRunProvider>();

            Mock.Arrange(() => microsoftTestRunProvider.UpdateTestRun(Arg.IsAny<TestRun>(), Arg.IsAny<TestRun>())).DoNothing();
            Mock.Arrange(() => microsoftTestRunProvider.GetAllNotPassedTests(Arg.IsAny<List<TestRunUnitTestResult>>())).Returns(new List<TestRunUnitTestResult>() { new TestRunUnitTestResult(), new TestRunUnitTestResult() });
            Mock.Arrange(() => microsoftTestRunProvider.GenerateAdditionalArgumentsForFailedTestsRun(Arg.IsAny<List<TestRunUnitTestResult>>(), Arg.AnyString)).DoNothing();
            Mock.Arrange(() => microsoftTestRunProvider.CalculatedFailedTestsPercentage(Arg.IsAny<List<TestRunUnitTestResult>>(), Arg.IsAny<List<TestRunUnitTestResult>>())).Returns(100);

            var engine = new TestExecutionService(
                microsoftTestRunProvider,
                fileSystemProvider,
                processExecutionProvider,
                consoleArgumentsProvider,
                log);
            int result = engine.ExecuteWithRetry();

            Mock.Assert(() => processExecutionProvider.Execute(Arg.AnyString), Occurs.Once());
            Mock.Assert(() => fileSystemProvider.DeserializeTestRun(Arg.AnyString), Occurs.Once());
            Mock.Assert(() => microsoftTestRunProvider.GenerateAdditionalArgumentsForFailedTestsRun(Arg.IsAny<List<TestRunUnitTestResult>>(), Arg.AnyString), Occurs.Never());
            Mock.Arrange(() => microsoftTestRunProvider.UpdateTestRun(Arg.IsAny<TestRun>(), Arg.IsAny<TestRun>())).OccursNever();
            Mock.Arrange(() => fileSystemProvider.SerializeTestRun(Arg.IsAny<TestRun>())).OccursNever();
            Assert.AreEqual<int>(1, result);
        }

        [TestMethod]
        public void ExecuteWithRetry_ShouldExecuteOnlyOneRun_WhenThresholdEqualToFailedTestsPercentage()
        {
            var log = Mock.Create<ILog>();
            Mock.Arrange(() => log.Info(Arg.AnyString));
            var consoleArgumentsProvider = Mock.Create<IConsoleArgumentsProvider>();
            string newFileName = Path.GetTempFileName();
            Mock.Arrange(() => consoleArgumentsProvider.NewResultsFilePath).Returns(newFileName);
            Mock.Arrange(() => consoleArgumentsProvider.ResultsFilePath).Returns("Exceptions.trx");
            Mock.Arrange(() => consoleArgumentsProvider.StandardArguments).Returns("ipconfig");
            Mock.Arrange(() => consoleArgumentsProvider.RetriesCount).Returns(2);
            Mock.Arrange(() => consoleArgumentsProvider.FailedTestsThreshold).Returns(50);
            var processExecutionProvider = Mock.Create<IProcessExecutionProvider>();
            Mock.Arrange(() => processExecutionProvider.Execute(string.Empty)).DoNothing();
            Mock.Arrange(() => processExecutionProvider.WaitForCurrentProcessExit()).DoNothing();
            var fileSystemProvider = Mock.Create<IFileSystemProvider>();
            Mock.Arrange(() => fileSystemProvider.SerializeTestRun(Arg.IsAny<TestRun>())).DoNothing();
            TestRun testRun = new TestRun();
            testRun.Results = new TestRunUnitTestResult[]
            {
                new TestRunUnitTestResult(),
                new TestRunUnitTestResult()
            };
            testRun.ResultSummary = new TestRunResultSummary();
            testRun.ResultSummary.Outcome = "Failed";
            Mock.Arrange(() => fileSystemProvider.DeserializeTestRun(Arg.AnyString)).Returns(testRun);
            var microsoftTestRunProvider = Mock.Create<IMsTestTestRunProvider>();

            Mock.Arrange(() => microsoftTestRunProvider.UpdateTestRun(Arg.IsAny<TestRun>(), Arg.IsAny<TestRun>())).DoNothing();
            Mock.Arrange(() => microsoftTestRunProvider.GetAllNotPassedTests(Arg.IsAny<List<TestRunUnitTestResult>>())).Returns(new List<TestRunUnitTestResult>() { new TestRunUnitTestResult(), new TestRunUnitTestResult() });
            Mock.Arrange(() => microsoftTestRunProvider.GenerateAdditionalArgumentsForFailedTestsRun(Arg.IsAny<List<TestRunUnitTestResult>>(), Arg.AnyString)).DoNothing();
            Mock.Arrange(() => microsoftTestRunProvider.CalculatedFailedTestsPercentage(Arg.IsAny<List<TestRunUnitTestResult>>(), Arg.IsAny<List<TestRunUnitTestResult>>())).Returns(50);

            var engine = new TestExecutionService(
                microsoftTestRunProvider,
                fileSystemProvider,
                processExecutionProvider,
                consoleArgumentsProvider,
                log);
            int result = engine.ExecuteWithRetry();

            Mock.Assert(() => processExecutionProvider.Execute(Arg.AnyString), Occurs.Once());
            Mock.Assert(() => fileSystemProvider.DeserializeTestRun(Arg.AnyString), Occurs.Once());
            Mock.Assert(() => microsoftTestRunProvider.GenerateAdditionalArgumentsForFailedTestsRun(Arg.IsAny<List<TestRunUnitTestResult>>(), Arg.AnyString), Occurs.Never());
            Mock.Arrange(() => microsoftTestRunProvider.UpdateTestRun(Arg.IsAny<TestRun>(), Arg.IsAny<TestRun>())).OccursNever();
            Mock.Arrange(() => fileSystemProvider.SerializeTestRun(Arg.IsAny<TestRun>())).OccursNever();
            Assert.AreEqual<int>(1, result);
        }

        [TestMethod]
        public void ExecuteWithRetry_ShouldExecuteFourRuns_WhenRetriestCountSetToThreeAndFailedTestsPresent()
        {
            var log = Mock.Create<ILog>();
            Mock.Arrange(() => log.Info(Arg.AnyString));
            var consoleArgumentsProvider = Mock.Create<IConsoleArgumentsProvider>();
            string newFileName = Path.GetTempFileName();
            Mock.Arrange(() => consoleArgumentsProvider.NewResultsFilePath).Returns(newFileName);
            Mock.Arrange(() => consoleArgumentsProvider.ResultsFilePath).Returns("Exceptions.trx");
            Mock.Arrange(() => consoleArgumentsProvider.StandardArguments).Returns("ipconfig");
            Mock.Arrange(() => consoleArgumentsProvider.RetriesCount).Returns(3);
            Mock.Arrange(() => consoleArgumentsProvider.FailedTestsThreshold).Returns(100);
            var processExecutionProvider = Mock.Create<IProcessExecutionProvider>();
            Mock.Arrange(() => processExecutionProvider.Execute(string.Empty)).DoNothing();
            Mock.Arrange(() => processExecutionProvider.WaitForCurrentProcessExit()).DoNothing();
            var fileSystemProvider = Mock.Create<IFileSystemProvider>();
            TestRun testRun = new TestRun();
            testRun.Results = new TestRunUnitTestResult[]
            {
                new TestRunUnitTestResult(),
                new TestRunUnitTestResult()
            };
            testRun.ResultSummary = new TestRunResultSummary();
            testRun.ResultSummary.Outcome = "Passed";
            Mock.Arrange(() => fileSystemProvider.DeserializeTestRun(Arg.AnyString)).Returns(testRun);
            Mock.Arrange(() => fileSystemProvider.SerializeTestRun(Arg.IsAny<TestRun>())).DoNothing();
            var microsoftTestRunProvider = Mock.Create<IMsTestTestRunProvider>();

            Mock.Arrange(() => microsoftTestRunProvider.UpdateTestRun(Arg.IsAny<TestRun>(), Arg.IsAny<TestRun>())).DoNothing();
            Mock.Arrange(() => microsoftTestRunProvider.GetAllNotPassedTests(Arg.IsAny<List<TestRunUnitTestResult>>())).Returns(new List<TestRunUnitTestResult>() { new TestRunUnitTestResult(), new TestRunUnitTestResult() });
            Mock.Arrange(() => microsoftTestRunProvider.GenerateAdditionalArgumentsForFailedTestsRun(Arg.IsAny<List<TestRunUnitTestResult>>(), Arg.AnyString)).DoNothing();

            var engine = new TestExecutionService(
                microsoftTestRunProvider,
                fileSystemProvider,
                processExecutionProvider,
                consoleArgumentsProvider,
                log);
            int result = engine.ExecuteWithRetry();

            Mock.Assert(() => processExecutionProvider.Execute(Arg.AnyString), Occurs.Exactly(4));
            Mock.Assert(() => fileSystemProvider.DeserializeTestRun(Arg.AnyString), Occurs.Exactly(4));
            Mock.Assert(() => microsoftTestRunProvider.GenerateAdditionalArgumentsForFailedTestsRun(Arg.IsAny<List<TestRunUnitTestResult>>(), Arg.AnyString), Occurs.Exactly(3));
            Mock.Arrange(() => microsoftTestRunProvider.UpdateTestRun(Arg.IsAny<TestRun>(), Arg.IsAny<TestRun>())).Occurs(3);
            Mock.Arrange(() => fileSystemProvider.SerializeTestRun(Arg.IsAny<TestRun>())).OccursNever();
            Assert.AreEqual<int>(1, result);
        }

        [TestMethod]
        public void ExecuteWithRetry_ShouldExecuteTwoRuns_WhenRetriestCountSetToThreeAndFailedTestsPresentAndSecondTimeNoFailedTestsPresent()
        {
            var log = Mock.Create<ILog>();
            Mock.Arrange(() => log.Info(Arg.AnyString));
            var consoleArgumentsProvider = Mock.Create<IConsoleArgumentsProvider>();
            string newFileName = Path.GetTempFileName();
            Mock.Arrange(() => consoleArgumentsProvider.NewResultsFilePath).Returns(newFileName);
            Mock.Arrange(() => consoleArgumentsProvider.ResultsFilePath).Returns("Exceptions.trx");
            Mock.Arrange(() => consoleArgumentsProvider.StandardArguments).Returns("ipconfig");
            Mock.Arrange(() => consoleArgumentsProvider.RetriesCount).Returns(3);
            Mock.Arrange(() => consoleArgumentsProvider.FailedTestsThreshold).Returns(100);
            var processExecutionProvider = Mock.Create<IProcessExecutionProvider>();
            Mock.Arrange(() => processExecutionProvider.Execute(string.Empty)).DoNothing();
            Mock.Arrange(() => processExecutionProvider.WaitForCurrentProcessExit()).DoNothing();
            var fileSystemProvider = Mock.Create<IFileSystemProvider>();
            TestRun testRun = new TestRun();
            testRun.Results = new TestRunUnitTestResult[]
            {
                new TestRunUnitTestResult(),
                new TestRunUnitTestResult()
            };
            testRun.ResultSummary = new TestRunResultSummary();
            testRun.ResultSummary.Outcome = "Passed";
            Mock.Arrange(() => fileSystemProvider.DeserializeTestRun(Arg.AnyString)).Returns(testRun);
            Mock.Arrange(() => fileSystemProvider.SerializeTestRun(Arg.IsAny<TestRun>())).DoNothing();
            var microsoftTestRunProvider = Mock.Create<IMsTestTestRunProvider>();

            Mock.Arrange(() => microsoftTestRunProvider.UpdateTestRun(Arg.IsAny<TestRun>(), Arg.IsAny<TestRun>())).DoNothing();
            Mock.Arrange(() => microsoftTestRunProvider.GetAllNotPassedTests(Arg.IsAny<List<TestRunUnitTestResult>>())).Returns(new List<TestRunUnitTestResult>() { new TestRunUnitTestResult(), new TestRunUnitTestResult() }).InSequence();
            Mock.Arrange(() => microsoftTestRunProvider.GetAllNotPassedTests(Arg.IsAny<List<TestRunUnitTestResult>>())).Returns(new List<TestRunUnitTestResult>()).InSequence();
            Mock.Arrange(() => microsoftTestRunProvider.GenerateAdditionalArgumentsForFailedTestsRun(Arg.IsAny<List<TestRunUnitTestResult>>(), Arg.AnyString)).DoNothing();

            var engine = new TestExecutionService(
                microsoftTestRunProvider,
                fileSystemProvider,
                processExecutionProvider,
                consoleArgumentsProvider,
                log);
            int result = engine.ExecuteWithRetry();

            Mock.Assert(() => processExecutionProvider.Execute(Arg.AnyString), Occurs.Exactly(2));
            Mock.Assert(() => fileSystemProvider.DeserializeTestRun(Arg.AnyString), Occurs.Exactly(2));
            Mock.Assert(() => microsoftTestRunProvider.GenerateAdditionalArgumentsForFailedTestsRun(Arg.IsAny<List<TestRunUnitTestResult>>(), Arg.AnyString), Occurs.Once());
            Mock.Arrange(() => microsoftTestRunProvider.UpdateTestRun(Arg.IsAny<TestRun>(), Arg.IsAny<TestRun>())).OccursOnce();
            Mock.Arrange(() => fileSystemProvider.SerializeTestRun(Arg.IsAny<TestRun>())).OccursNever();
            Assert.AreEqual<int>(0, result);
        }
    }
}
