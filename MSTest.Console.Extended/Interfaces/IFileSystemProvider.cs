using MSTest.Console.Extended.Data;

namespace MSTest.Console.Extended.Interfaces
{
    public interface IFileSystemProvider
    {
        void SerializeTestRun(TestRun updatedTestRun);

        TestRun DeserializeTestRun(string resultsPath = "");

        void DeleteTestResultFiles();

        void ReplaceTestResultFiles(TestRun originalTestRun, TestRunUnitTestResult originalResult, TestRun retryTestRun, TestRunUnitTestResult retryResult);
    }
}