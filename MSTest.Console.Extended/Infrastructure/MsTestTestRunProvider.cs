﻿using System;
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

            sb.Append("/unique");

            string additionalArgumentsForFailedTestsRun = string.Concat(this.consoleArgumentsProvider.StandardArguments, sb.ToString());
            additionalArgumentsForFailedTestsRun = additionalArgumentsForFailedTestsRun.Replace(this.consoleArgumentsProvider.ResultsFilePath, newResultsFilePath);
            additionalArgumentsForFailedTestsRun = additionalArgumentsForFailedTestsRun.TrimEnd();

            return additionalArgumentsForFailedTestsRun;
        }

        public void UpdateTestRun(TestRun source, TestRun target)
        {
            this.fileSystemProvider.ReplaceTestResultsFiles(source, target);

            this.UpdateTestResultsInfo(source, target);

            this.UpdateResultsSummary(target);
        }

        private void UpdateTestResultsInfo(TestRun source, TestRun target)
        {
            var targetResults = target.Results.ToList();
            foreach (var sourceResult in source.Results)
            {
                var targetResult = targetResults.Where(x => x.TestId == sourceResult.TestId).First();

                TestRunUnitTestResult updatedTestResult = this.GetUpdatedTestResult(sourceResult, targetResult);
                var targetResultIndex = targetResults.IndexOf(targetResult);
                target.Results[targetResultIndex] = updatedTestResult;
            }
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