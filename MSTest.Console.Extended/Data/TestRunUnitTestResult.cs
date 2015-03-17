using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace MSTest.Console.Extended.Data
{
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010")]
    public partial class TestRunUnitTestResult
    {
        public TestRunUnitTestResultOutput Output
        {
            get;
            set;
        }

        [XmlAttributeAttribute("executionId")]
        public string ExecutionId
        {
            get;
            set;
        }

        [XmlAttributeAttribute("testId")]
        public string TestId
        {
            get;
            set;
        }

        [XmlAttributeAttribute("testName")]
        public string TestName
        {
            get;
            set;
        }

        [XmlAttributeAttribute("computerName")]
        public string ComputerName
        {
            get;
            set;
        }

        [XmlIgnore]
        [XmlAttributeAttribute("duration")]
        public TimeSpan Duartion
        {
            get;
            set;
        }

        // XmlSerializer does not support TimeSpan, so use this property for 
        // serialization instead.
        [Browsable(false)]
        [XmlAttributeAttribute(DataType = "duration", AttributeName = "duration")]
        public string DurationString
        {
            get
            {
                return this.Duartion.ToString();
            }
            set
            {
                this.Duartion = string.IsNullOrEmpty(value) ? TimeSpan.Zero : TimeSpan.Parse(value);
            }
        }

        [XmlAttributeAttribute("startTime")]
        public DateTime StartTime
        {
            get;
            set;
        }

        [XmlAttributeAttribute("endTime")]
        public DateTime EndTime
        {
            get;
            set;
        }

        [XmlAttributeAttribute("testType")]
        public string TestType
        {
            get;
            set;
        }

        [XmlAttributeAttribute("outcome")]
        public string Outcome
        {
            get;
            set;
        }

        [XmlAttributeAttribute("testListId")]
        public string TestListId
        {
            get;
            set;
        }

        [XmlAttributeAttribute("relativeResultsDirectory")]
        public string RelativeResultsDirectory
        {
            get;
            set;
        }
    }
}