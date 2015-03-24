using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MSTest.Console.Extended.Infrastructure;
using MSTest.Console.Extended.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using System.Xml;
using System.Xml.Linq;
using MSTest.Console.Extended.Data;

namespace MSTest.Console.Extended.UnitTests
{
    [TestClass]
    public class FileSystemProviderTests
    {
        [TestMethod]
        public void DeleteTestResultFiles_ShouldNotDeleteFiles_WhenShouldDeleteOldResultsFilesIsFalse()
        {
            var consoleArgumentsProvider = Mock.Create<IConsoleArgumentsProvider>();
            string newFileName = Path.GetTempFileName();
            var file = File.CreateText(newFileName);
            file.Close();

            Mock.Arrange(() => consoleArgumentsProvider.ShouldDeleteOldResultsFiles).Returns(false);
            var fileSystemProvider = new FileSystemProvider(consoleArgumentsProvider);
            fileSystemProvider.DeleteTestResultFiles();

            Assert.IsTrue(File.Exists(newFileName));
        }

        [TestMethod]
        public void DeleteTestResultFiles_ShouldDeleteOldResultsFiles_WhenShouldDeleteOldResultsFilesIsTrueAndTwoFilesExist()
        {
            var consoleArgumentsProvider = Mock.Create<IConsoleArgumentsProvider>();
            string newFileName = Path.GetTempFileName();
            var file = File.CreateText(newFileName);
            file.Close();
            string newFileName1 = Path.GetTempFileName();
            file = File.CreateText(newFileName);
            file.Close();
            Mock.Arrange(() => consoleArgumentsProvider.ShouldDeleteOldResultsFiles).Returns(true);
            Mock.Arrange(() => consoleArgumentsProvider.ResultsFilePath).Returns(newFileName);
            Mock.Arrange(() => consoleArgumentsProvider.NewResultsFilePath).Returns(newFileName1);
            var fileSystemProvider = new FileSystemProvider(consoleArgumentsProvider);
            fileSystemProvider.DeleteTestResultFiles();

            Assert.IsFalse(File.Exists(newFileName));
            Assert.IsFalse(File.Exists(newFileName1));
        }

        [TestMethod]
        public void DeleteTestResultFiles_ShouldDeleteOldResultsFiles_WhenShouldDeleteOldTestResultFilesIsTrueAndNoNewResultsFileExist()
        {
            var consoleArgumentsProvider = Mock.Create<IConsoleArgumentsProvider>();
            string newFileName = Path.GetTempFileName();
            var file = File.CreateText(newFileName);
            file.Close();
            Mock.Arrange(() => consoleArgumentsProvider.ShouldDeleteOldResultsFiles).Returns(true);
            Mock.Arrange(() => consoleArgumentsProvider.ResultsFilePath).Returns(newFileName);
            Mock.Arrange(() => consoleArgumentsProvider.NewResultsFilePath).Returns(newFileName);
            var fileSystemProvider = new FileSystemProvider(consoleArgumentsProvider);
            fileSystemProvider.DeleteTestResultFiles();

            Assert.IsFalse(File.Exists(newFileName));
        }
        [TestMethod]
        public void DeserializeTestRun_ShouldDeserializeTestResultsFileCorrectly_WhenNoFailedTestsPresent()
        {
            var consoleArgumentsProvider = Mock.Create<IConsoleArgumentsProvider>();
            string newFileName = Path.GetTempFileName();
            Mock.Arrange(() => consoleArgumentsProvider.NewResultsFilePath).Returns(newFileName);
            var fileSystemProvider = new FileSystemProvider(consoleArgumentsProvider);
            var testRun = fileSystemProvider.DeserializeTestRun("Resources\\NoExceptions.trx");
            Assert.AreEqual<int>(2, testRun.Results.Count());
            Assert.AreEqual<string>("Passed", testRun.Results.First().Outcome);
            Assert.IsNotNull(testRun.ResultSummary);
            Assert.AreEqual<int>(2, testRun.ResultSummary.Counters.Total);
            Assert.AreEqual<int>(2, testRun.ResultSummary.Counters.Passed);
            Assert.AreEqual<int>(0, testRun.ResultSummary.Counters.Failed);
            Assert.AreEqual<string>("Passed", testRun.ResultSummary.Outcome);
        }

        [TestMethod]
        public void DeserializeTestRun_ShouldDeserializeTestResultsFileCorrectly_WhenFailedTestsPresent()
        {
            var consoleArgumentsProvider = Mock.Create<IConsoleArgumentsProvider>();
            string newFileName = Path.GetTempFileName();
            Mock.Arrange(() => consoleArgumentsProvider.NewResultsFilePath).Returns(newFileName);
            var fileSystemProvider = new FileSystemProvider(consoleArgumentsProvider);
            var testRun = fileSystemProvider.DeserializeTestRun("Resources\\Exceptions.trx");
            Assert.AreEqual<int>(2, testRun.Results.Count());
            Assert.AreEqual<string>("Failed", testRun.Results.First().Outcome);
            Assert.IsNotNull(testRun.ResultSummary);
            Assert.AreEqual<int>(2, testRun.ResultSummary.Counters.Total);
            Assert.AreEqual<int>(1, testRun.ResultSummary.Counters.Passed);
            Assert.AreEqual<int>(1, testRun.ResultSummary.Counters.Failed);
            Assert.AreEqual<string>("Failed", testRun.ResultSummary.Outcome);
        }

