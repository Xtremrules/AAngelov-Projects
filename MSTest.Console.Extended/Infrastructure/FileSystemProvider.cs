using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using MSTest.Console.Extended.Data;
using MSTest.Console.Extended.Interfaces;

namespace MSTest.Console.Extended.Infrastructure
{
    public class FileSystemProvider : IFileSystemProvider
    {
        private readonly IConsoleArgumentsProvider consoleArgumentsProvider;

        public FileSystemProvider(IConsoleArgumentsProvider consoleArgumentsProvider)
        {
            this.consoleArgumentsProvider = consoleArgumentsProvider;
        }

        public void SerializeTestRun(TestRun testRun)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(TestRun));
            TextWriter writer = new StreamWriter(this.consoleArgumentsProvider.NewResultsFilePath);

            using (writer)
            {
                serializer.Serialize(writer, testRun);
            }
        }

        public TestRun DeserializeTestRun(string resultsPath = "")
        {
            TestRun testRun = null;

            if (string.IsNullOrEmpty(resultsPath))
            {
                resultsPath = this.consoleArgumentsProvider.ResultsFilePath;
            }

            if (File.Exists(resultsPath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TestRun));
                StreamReader reader = new StreamReader(resultsPath);

                using (reader)
                {
                    testRun = (TestRun)serializer.Deserialize(reader);
                }
            }

            return testRun;
        }

        public void DeleteTestResultFiles()
        {
            if (this.consoleArgumentsProvider.ShouldDeleteOldResultsFiles)
            {
                var filesToBeDeleted = new List<string>()
                {
                    this.consoleArgumentsProvider.ResultsFilePath,
                    this.consoleArgumentsProvider.NewResultsFilePath
                };

                foreach (var currentFilePath in filesToBeDeleted)
                {
                    if (File.Exists(currentFilePath))
                    {
                        File.Delete(currentFilePath);
                    }
                }
            }
        }

        public void ReplaceTestResultResultsFiles(TestRun sourceRun, TestRunUnitTestResult sourceResult, TestRun targetRun, TestRunUnitTestResult targetResult)
        {
            var sourceResultFiles = this.GetTestResultFilesPaths(sourceRun, sourceResult);
            var destinationResultFiles = this.GetTestResultFilesPaths(targetRun, targetResult);

            if (sourceResultFiles.Count != destinationResultFiles.Count)
            {
                string message = string.Format("Source and destination files have different count: source ({0}) - destination ({1})", sourceResultFiles.Count, destinationResultFiles.Count);
                throw new InvalidOperationException(message);
            }

            for (int i = 0; i < sourceResultFiles.Count; i++)
            {
                var destinationFile = destinationResultFiles[i];

                if (File.Exists(destinationFile))
                {
                    File.Delete(destinationFile);
                }

                var sourceFile = sourceResultFiles[i];
                File.Copy(sourceFile, destinationFile);
            }
        }

        private IList<string> GetTestResultFilesPaths(TestRun run, TestRunUnitTestResult result)
        {
            IList<string> filesPaths = new List<string>();

            if (result.ResultFiles != null && result.ResultFiles.Length > 0)
            {
                string baseResultsFolder = Path.Combine(run.TestSettings.Deployment.UserDeploymentRoot,
                      run.TestSettings.Deployment.RunDeploymentRoot,
                      "In",
                      result.ExecutionId);

                foreach (var file in result.ResultFiles)
                {
                    string filePath = Path.Combine(baseResultsFolder, file.Path);
                    filesPaths.Add(filePath);
                }
            }

            return filesPaths;
        }


        public void ReplaceFiles(TestRun sourceRun, TestRunUnitTestResult sourceResult, TestRun targetRun, TestRunUnitTestResult targetResult)
        {
            throw new NotImplementedException();
        }
    }
}