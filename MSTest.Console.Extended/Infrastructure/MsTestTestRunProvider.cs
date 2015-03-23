using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MSTest.Console.Extended.Data;
using MSTest.Console.Extended.Interfaces;
using log4net;

namespace MSTest.Console.Extended.Infrastructure
{
    public class MsTestTestRunProvider : IMsTestTestRunProvider
    {
        private const string PassedOutcome = "Passed";
        private const string FailedOutocme = "Failed";

        private readonly ILog log;
        private readonly IConsoleArgumentsProvider consoleArgumentsProvider;
        private readonly IFileSystemProvider fileSystemProvider;

        public MsTestTestRunProvider(IConsoleArgumentsProvider consoleArgumentsProvider, ILog log)
        {
            this.consoleArgumentsProvider = consoleArgumentsProvider;
            this.fileSystemProvider = new FileSystemProvider(this.consoleArgumentsProvider);
            this.log = log;
        }

        public void UpdateInitialTestRun(TestRun initialTestRun, TestRun retryTestRun)
        {
            var initialResults = initialTestRun.Results.ToList();
            foreach (var retryResult in retryTestRun.Results)
            {
                var initialResult = initialTestRun.Results.Where(x=>x.TestId == retryResult.TestId).First();
                var initialResultIndex = initialResults.IndexOf(initialResult);

                this.fileSystemProvider.ReplaceTestResultFiles(initialTestRun, initialResult, retryTestRun, retryResult);

                TestRunUnitTestResult updatedTestResult = this.UpdateTestResultInformation(initialResult, retryResult);
                initialTestRun.Results[initialResultIndex] = updatedTestResult;
            }

            this.UpdateResultsSummary(initialTestRun);
        }

        
        public List<TestRunUnitTestResult> GetAllNotPassedTests(List<TestRunUnitTestResult> allTests)
        {
            List<TestRunUnitTestResult> failedTests = new List<TestRunUnitTestResult>();
            failedTests = allTests.Where(x => !x.Outcome.Equals(PassedOutcome)).ToList();

            return failedTests;
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

        public string GenerateAdditionalArgumentsForFailedTestsRun(List<TestRunUnitTestResult> failedTests, string newTestResultFilePath)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" ");

            foreach (var test in failedTests)
            {
                sb.AppendFormat("/test:{0} ", test.TestName);
                System.Console.WriteLine("##### MSTestRetrier: Execute again {0}", test.TestName);
                this.log.InfoFormat("##### MSTestRetrier: Execute again {0}", test.TestName);
            }

            string additionalArgumentsForFailedTestsRun = string.Concat(this.consoleArgumentsProvider.ConsoleArguments, sb.ToString());
            additionalArgumentsForFailedTestsRun = additionalArgumentsForFailedTestsRun.Replace(this.consoleArgumentsProvider.TestResultPath, newTestResultFilePath);
            additionalArgumentsForFailedTestsRun = additionalArgumentsForFailedTestsRun.TrimEnd();

            return additionalArgumentsForFailedTestsRun;
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

        private TestRunUnitTestResult UpdateTestResultInformation(TestRunUnitTestResult originalResult, TestRunUnitTestResult retryResult)
        {
            TestRunUnitTestResult updatedResult = retryResult;

            updatedResult.ExecutionId = originalResult.ExecutionId;
            updatedResult.RelativeResultsDirectory = originalResult.RelativeResultsDirectory;

            if (updatedResult.InnerResults != null)
            {
                foreach (var innerResult in retryResult.InnerResults)
                {
                    innerResult.ParentExecutionId = originalResult.ExecutionId;
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