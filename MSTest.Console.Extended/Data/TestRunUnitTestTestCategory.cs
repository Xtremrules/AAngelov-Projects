using System.Xml.Serialization;

namespace MSTest.Console.Extended.Data
{
    [XmlTypeAttribute(AnonymousType = true)]
    public partial class TestRunUnitTestTestCategory
    {
        public UnitTestTestCategoryTestCategoryItem TestCategoryItem
        {
            get;
            set;
        }
    }
}