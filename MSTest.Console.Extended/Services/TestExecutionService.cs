﻿using System.IO;
using System.Linq;
using log4net;
using MSTest.Console.Extended.Interfaces;

namespace MSTest.Console.Extended.Services
{
    public class TestExecutionService
    {
        private readonly ILog log;

        private readonly IMsTestTestRunProvider microsoftTestTestRunProvider;

        private readonly IFileSystemProvider fileSystemProvider;

        private readonly IProcessExecutionProvider processExecutionProvider;

        private readonly IConsoleArgumentsProvider consoleArgumentsProvider;

        public TestExecutionService(
            IMsTestTestRunProvider microsoftTestTestRunProvider,
            IFileSystemProvider fileSystemProvider,
            IProcessExecutionProvider processExecutionProvider,
            IConsoleArgumentsProvider consoleArgumentsProvider,
            ILog log)
        {
            this.microsoftTestTestRunProvider = microsoftTestTestRunProvider;
            this.fileSystemProvider = fileSystemProvider;
            this.processExecutionProvider = processExecutionProvider;
            this.consoleArgumentsProvider = consoleArgumentsProvider;
            this.log = log;
        }
        
        public int ExecuteWithRetry()
        {
            this.fileSystemProvider.DeleteTestResultFiles();

            this.processExecutionProvider.Execute();
            this.processExecutionProvider.WaitForCurrentProcessExit();
            var initialTestRun = this.fileSystemProvider.DeserializeTestRun();

            var initialTestRunResults = initialTestRun.Results.ToList();
            var failedTests = this.microsoftTestTestRunProvider.GetAllNotPassedTests(initialTestRunResults);
            int failedTestsPercentage = this.microsoftTestTestRunProvider.CalculatedFailedTestsPercentage(failedTests, initialTestRunResults);

            if (failedTestsPercentage < this.consoleArgumentsProvider.FailedTestsThreshold)
            {
                for (int i = 0; i < this.consoleArgumentsProvider.RetriesCount; i++)
                {
                    this.log.InfoFormat("Start to execute again {0} failed tests.", failedTests.Count);
                    if (failedTests.Count > 0)
                    {
                        string retryTestRunResultsFilePath = this.GetRetryTestRunResultsFilePath(i + 1);
                        string retryTestRunArguments = this.microsoftTestTestRunProvider.GenerateAdditionalArgumentsForFailedTestsRun(failedTests, retryTestRunResultsFilePath);

                        this.log.InfoFormat("Run {0} time with arguments {1}", i + 2, retryTestRunArguments);
                        this.processExecutionProvider.Execute(retryTestRunArguments);
                        this.processExecutionProvider.WaitForCurrentProcessExit();

                        var retryTestRun = this.fileSystemProvider.DeserializeTestRun(retryTestRunResultsFilePath);

                        this.microsoftTestTestRunProvider.UpdateTestRun(retryTestRun, initialTestRun);
                    }
                    else
                    {
                        break;
                    }

                    failedTests = this.microsoftTestTestRunProvider.GetAllNotPassedTests(initialTestRun.Results.ToList());
                }
            }

            this.fileSystemProvider.SerializeTestRun(initialTestRun);

            int exitCode = 0;
            if (failedTests.Count > 0)
            {
                exitCode = 1;
            }

            return exitCode;
        }

        private string GetRetryTestRunResultsFilePath(int retryIndex)
        {
            string initialResultsFileName = Path.GetFileNameWithoutExtension(this.consoleArgumentsProvider.ResultsFilePath);
            string initialResultsDirectory = Path.GetDirectoryName(this.consoleArgumentsProvider.ResultsFilePath);

            string retryResultsFileName = string.Format("{0}-Retry{1}.trx", initialResultsFileName, retryIndex);

            string retryTestRunResultsFilePath = Path.Combine(initialResultsDirectory, retryResultsFileName); 

            return retryTestRunResultsFilePath;
        }
    }
}