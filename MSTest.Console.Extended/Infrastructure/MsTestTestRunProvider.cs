using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using log4net;
using MSTest.Console.Extended.Data;
using MSTest.Console.Extended.Interfaces;

namespace MSTest.Console.Extended.Infrastructure
{
    public class MsTestTestRunProvider : IMsTestTestRunProvider
    {
        private const string PassedOutcome = "Passed";
        private const string FailedOutocme = "Failed";

        private readonly ILog log;
        private readonly IConsoleArgumentsProvider consoleArgumentsProvider;
        private readonly IFileSystemProvider fileSystemProvider;

        public MsTestTestRunProvider(IConsoleArgumentsProvider consoleArgumentsProvider, IFileSystemProvider fileSystemprovider, ILog log)
        {
            this.consoleArgumentsProvider = consoleArgumentsProvider;
            this.fileSystemProvider = fileSystemprovider;
            this.log = log;
        }

        public List<TestRunUnitTestResult> GetAllNotPassedTests(List<TestRunUnitTestResult> allTests)
        {
            List<TestRunUnitTestResult> failedTests = new List<TestRunUnitTestResult>();
            failedTests = allTests.Where(x => !x.Outcome.Equals(PassedOutcome)).ToList();

            return failedTests;
        }

        public int CalculatedFailedTestsPercentage(List<TestRunUnitTestResult> failedTests, List<TestRunUnitTestResult> allTests)
        {
            double result = 0;
            if (allTests.Count > 0)
            {
                result = ((double)failedTests.Count / (double)allTests.Count) * 100;
            }

            return (int)result;
        }

        public string GenerateAdditionalArgumentsForFailedTestsRun(List<TestRunUnitTestResult> failedTests, string newResultsFilePath)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" ");

            foreach (var test in failedTests)
            {
                sb.AppendFormat("/test:{0} ", test.TestName);
                System.Console.WriteLine("##### MSTestRetrier: Execute again {0}", test.TestName);
                this.log.InfoFormat("##### MSTestRetrier: Execute again {0}", test.TestName);
            }

            string additionalArgumentsForFailedTestsRun = string.Concat(this.consoleArgumentsProvider.StandardArguments, sb.ToString());
            additionalArgumentsForFailedTestsRun = additionalArgumentsForFailedTestsRun.Replace(this.consoleArgumentsProvider.ResultsFilePath, newResultsFilePath);
            additionalArgumentsForFailedTestsRun = additionalArgumentsForFailedTestsRun.TrimEnd();

            return additionalArgumentsForFailedTestsRun;
        }

        public void UpdateTestRun(TestRun source, TestRun target)
        {
            var targetResults = target.Results.ToList();
            foreach (var sourceResult in source.Results)
            {
                var targetResult = targetResults.Where(x => x.TestId == sourceResult.TestId).First();

                var sourceResultFilesPaths = this.GetTestResultFilesPaths(source, sourceResult);
                var targetResultFilesPaths = this.GetTestResultFilesPaths(target, targetResult);
                this.fileSystemProvider.ReplaceFiles(sourceResultFilesPaths, targetResultFilesPaths);

                TestRunUnitTestResult updatedTestResult = this.GetUpdatedTestResult(sourceResult, targetResult);
                var targetResultIndex = targetResults.IndexOf(targetResult);
                target.Results[targetResultIndex] = updatedTestResult;
            }

            this.UpdateResultsSummary(target);
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

        private void UpdateResultsSummary(TestRun testRun)
        {
            testRun.ResultSummary.Counters.Failed = this.GetSpecificOutcomeTestsCount(testRun, FailedOutocme);
            testRun.ResultSummary.Counters.Passed = this.GetSpecificOutcomeTestsCount(testRun, PassedOutcome);

            if (testRun.ResultSummary.Counters.Passed != testRun.ResultSummary.Counters.Executed)
            {
                testRun.ResultSummary.Outcome = FailedOutocme;
            }
            else
            {
                testRun.ResultSummary.Outcome = PassedOutcome;
            }
        }

        private TestRunUnitTestResult GetUpdatedTestResult(TestRunUnitTestResult source, TestRunUnitTestResult target)
        {
            TestRunUnitTestResult updatedResult = source;

            updatedResult.ExecutionId = target.ExecutionId;
            updatedResult.RelativeResultsDirectory = target.RelativeResultsDirectory;

            if (updatedResult.InnerResults != null)
            {
                foreach (var innerResult in source.InnerResults)
                {
                    innerResult.ParentExecutionId = target.ExecutionId;
                }
            }

            return updatedResult;
        }

        private int GetSpecificOutcomeTestsCount(TestRun testRun, string outcome)
        {
            int count = 0;
            foreach (var result in testRun.Results)
            {
                if (result.Outcome == outcome)
                {
                    count++;
                }

                if (result.InnerResults != null)
                {
                    count += result.InnerResults.Where(x => x.Outcome == outcome).Count();
                }
            }

            return count;
        }
    }
}