using System;
using System.Diagnostics;
using log4net;
using MSTest.Console.Extended.Interfaces;

namespace MSTest.Console.Extended.Infrastructure
{
    public class ProcessExecutionProvider : IProcessExecutionProvider
    {
        private readonly ILog log;
        private readonly IConsoleArgumentsProvider consoleArgumentsProvider;
        private string processName;

        public ProcessExecutionProvider(string processName, IConsoleArgumentsProvider consoleArgumentsProvider, ILog log)
        {
            this.ProcessName = processName;
            this.consoleArgumentsProvider = consoleArgumentsProvider;
            this.log = log;
        }

        public string ProcessName
        {
            get
            {
                return this.processName;
            }

            private set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("Process name cannot be null or empty string.");    
                }

                this.processName = value;
            }
        }

        public Process CurrentProcess
        {
            get;
            private set;
        }

        public void Execute(string arguments = "")
        {
            if (string.IsNullOrEmpty(arguments))
            {
                arguments = this.consoleArgumentsProvider.ConsoleArguments;
            }

            ProcessStartInfo processStartInfo = new ProcessStartInfo(this.ProcessName, arguments);
            processStartInfo.WindowStyle = ProcessWindowStyle.Normal;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;
            processStartInfo.UseShellExecute = false;

            this.CurrentProcess = new Process();
            this.CurrentProcess.StartInfo = processStartInfo;
            this.CurrentProcess.OutputDataReceived += (sender, args) =>
            {
                System.Console.WriteLine(args.Data);
                this.log.Info(args.Data);
            };

            this.CurrentProcess.Start();

            if (this.CurrentProcess != null)
            {
                this.CurrentProcess.BeginErrorReadLine();
            }

            if (this.CurrentProcess != null)
            {
                this.CurrentProcess.BeginOutputReadLine();
            }
        }

        public void WaitForCurrentProcessExit()
        {
            this.CurrentProcess.WaitForExit();
        }
    }
}