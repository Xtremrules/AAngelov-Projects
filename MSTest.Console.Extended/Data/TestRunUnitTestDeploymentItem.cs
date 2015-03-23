using System;
using System.Xml.Serialization;

namespace MSTest.Console.Extended.Data
{
    [XmlTypeAttribute(AnonymousType = true)]
    public class TestRunUnitTestDeploymentItem
    {
        [XmlAttributeAttribute("filename")]
        public string FileName
        {
            get;
            set;
        }

        [XmlAttributeAttribute("outputDirectory")]
        public string OutputDirectory
        {
            get;
            set;
        }
    }
}
