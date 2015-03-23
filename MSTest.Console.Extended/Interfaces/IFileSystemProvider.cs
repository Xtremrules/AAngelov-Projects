using System;
using System.Collections.Generic;
using MSTest.Console.Extended.Data;

namespace MSTest.Console.Extended.Interfaces
{
    public interface IFileSystemProvider
    {
        void SerializeTestRun(TestRun updatedTestRun);

        TestRun DeserializeTestRun(string resultsPath = "");

        void DeleteTestResultFiles();

        void ReplaceTestResultResultsFiles(TestRun sourceRun, TestRunUnitTestResult sourceResult, TestRun targetRun, TestRunUnitTestResult targetResult);
    }
}