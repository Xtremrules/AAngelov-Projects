using System.Xml.Serialization;

namespace MSTest.Console.Extended.Data
{
    [XmlTypeAttribute(AnonymousType = true)]
    public partial class UnitTestOwnersOwner
    {
        [XmlAttributeAttribute("name")]
        public string Name
        {
            get;
            set;
        }
    }
}