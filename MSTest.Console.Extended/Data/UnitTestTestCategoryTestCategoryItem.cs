using System.Xml.Serialization;

namespace MSTest.Console.Extended.Data
{
    [XmlTypeAttribute(AnonymousType = true)]
    public partial class UnitTestTestCategoryTestCategoryItem
    {

        [XmlAttributeAttribute()]
        public string TestCategory
        {
            get;
            set;
        }
    }
}