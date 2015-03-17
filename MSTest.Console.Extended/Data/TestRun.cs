using System.Xml.Serialization;

namespace MSTest.Console.Extended.Data
{
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010")]
    [XmlRootAttribute(Namespace = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010", IsNullable = false)]
    public partial class TestRun
    {
        public TestRunTestSettings TestSettings
        {
            get;
            set;
        }

        public TestRunTimes Times
        {
            get;
            set;
        }

        public TestRunResultSummary ResultSummary
        {
            get;
            set;
        }

        [XmlArrayItemAttribute("UnitTest", IsNullable = false)]
        public TestRunUnitTest[] TestDefinitions
        {
            get;
            set;
        }

        [XmlArrayItemAttribute("TestList", IsNullable = false)]
        public TestRunTestList[] TestLists
        {
            get;
            set;
        }

        [XmlArrayItemAttribute("TestEntry", IsNullable = false)]
        public TestRunTestEntry[] TestEntries
        {
            get;
            set;
        }

        [XmlArrayItemAttribute("UnitTestResult", IsNullable = false)]
        public TestRunUnitTestResult[] Results
        {
            get;
            set;
        }

        [XmlAttributeAttribute("id")]
        public string Id
        {
            get;
            set;
        }

        [XmlAttributeAttribute("name")]
        public string Name
        {
            get;
            set;
        }

        [XmlAttributeAttribute("runUser")]
        public string RunUser
        {
            get;
            set;
        }
    }
}