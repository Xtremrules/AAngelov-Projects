using System.Diagnostics;

namespace MSTest.Console.Extended.Interfaces
{
    public interface IProcessExecutionProvider
    {
        string ProcessName { get; }

        Process CurrentProcess { get; }

        void Execute(string arguments = "");  

        void WaitForCurrentProcessExit();
    }
}