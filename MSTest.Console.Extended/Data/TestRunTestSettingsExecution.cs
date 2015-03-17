using System.Xml.Serialization;

namespace MSTest.Console.Extended.Data
{
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010")]
    public partial class TestRunTestSettingsExecution
    {
        public object TestTypeSpecific
        {
            get;
            set;
        }

        public TestRunTestSettingsExecutionAgentRule AgentRule
        {
            get;
            set;
        }
    }
}