﻿using System;
using System.Linq;
using System.Xml.Serialization;

namespace MSTest.Console.Extended.Data
{
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010")]
    public partial class TestRunResultSummaryRunInfos
    {
        public TestRunResultSummaryRunInfosRunInfo RunInfo
        {
            get;
            set;
        }
    }
}