namespace MSTest.Console.Extended.Interfaces
{
    public interface IConsoleArgumentsProvider
    {
        string StandardArguments { get; set; }
        
        string ResultsFilePath { get; set; }

        string NewResultsFilePath { get; set; }

        int RetriesCount { get; set; }

        bool ShouldDeleteOldResultsFiles { get; set; }
    
        int FailedTestsThreshold { get; set; }
    }
}