using System.Xml.Serialization;

namespace MSTest.Console.Extended.Data
{
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010")]
    public partial class TestRunTestSettingsDeployment
    {
        [XmlAttributeAttribute("userDeploymentRoot")]
        public string UserDeploymentRoot
        {
            get;
            set;
        }

        [XmlAttributeAttribute("useDefaultDeploymentRoot")]
        public bool UseDefaultDeploymentRoot
        {
            get;
            set;
        }

        [XmlAttributeAttribute("runDeploymentRoot")]
        public string RunDeploymentRoot
        {
            get;
            set;
        }
    }
}