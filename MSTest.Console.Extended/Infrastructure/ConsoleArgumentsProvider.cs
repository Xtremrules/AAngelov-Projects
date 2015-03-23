using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using MSTest.Console.Extended.Interfaces;

namespace MSTest.Console.Extended.Infrastructure
{
    public class ConsoleArgumentsProvider : IConsoleArgumentsProvider
    {
        private const string TestResultFilePathRegexPattern = @".*resultsfile:(?<ResultsFilePath>[1-9A-Za-z\\:._]{1,})";
        private const string NewTestResultFilePathRegexPattern = @".*(?<NewResultsFilePathArgument>/newResultsfile:(?<NewResultsFilePath>[1-9A-Za-z\\:._]{1,}))";
        private const string RetriesCountRegexPattern = @".*(?<RetriesArgument>/retriesCount:(?<RetriesCount>[0-9]{1})).*";
        private const string FailedTestsThresholdRegexPattern = @".*(?<ThresholdArgument>/threshold:(?<ThresholdCount>[0-9]{1,2})).*";
        private const string DeleteOldFilesRegexPattern = @".*(?<DeleteOldFilesArgument>/deleteOldResultsFiles:(?<DeleteOldFilesValue>[a-zA-Z]{4,5})).*";
        private const string ArgumentRegexPattern = @".*/(?<ArgumentName>[a-zA-Z]{1,}):(?<ArgumentValue>.*)";

        public ConsoleArgumentsProvider(string[] arguments)
        {
            this.ConsoleArguments = this.InitializeInitialConsoleArguments(arguments);
            this.InitializeTestResultsPath();
            this.InitializeNewTestResultsPath();
            this.InitializeRetriesCount();
            this.InitializeFailedTestsThreshold();
            this.InitializeDeleteOldResultFiles();
        }

        public string ConsoleArguments
        {
            get;
            set;
        }

        public string TestResultPath
        {
            get;
            set;
        }

        public string NewTestResultPath
        {
            get;
            set;
        }

        public int RetriesCount
        {
            get;
            set;
        }

        public int FailedTestsThreshold
        {
            get;
            set;
        }

        public bool ShouldDeleteOldTestResultFiles
        {
            get;
            set;
        }

        private void InitializeTestResultsPath()
        {
            Regex testResultsPathRegex = this.GetArgumentRegex(TestResultFilePathRegexPattern);
            Match currentMatch = testResultsPathRegex.Match(this.ConsoleArguments);

            if (!currentMatch.Success)
            {
                throw new ArgumentException("You need to specify path to test results.");
            }

            this.TestResultPath = currentMatch.Groups["ResultsFilePath"].Value;
        }

        private void InitializeNewTestResultsPath()
        {
            Regex newTestResultsPathRegex = this.GetArgumentRegex(NewTestResultFilePathRegexPattern);
            Match currentMatch = newTestResultsPathRegex.Match(this.ConsoleArguments);

            if (!currentMatch.Success)
            {
                this.NewTestResultPath = this.TestResultPath;
            }
            else
            {
                this.NewTestResultPath = currentMatch.Groups["NewResultsFilePath"].Value;
                this.ConsoleArguments = this.ConsoleArguments.Replace(currentMatch.Groups["NewResultsFilePathArgument"].Value, string.Empty);
            }
        }

        private void InitializeRetriesCount()
        {
            Regex retriesCountRegex = this.GetArgumentRegex(RetriesCountRegexPattern);
            Match currentMatch = retriesCountRegex.Match(this.ConsoleArguments);

            if (!currentMatch.Success)
            {
                this.RetriesCount = 0;
            }
            else
            {
                this.RetriesCount = int.Parse(currentMatch.Groups["RetriesCount"].Value);
                this.ConsoleArguments = this.ConsoleArguments.Replace(currentMatch.Groups["RetriesArgument"].Value, string.Empty);
            }
        }

        private void InitializeFailedTestsThreshold()
        {
            Regex failedTestsThresholdRegex = this.GetArgumentRegex(FailedTestsThresholdRegexPattern);
            Match currentMatch = failedTestsThresholdRegex.Match(this.ConsoleArguments);

            if (!currentMatch.Success)
            {
                this.FailedTestsThreshold = int.Parse(ConfigurationManager.AppSettings["ThresholdDefaultPercentage"]);
            }
            else
            {
                this.FailedTestsThreshold = int.Parse(currentMatch.Groups["ThresholdCount"].Value);
                this.ConsoleArguments = this.ConsoleArguments.Replace(currentMatch.Groups["ThresholdArgument"].Value, string.Empty);
            }
        }

        private void InitializeDeleteOldResultFiles()
        {
            Regex deleteOldResultsRegex = this.GetArgumentRegex(DeleteOldFilesRegexPattern);
            Match currentMatch = deleteOldResultsRegex.Match(this.ConsoleArguments);

            if (!currentMatch.Success)
            {
                this.ShouldDeleteOldTestResultFiles = false;
            }
            else
            {
                this.ShouldDeleteOldTestResultFiles = bool.Parse(currentMatch.Groups["DeleteOldFilesValue"].Value);
                this.ConsoleArguments = this.ConsoleArguments.Replace(currentMatch.Groups["DeleteOldFilesArgument"].Value, string.Empty);
            }
        }

        private string InitializeInitialConsoleArguments(string[] arguments)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var argument in arguments)
            {
                string currentValueToBeAppended = argument;
                KeyValuePair<string, string> currentArgumentPair = this.SplitArgumentNameAndValue(argument);

                if (currentArgumentPair.Key != null && currentArgumentPair.Value.Contains(" "))
                {
                    currentValueToBeAppended = string.Concat("/", currentArgumentPair.Key, ":", "\"", currentArgumentPair.Value, "\"");
                }

                sb.AppendFormat("{0} ", currentValueToBeAppended);
            }
            return sb.ToString().TrimEnd();
        }

        private KeyValuePair<string, string> SplitArgumentNameAndValue(string argument)
        {
            KeyValuePair<string, string> argumentPair = new KeyValuePair<string, string>();
            Regex argumentPairRegex = new Regex(ArgumentRegexPattern, RegexOptions.Singleline);
            Match currentMatch = argumentPairRegex.Match(argument);

            if (currentMatch.Success)
            {
                argumentPair = new KeyValuePair<string, string>(currentMatch.Groups["ArgumentName"].Value, currentMatch.Groups["ArgumentValue"].Value);
            }

            return argumentPair;
        }

        private Regex GetArgumentRegex(string argumentPattern)
        {
            Regex argumentRegex = new Regex(argumentPattern, RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
            return argumentRegex;
        }
    }
}