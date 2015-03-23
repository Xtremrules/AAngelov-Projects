using System.Collections.Generic;
using MSTest.Console.Extended.Data;

namespace MSTest.Console.Extended.Interfaces
{
    public interface IMsTestTestRunProvider
    {
        List<TestRunUnitTestResult> GetAllNotPassedTests(List<TestRunUnitTestResult> allTests);

        int CalculatedFailedTestsPercentage(List<TestRunUnitTestResult> failedTests, List<TestRunUnitTestResult> allTests);

        string GenerateAdditionalArgumentsForFailedTestsRun(List<TestRunUnitTestResult> failedTests, string newTestResultFilePath);

        void UpdateTestRun(TestRun source, TestRun target);
    }
}