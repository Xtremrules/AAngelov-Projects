using System.Xml.Serialization;

namespace MSTest.Console.Extended.Data
{
    [XmlTypeAttribute(AnonymousType = true)]
    public partial class TestRunUnitTestOwners
    {
        public UnitTestOwnersOwner Owner
        {
            get;
            set;
        }
    }
}