        [TestMethod]
        public void DeserializeTestRun_ShouldDeserializeTestResultsFileCorrectly_WhenFailedTestsPresentAndNoTestResultsFilePassed()
        {
            var consoleArgumentsProvider = Mock.Create<IConsoleArgumentsProvider>();
            Mock.Arrange(() => consoleArgumentsProvider.ResultsFilePath).Returns("Resources\\Exceptions.trx");
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

        [TestMethod]
        public void SerializeTestRun_ShouldSerializeTestResultsFileCorrectly_WhenNoFailedTestsPresent()
        {
            var consoleArgumentsProvider = Mock.Create<IConsoleArgumentsProvider>();
            string newFileName = Path.GetTempFileName();
            Mock.Arrange(() => consoleArgumentsProvider.NewResultsFilePath).Returns(newFileName);
            var fileSystemProvider = new FileSystemProvider(consoleArgumentsProvider);
            var testRun = fileSystemProvider.DeserializeTestRun("Resources\\NoExceptions.trx");

            fileSystemProvider.SerializeTestRun(testRun);

            string originalFileContent = File.ReadAllText("Resources\\NoExceptions.trx");
            string newFileContent = File.ReadAllText(newFileName);
            var originalDoc = new XmlDocument();
            originalDoc.LoadXml(originalFileContent);
            var newDoc = new XmlDocument();
            newDoc.LoadXml(newFileContent);
            var originalXElement = XElement.Parse(originalFileContent);
            var newXElement = XElement.Parse(newFileContent);
            bool documentsAreEqual = XElement.DeepEquals(originalXElement, newXElement);
            Assert.IsTrue(documentsAreEqual);
        }

        [TestMethod]
        public void SerializeTestRun_ShouldSerializeTestResultsFileCorrectly_WhenFailedTestsPresent()
        {
            var consoleArgumentsProvider = Mock.Create<IConsoleArgumentsProvider>();
            string newFileName = Path.GetTempFileName();
            Mock.Arrange(() => consoleArgumentsProvider.NewResultsFilePath).Returns(newFileName);
            var fileSystemProvider = new FileSystemProvider(consoleArgumentsProvider);
            var testRun = fileSystemProvider.DeserializeTestRun("Resources\\Exceptions.trx");

            fileSystemProvider.SerializeTestRun(testRun);

            string originalFileContent = File.ReadAllText("Resources\\Exceptions.trx");
            string newFileContent = File.ReadAllText(newFileName);
            var originalDoc = new XmlDocument();
            originalDoc.LoadXml(originalFileContent);
            var newDoc = new XmlDocument();
            newDoc.LoadXml(newFileContent);
            var originalXElement = XElement.Parse(originalFileContent);
            var newXElement = XElement.Parse(newFileContent);
            bool documentsAreEqual = XElement.DeepEquals(originalXElement, newXElement);
            Assert.IsTrue(documentsAreEqual);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ReplaceTestResultsFiles_ShouldThrowException_WhenTargetRunResultsDoNotContainSourceRunResult()
        {
            var sourceRun = new TestRun();
            var sourceResult = new TestRunUnitTestResult();
            sourceResult.TestId = "sourceTestId";

            sourceRun.Results = new TestRunUnitTestResult[] { sourceResult };

            var targetRun = new TestRun();
            var targetResult = new TestRunUnitTestResult();
            targetResult.TestId = "targetRunTestId";

            targetRun.Results = new TestRunUnitTestResult[] { targetResult };

            var consoleArgumentsProvider = Mock.Create<IConsoleArgumentsProvider>();

            var fileSystemProvider = new FileSystemProvider(consoleArgumentsProvider);
            fileSystemProvider.ReplaceTestResultsFiles(sourceRun, targetRun);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ReplaceTestResultsFiles_ShouldThrowException_WhenSourceResultFilesCountDiffersTargetResultFilesCount()
        {
            var sourceRun = new TestRun();
            var sourceResult = new TestRunUnitTestResult();
            sourceResult.TestId = "testId";
            sourceResult.ExecutionId = "456";
            sourceResult.ResultFiles = new TestRunUnitTestResultResultFile[] { 
                new TestRunUnitTestResultResultFile(){ Path = "result1" }, 
                new TestRunUnitTestResultResultFile(){ Path = "result2" } 
            };
            sourceRun.TestSettings = new TestRunTestSettings();
            sourceRun.TestSettings.Deployment = new TestRunTestSettingsDeployment();
            sourceRun.TestSettings.Deployment.UserDeploymentRoot = "D:\\TestRuns";
            sourceRun.TestSettings.Deployment.RunDeploymentRoot = "Source";
            sourceRun.Results = new TestRunUnitTestResult[] { sourceResult };

            var targetRun = new TestRun();
            var targetResult = new TestRunUnitTestResult();
            targetResult.TestId = "testId";
            targetResult.ExecutionId = "123";
            targetResult.ResultFiles = new TestRunUnitTestResultResultFile[] { 
                new TestRunUnitTestResultResultFile() { Path = "result1" }
            };
            targetRun.TestSettings = new TestRunTestSettings();
            targetRun.TestSettings.Deployment = new TestRunTestSettingsDeployment();
            targetRun.TestSettings.Deployment.UserDeploymentRoot = "D:\\TestRuns";
            targetRun.TestSettings.Deployment.RunDeploymentRoot = "Target";
            targetRun.Results = new TestRunUnitTestResult[] { targetResult };

            var consoleArgumentsProvider = Mock.Create<IConsoleArgumentsProvider>();

            var fileSystemProvider = new FileSystemProvider(consoleArgumentsProvider);
            fileSystemProvider.ReplaceTestResultsFiles(sourceRun, targetRun);
        }
    }
}
