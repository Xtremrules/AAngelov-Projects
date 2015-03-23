using System;
using System.Xml.Serialization;

namespace MSTest.Console.Extended.Data
{
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010")]
    public partial class TestRunUnitTestResult : TestRunUnitTestResultBase
    {
        public TestRunResultSummaryCounters Counters
        {
            get;
            set;
        }

        [XmlArrayItemAttribute("ResultFile")]
        public TestRunUnitTestResultResultFile[] ResultFiles
        {
            get;
            set;
        }

        [XmlArrayItemAttribute("UnitTestResult")]
        public TestRunUnitTestResultInnerUnitTestResult[] InnerResults
        {
            get;
            set;
        }
    }
}