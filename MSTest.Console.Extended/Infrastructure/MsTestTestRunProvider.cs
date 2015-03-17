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

        public MsTestTestRunProvider(IConsoleArgumentsProvider consoleArgumentsProvider, ILog log)
        {
            this.consoleArgumentsProvider = consoleArgumentsProvider;
            this.log = log;
        }

        public List<TestRunUnitTestResult> GetAllPassedTests(TestRun testRun)
        {
            List<TestRunUnitTestResult> passedTests = new List<TestRunUnitTestResult>();

            passedTests = testRun.Results.Where(x => x.Outcome.Equals(PassedOutcome)).ToList();
            return passedTests;
        }

        public void UpdatePassedTests(List<TestRunUnitTestResult> passedTests, List<TestRunUnitTestResult> allTests)
        {
            foreach (var test in allTests)
            {
                bool testPassed = passedTests.Any(x => x.TestId.Equals(test.TestId));

                if (testPassed)
                {
                    test.Outcome = PassedOutcome;
                }
            }
        }

        public List<TestRunUnitTestResult> GetAllNotPassedTests(List<TestRunUnitTestResult> allTests)
        {
            List<TestRunUnitTestResult> failedTests = new List<TestRunUnitTestResult>();
            failedTests = allTests.Where(x => !x.Outcome.Equals(PassedOutcome)).ToList();

            return failedTests;
        }

        public void UpdateResultsSummary(TestRun testRun)
        {
            testRun.ResultSummary.Counters.Failed = (byte)testRun.Results.ToList().Count(x => x.Outcome.Equals(FailedOutocme));
            testRun.ResultSummary.Counters.Passed = (byte)testRun.Results.ToList().Count(x => x.Outcome.Equals(PassedOutcome));

            if ((int)testRun.ResultSummary.Counters.Passed != testRun.Results.Length)
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
    }
}