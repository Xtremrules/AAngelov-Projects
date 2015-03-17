using System.Xml.Serialization;

namespace MSTest.Console.Extended.Data
{
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010")]
    public partial class TestRunTestSettings
    {
        public TestRunTestSettingsExecution Execution
        {
            get;
            set;
        }

        public TestRunTestSettingsDeployment Deployment
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

        [XmlAttributeAttribute("id")]
        public string Id
        {
            get;
            set;
        }
    }
}