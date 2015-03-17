using System.Xml.Serialization;

namespace MSTest.Console.Extended.Data
{
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010")]
    public partial class TestRunResultSummary
    {
        public TestRunResultSummaryCounters Counters
        {
            get;
            set;
        }

        public TestRunResultSummaryOutput Output
        {
            get;
            set;
        }

        public TestRunResultSummaryRunInfos RunInfos
        {
            get;
            set;
        }

        [XmlAttributeAttribute("outcome")]
        public string Outcome
        {
            get;
            set;
        }
    }
}