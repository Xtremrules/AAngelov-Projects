using System.Xml.Serialization;

namespace MSTest.Console.Extended.Data
{
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010")]
    public partial class TestRunResultSummaryCounters
    {
        [XmlAttributeAttribute("total")]
        public byte Total
        {
            get;
            set;
        }

        [XmlAttributeAttribute("executed")]
        public byte Executed
        {
            get;
            set;
        }

        [XmlAttributeAttribute("passed")]
        public byte Passed
        {
            get;
            set;
        }

        [XmlAttributeAttribute("error")]
        public byte Error
        {
            get;
            set;
        }

        [XmlAttributeAttribute("failed")]
        public byte Failed
        {
            get;
            set;
        }

        [XmlAttributeAttribute("timeout")]
        public byte Timeout
        {
            get;
            set;
        }

        [XmlAttributeAttribute("aborted")]
        public byte Aborted
        {
            get;
            set;
        }

        [XmlAttributeAttribute("inconclusive")]
        public byte Inconclusive
        {
            get;
            set;
        }

        [XmlAttributeAttribute("passedButRunAborted")]
        public byte PassedButRunAborted
        {
            get;
            set;
        }

        [XmlAttributeAttribute("notRunnable")]
        public byte NotRunnable
        {
            get;
            set;
        }

        [XmlAttributeAttribute("notExecuted")]
        public byte NotExecuted
        {
            get;
            set;
        }

        [XmlAttributeAttribute("disconnected")]
        public byte Disconnected
        {
            get;
            set;
        }

        [XmlAttributeAttribute("warning")]
        public byte Warning
        {
            get;
            set;
        }

        [XmlAttributeAttribute("completed")]
        public byte Completed
        {
            get;
            set;
        }

        [XmlAttributeAttribute("inProgress")]
        public byte InProgress
        {
            get;
            set;
        }

        [XmlAttributeAttribute("pending")]
        public byte Pending
        {
            get;
            set;
        }
    }
}