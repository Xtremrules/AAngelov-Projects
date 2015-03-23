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
            TextWriter writer = new StreamWriter(this.consoleArgumentsProvider.NewTestResultPath);

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
                resultsPath = this.consoleArgumentsProvider.TestResultPath;
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
            if (this.consoleArgumentsProvider.ShouldDeleteOldTestResultFiles)
            {
                var filesToBeDeleted = new List<string>()
                {
                    this.consoleArgumentsProvider.TestResultPath,
                    this.consoleArgumentsProvider.NewTestResultPath
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

        public void ReplaceFiles(IList<string> sourceFiles, IList<string> destinationFiles)
        {
            if (sourceFiles.Count != destinationFiles.Count)
            {
                string message = string.Format("Source and destination files have different count: source ({0}) - destination ({1})", sourceFiles.Count, destinationFiles.Count);
                throw new InvalidOperationException(message);
            }

            for (int i = 0; i < sourceFiles.Count; i++)
            {
                var destinationFile = destinationFiles[i];

                if (File.Exists(destinationFile))
                {
                    File.Delete(destinationFile);
                }

                var sourceFile = sourceFiles[i];
                File.Copy(sourceFile, destinationFile);
            }
        }
    }
}