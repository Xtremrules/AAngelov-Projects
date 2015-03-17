using System.Xml.Serialization;

namespace MSTest.Console.Extended.Data
{
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010")]
    public partial class TestRunResultSummaryRunInfosRunInfo
    {
        public string Text
        {
            get;
            set;
        }

        [XmlAttributeAttribute("computerName")]
        public string ComputerName
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

        [XmlAttributeAttribute("timestamp")]
        public System.DateTime Timestamp
        {
            get;
            set;
        }
    }
}