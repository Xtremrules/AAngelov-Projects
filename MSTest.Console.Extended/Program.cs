using System;
using System.Configuration;
using log4net;
using MSTest.Console.Extended.Infrastructure;
using MSTest.Console.Extended.Services;

namespace MSTest.Console.Extended
{
    public class Program
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));

        public static void Main(string[] arguments)
        {
            string microsoftTestConsoleExePath = ConfigurationManager.AppSettings["MSTestConsoleRunnerPath"];

            var consoleArgumentsProvider = new ConsoleArgumentsProvider(arguments);

            var fileSystemProvider = new FileSystemProvider(consoleArgumentsProvider);
            var testRunProviderLogger = LogManager.GetLogger(typeof(MsTestTestRunProvider));
            var testRunprovider = new MsTestTestRunProvider(consoleArgumentsProvider, fileSystemProvider, testRunProviderLogger);
            var processExecutionProvider = new ProcessExecutionProvider(microsoftTestConsoleExePath, consoleArgumentsProvider, LogManager.GetLogger(typeof(ProcessExecutionProvider)));
            var testExecutionProviderLogger = LogManager.GetLogger(typeof(TestExecutionService));

            var engine = new TestExecutionService(
                testRunprovider,
                fileSystemProvider,
                processExecutionProvider,
                consoleArgumentsProvider,
                testExecutionProviderLogger);

            try
            {
                int result = engine.ExecuteWithRetry();
                Environment.Exit(result);
            }
            catch (Exception ex)
            {
                Logger.Error(string.Concat(ex.Message, ex.StackTrace));
            }
        }
    }
}