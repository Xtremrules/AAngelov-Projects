using System.Xml.Serialization;

namespace MSTest.Console.Extended.Data
{
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010")]
    public partial class TestRunUnitTestTestMethod
    {
        [XmlAttributeAttribute("codeBase")]
        public string CodeBase
        {
            get;
            set;
        }

        [XmlAttributeAttribute("adapterTypeName")]
        public string AdapterTypeName
        {
            get;
            set;
        }

        [XmlAttributeAttribute("className")]
        public string ClassName
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
    }
}