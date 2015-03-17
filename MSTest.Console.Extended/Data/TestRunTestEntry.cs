using System.Xml.Serialization;

namespace MSTest.Console.Extended.Data
{
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010")]
    public partial class TestRunTestEntry
    {
        [XmlAttributeAttribute("testId")]
        public string TestId
        {
            get;
            set;
        }

        [XmlAttributeAttribute("executionId")]
        public string ExecutionId
        {
            get;
            set;
        }

        [XmlAttributeAttribute("testListId")]
        public string TestListId
        {
            get;
            set;
        }
    }
}