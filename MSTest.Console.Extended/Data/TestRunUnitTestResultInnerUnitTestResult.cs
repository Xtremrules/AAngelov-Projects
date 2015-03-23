using System;
using System.Xml.Serialization;

namespace MSTest.Console.Extended.Data
{
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010")]
    public class TestRunUnitTestResultInnerUnitTestResult : TestRunUnitTestResultBase
    {
        [XmlAttributeAttribute("parentExecutionId")]
        public string ParentExecutionId
        {
            get;
            set;
        }

        [XmlAttributeAttribute("dataRowInfo")]
        public string DataRowInfo
        {
            get;
            set;
        }
    }
}
