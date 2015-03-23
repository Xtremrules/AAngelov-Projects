using System;
using System.Xml.Serialization;

namespace MSTest.Console.Extended.Data
{
    [XmlTypeAttribute(AnonymousType = true)]
    public class TestRunUnitTestDataSource
    {
        [XmlAttributeAttribute("connectionString")]
        public string ConnectionString
        {
            get;
            set;
        }

        [XmlAttributeAttribute("providerInvariantName")]
        public string ProviderInvariantName
        {
            get;
            set;
        }

        [XmlAttributeAttribute("tableName")]
        public string TableName
        {
            get;
            set;
        }
    }
}
