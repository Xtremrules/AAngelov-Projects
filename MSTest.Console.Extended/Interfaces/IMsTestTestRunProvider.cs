using System.Collections.Generic;
using MSTest.Console.Extended.Data;

namespace MSTest.Console.Extended.Interfaces
{
    public interface IMsTestTestRunProvider
    {
        void UpdateInitialTestRun(TestRun originalTestRun, TestRun newTestRun);

        List<TestRunUnitTestResult> GetAllNotPassedTests(List<TestRunUnitTestResult> allTests);

        string GenerateAdditionalArgumentsForFailedTestsRun(List<TestRunUnitTestResult> failedTests, string newTestResultFilePath);

        int CalculatedFailedTestsPercentage(List<TestRunUnitTestResult> failedTests, List<TestRunUnitTestResult> allTests);
    }
}