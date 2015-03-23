using System.Xml.Serialization;

namespace MSTest.Console.Extended.Data
{
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010")]
    public partial class TestRunUnitTest
    {
        public TestRunUnitTestOwners Owners
        {
            get;
            set;
        }

        public TestRunUnitTestTestCategory TestCategory
        {
            get;
            set;
        }

        [XmlArrayItemAttribute("DeploymentItem")]
        public TestRunUnitTestDeploymentItem[] DeploymentItems
        {
            get;
            set;
        }

        public TestRunUnitTestDataSource DataSource
        {
            get;
            set;
        }

        public TestRunUnitTestExecution Execution
        {
            get;
            set;
        }

        public TestRunUnitTestTestMethod TestMethod
        {
            get;
            set;
        }

        public string Extension
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

        [XmlAttributeAttribute("storage")]
        public string Storage
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