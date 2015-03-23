using System;
using System.Xml.Serialization;

namespace MSTest.Console.Extended.Data
{
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010")]
    public class TestRunUnitTestResultResultFile
    {
        [XmlAttributeAttribute("path")]
        public string Path
        {
            get;
            set;
        }
    }
}
