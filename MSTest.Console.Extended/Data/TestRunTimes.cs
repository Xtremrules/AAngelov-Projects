using System;
using System.Xml.Serialization;

namespace MSTest.Console.Extended.Data
{
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010")]
    public partial class TestRunTimes
    {
        [XmlAttributeAttribute("creation")]
        public DateTime Creation
        {
            get;
            set;
        }

        [XmlAttributeAttribute("queuing")]
        public DateTime Queuing
        {
            get;
            set;
        }

        [XmlAttributeAttribute("start")]
        public DateTime Start
        {
            get;
            set;
        }

        [XmlAttributeAttribute("finish")]
        public DateTime Finish
        {
            get;
            set;
        }
    }
}