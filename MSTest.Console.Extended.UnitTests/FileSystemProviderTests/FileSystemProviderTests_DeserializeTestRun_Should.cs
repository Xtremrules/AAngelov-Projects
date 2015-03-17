﻿using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSTest.Console.Extended.Infrastructure;
using MSTest.Console.Extended.Interfaces;
using Telerik.JustMock;

namespace MSTest.Console.Extended.UnitTests.FileSystemProviderTests
{
    [TestClass]
    public class FileSystemProviderTests_DeserializeTestRun_Should
    {
        [TestMethod]
        public void DeserializeTestResultsFile_WhenNoFailedTestsPresent()
        {
            var consoleArgumentsProvider = Mock.Create<IConsoleArgumentsProvider>();
            string newFileName = Path.GetTempFileName();
            Mock.Arrange(() => consoleArgumentsProvider.NewTestResultPath).Returns(newFileName);
            var fileSystemProvider = new FileSystemProvider(consoleArgumentsProvider);
            var testRun = fileSystemProvider.DeserializeTestRun("NoExceptions.trx");
            Assert.AreEqual<int>(2, testRun.Results.Count());
            Assert.AreEqual<string>("Passed", testRun.Results.First().Outcome);
            Assert.IsNotNull(testRun.ResultSummary);
            Assert.AreEqual<int>(2, testRun.ResultSummary.Counters.Total);
            Assert.AreEqual<int>(2, testRun.ResultSummary.Counters.Passed);
            Assert.AreEqual<int>(0, testRun.ResultSummary.Counters.Failed);
            Assert.AreEqual<string>("Passed", testRun.ResultSummary.Outcome);
        }

        [TestMethod]
        public void DeserializeTestResultsFile_WhenFailedTestsPresent()
        {
            var consoleArgumentsProvider = Mock.Create<IConsoleArgumentsProvider>();
            string newFileName = Path.GetTempFileName();
            Mock.Arrange(() => consoleArgumentsProvider.NewTestResultPath).Returns(newFileName);
            var fileSystemProvider = new FileSystemProvider(consoleArgumentsProvider);
            var testRun = fileSystemProvider.DeserializeTestRun("Exceptions.trx");
            Assert.AreEqual<int>(2, testRun.Results.Count());
            Assert.AreEqual<string>("Failed", testRun.Results.First().Outcome);
            Assert.IsNotNull(testRun.ResultSummary);
            Assert.AreEqual<int>(2, testRun.ResultSummary.Counters.Total);
            Assert.AreEqual<int>(1, testRun.ResultSummary.Counters.Passed);
            Assert.AreEqual<int>(1, testRun.ResultSummary.Counters.Failed);
            Assert.AreEqual<string>("Failed", testRun.ResultSummary.Outcome);
        }

        [TestMethod]
        public void DeserializeTestResultsFile_WhenFailedTestsPresentAndNoTestResultsFilePassed()
        {
            var consoleArgumentsProvider = Mock.Create<IConsoleArgumentsProvider>();
            Mock.Arrange(() => consoleArgumentsProvider.TestResultPath).Returns("Exceptions.trx");
            var fileSystemProvider = new FileSystemProvider(consoleArgumentsProvider);
            var testRun = fileSystemProvider.DeserializeTestRun();
            Assert.AreEqual<int>(2, testRun.Results.Count());
            Assert.AreEqual<string>("Failed", testRun.Results.First().Outcome);
            Assert.IsNotNull(testRun.ResultSummary);
            Assert.AreEqual<int>(2, testRun.ResultSummary.Counters.Total);
            Assert.AreEqual<int>(1, testRun.ResultSummary.Counters.Passed);
            Assert.AreEqual<int>(1, testRun.ResultSummary.Counters.Failed);
            Assert.AreEqual<string>("Failed", testRun.ResultSummary.Outcome);
        }
    }
}