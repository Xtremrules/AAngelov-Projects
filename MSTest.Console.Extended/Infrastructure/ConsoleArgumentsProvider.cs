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
        private const string ResultsFilePathRegexPattern = @".*/resultsfile:(?<ResultsFilePath>[0-9A-Za-z\\:._]{1,})";
        private const string NewResultsFilePathRegexPattern = @".*(?<NewResultsFilePathArgument>/newResultsfile:(?<NewResultsFilePath>[1-9A-Za-z\\:._]{1,}))";
        private const string RetriesCountRegexPattern = @".*(?<RetriesArgument>/retriesCount:(?<RetriesCount>[0-9]{1})).*";
        private const string FailedTestsThresholdRegexPattern = @".*(?<ThresholdArgument>/threshold:(?<ThresholdCount>[0-9]{1,2})).*";
        private const string DeleteOldResultsFilesRegexPattern = @".*(?<DeleteOldFilesArgument>/deleteOldResultsFiles:(?<DeleteOldFilesValue>[a-zA-Z]{4,5})).*";
        private const string ArgumentRegexPattern = @".*/(?<ArgumentName>[a-zA-Z]{1,}):(?<ArgumentValue>.*)";

        public ConsoleArgumentsProvider(string[] arguments)
        {
            this.StandardArguments = this.InitializeArguments(arguments);
            this.InitializeResultsFilePath();
            this.InitializeNewResultsFilePath();
            this.InitializeRetriesCount();
            this.InitializeFailedTestsThreshold();
            this.InitializeDeleteOldResultFiles();
        }

        public string StandardArguments
        {
            get;
            set;
        }

        public string ResultsFilePath
        {
            get;
            set;
        }

        public string NewResultsFilePath
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

        public bool ShouldDeleteOldResultsFiles
        {
            get;
            set;
        }

        private void InitializeResultsFilePath()
        {
            Regex testResultsPathRegex = this.GetArgumentRegex(ResultsFilePathRegexPattern);
            Match currentMatch = testResultsPathRegex.Match(this.StandardArguments);

            if (!currentMatch.Success)
            {
                throw new ArgumentException("You need to specify path to test results.");
            }

            this.ResultsFilePath = currentMatch.Groups["ResultsFilePath"].Value;
        }

        private void InitializeNewResultsFilePath()
        {
            Regex newTestResultsPathRegex = this.GetArgumentRegex(NewResultsFilePathRegexPattern);
            Match currentMatch = newTestResultsPathRegex.Match(this.StandardArguments);

            if (!currentMatch.Success)
            {
                this.NewResultsFilePath = this.ResultsFilePath;
            }
            else
            {
                this.NewResultsFilePath = currentMatch.Groups["NewResultsFilePath"].Value;
                this.StandardArguments = this.StandardArguments.Replace(currentMatch.Groups["NewResultsFilePathArgument"].Value, string.Empty);
            }
        }

        private void InitializeRetriesCount()
        {
            Regex retriesCountRegex = this.GetArgumentRegex(RetriesCountRegexPattern);
            Match currentMatch = retriesCountRegex.Match(this.StandardArguments);

            if (!currentMatch.Success)
            {
                this.RetriesCount = 0;
            }
            else
            {
                this.RetriesCount = int.Parse(currentMatch.Groups["RetriesCount"].Value);
                this.StandardArguments = this.StandardArguments.Replace(currentMatch.Groups["RetriesArgument"].Value, string.Empty);
            }
        }

        private void InitializeFailedTestsThreshold()
        {
            Regex failedTestsThresholdRegex = this.GetArgumentRegex(FailedTestsThresholdRegexPattern);
            Match currentMatch = failedTestsThresholdRegex.Match(this.StandardArguments);

            if (!currentMatch.Success)
            {
                this.FailedTestsThreshold = int.Parse(ConfigurationManager.AppSettings["ThresholdDefaultPercentage"]);
            }
            else
            {
                this.FailedTestsThreshold = int.Parse(currentMatch.Groups["ThresholdCount"].Value);
                this.StandardArguments = this.StandardArguments.Replace(currentMatch.Groups["ThresholdArgument"].Value, string.Empty);
            }
        }

        private void InitializeDeleteOldResultFiles()
        {
            Regex deleteOldResultsRegex = this.GetArgumentRegex(DeleteOldResultsFilesRegexPattern);
            Match currentMatch = deleteOldResultsRegex.Match(this.StandardArguments);

            if (!currentMatch.Success)
            {
                this.ShouldDeleteOldResultsFiles = false;
            }
            else
            {
                this.ShouldDeleteOldResultsFiles = bool.Parse(currentMatch.Groups["DeleteOldFilesValue"].Value);
                this.StandardArguments = this.StandardArguments.Replace(currentMatch.Groups["DeleteOldFilesArgument"].Value, string.Empty);
            }
        }

        private string InitializeArguments(string[] arguments)
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