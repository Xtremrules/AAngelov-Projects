using System.IO;
using System.Linq;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSTest.Console.Extended.Infrastructure;
using MSTest.Console.Extended.Interfaces;
using Telerik.JustMock;

namespace MSTest.Console.Extended.UnitTests.MsTestTestRunProviderTests
{
    [TestClass]
    public class MsTestTestRunProvider_UpdateResultsSummary_Should
    {
        [TestMethod]
        public void SetCorrectPassedCounter_WhenNoFailedTestsPresent()
        {
            var log = Mock.Create<ILog>();
            Mock.Arrange(() => log.Info(Arg.AnyString));
            var consoleArgumentsProvider = Mock.Create<IConsoleArgumentsProvider>();
            string newFileName = Path.GetTempFileName();
            Mock.Arrange(() => consoleArgumentsProvider.NewTestResultPath).Returns(newFileName);
            var fileSystemProvider = new FileSystemProvider(consoleArgumentsProvider);
            var failedTestsRun = fileSystemProvider.DeserializeTestRun("Exceptions.trx");
            var microsoftTestTestRunProvider = new MsTestTestRunProvider(consoleArgumentsProvider, log);

            var failedTests = microsoftTestTestRunProvider.GetAllNotPassedTests(failedTestsRun.Results.ToList());
            
            microsoftTestTestRunProvider.UpdatePassedTests(failedTests, failedTestsRun.Results.ToList());
            microsoftTestTestRunProvider.UpdateResultsSummary(failedTestsRun);

            Assert.AreEqual<int>(2, failedTestsRun.ResultSummary.Counters.Passed);
        }

        [TestMethod]
        public void SetCorrectPassedCounter_WhenFailedTestsPresent()
        {
            var log = Mock.Create<ILog>();
            Mock.Arrange(() => log.Info(Arg.AnyString));
            var consoleArgumentsProvider = Mock.Create<IConsoleArgumentsProvider>();
            string newFileName = Path.GetTempFileName();
            Mock.Arrange(() => consoleArgumentsProvider.NewTestResultPath).Returns(newFileName);
            var fileSystemProvider = new FileSystemProvider(consoleArgumentsProvider);
            var failedTestsRun = fileSystemProvider.DeserializeTestRun("Exceptions.trx");
            var microsoftTestTestRunProvider = new MsTestTestRunProvider(consoleArgumentsProvider, log);

            microsoftTestTestRunProvider.UpdateResultsSummary(failedTestsRun);

            Assert.AreEqual<int>(1, failedTestsRun.ResultSummary.Counters.Passed);
        }

        [TestMethod]
        public void SetPassedSummaryOutcome_WhenNoFailedTestsPresentAfterRerun()
        {
            var log = Mock.Create<ILog>();
            Mock.Arrange(() => log.Info(Arg.AnyString));
            var consoleArgumentsProvider = Mock.Create<IConsoleArgumentsProvider>();
            string newFileName = Path.GetTempFileName();
            Mock.Arrange(() => consoleArgumentsProvider.NewTestResultPath).Returns(newFileName);
            var fileSystemProvider = new FileSystemProvider(consoleArgumentsProvider);
            var failedTestsRun = fileSystemProvider.DeserializeTestRun("Exceptions.trx");
            var microsoftTestTestRunProvider = new MsTestTestRunProvider(consoleArgumentsProvider, log);

            var failedTests = microsoftTestTestRunProvider.GetAllNotPassedTests(failedTestsRun.Results.ToList());
            
            microsoftTestTestRunProvider.UpdatePassedTests(failedTests, failedTestsRun.Results.ToList());
            microsoftTestTestRunProvider.UpdateResultsSummary(failedTestsRun);

            Assert.AreEqual<string>("Passed", failedTestsRun.ResultSummary.Outcome);
        }

        [TestMethod]
        public void SetFailedSummaryOutcome_WhenOneFailedTestPresent()
        {
            var log = Mock.Create<ILog>();
            Mock.Arrange(() => log.Info(Arg.AnyString));
            var consoleArgumentsProvider = Mock.Create<IConsoleArgumentsProvider>();
            string newFileName = Path.GetTempFileName();
            Mock.Arrange(() => consoleArgumentsProvider.NewTestResultPath).Returns(newFileName);
            var fileSystemProvider = new FileSystemProvider(consoleArgumentsProvider);
            var failedTestsRun = fileSystemProvider.DeserializeTestRun("Exceptions.trx");
            var microsoftTestTestRunProvider = new MsTestTestRunProvider(consoleArgumentsProvider, log);

            microsoftTestTestRunProvider.UpdateResultsSummary(failedTestsRun);

            Assert.AreEqual<string>("Failed", failedTestsRun.ResultSummary.Outcome);
        }

        [TestMethod]
        public void SetCorrectFailedCounter_WhenNoFailedTestsPresent()
        {
            var log = Mock.Create<ILog>();
            Mock.Arrange(() => log.Info(Arg.AnyString));
            var consoleArgumentsProvider = Mock.Create<IConsoleArgumentsProvider>();
            string newFileName = Path.GetTempFileName();
            Mock.Arrange(() => consoleArgumentsProvider.NewTestResultPath).Returns(newFileName);
            var fileSystemProvider = new FileSystemProvider(consoleArgumentsProvider);
            var failedTestsRun = fileSystemProvider.DeserializeTestRun("Exceptions.trx");
            var microsoftTestTestRunProvider = new MsTestTestRunProvider(consoleArgumentsProvider, log);

            var failedTests = microsoftTestTestRunProvider.GetAllNotPassedTests(failedTestsRun.Results.ToList());
            
            microsoftTestTestRunProvider.UpdatePassedTests(failedTests, failedTestsRun.Results.ToList());
            microsoftTestTestRunProvider.UpdateResultsSummary(failedTestsRun);

            Assert.AreEqual<int>(0, failedTestsRun.ResultSummary.Counters.Failed);
        }

        [TestMethod]
        public void SetCorrectFailedCounter_WhenFailedTestsPresent()
        {
            var log = Mock.Create<ILog>();
            Mock.Arrange(() => log.Info(Arg.AnyString));
            var consoleArgumentsProvider = Mock.Create<IConsoleArgumentsProvider>();
            string newFileName = Path.GetTempFileName();
            Mock.Arrange(() => consoleArgumentsProvider.NewTestResultPath).Returns(newFileName);
            var fileSystemProvider = new FileSystemProvider(consoleArgumentsProvider);
            var failedTestsRun = fileSystemProvider.DeserializeTestRun("Exceptions.trx");
            var microsoftTestTestRunProvider = new MsTestTestRunProvider(consoleArgumentsProvider, log);

            microsoftTestTestRunProvider.UpdateResultsSummary(failedTestsRun);

            Assert.AreEqual<int>(1, failedTestsRun.ResultSummary.Counters.Failed);
        }
    }
}