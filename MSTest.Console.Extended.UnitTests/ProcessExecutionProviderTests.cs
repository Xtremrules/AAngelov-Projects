using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using log4net;
using MSTest.Console.Extended.Infrastructure;
using System.Threading.Tasks;
using System.Threading;
using MSTest.Console.Extended.Interfaces;

namespace MSTest.Console.Extended.UnitTests
{
    [TestClass]
    public class ProcessExecutionProviderTests
    {
        private const string IpConfigArgument = "ipconfig";
        private const string CmdFileName = "cmd.exe";

        [TestMethod]
        public void WaitForCurrentProcessExit_ShouldFinishCorrectly()
        {
            var log = Mock.Create<ILog>();
            Mock.Arrange(() => log.Info(Arg.AnyString));
            var processExecutionProvider = new ProcessExecutionProvider("cmd.exe", null, log);

            processExecutionProvider.Execute("ipconfig");

            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(200);
                processExecutionProvider.CurrentProcess.Kill();
            });

            processExecutionProvider.WaitForCurrentProcessExit();
            Assert.IsTrue(processExecutionProvider.CurrentProcess.HasExited);
        }

        [TestMethod]
        public void Execute_ShouldStartProcessWithAdditionalArguments_WhenAdditionalArgumentsProvided()
        {
            var log = Mock.Create<ILog>();
            Mock.Arrange(() => log.Info(Arg.AnyString));
            var processExecutionProvider = new ProcessExecutionProvider(CmdFileName, null, log);

            processExecutionProvider.Execute(IpConfigArgument);

            Assert.IsNotNull(processExecutionProvider.CurrentProcess);
            Assert.IsNotNull(processExecutionProvider.CurrentProcess.StartInfo);
            Assert.AreEqual(CmdFileName, processExecutionProvider.CurrentProcess.StartInfo.FileName);
            Assert.AreEqual(IpConfigArgument, processExecutionProvider.CurrentProcess.StartInfo.Arguments);

            processExecutionProvider.CurrentProcess.Kill();
        }

        [TestMethod]
        public void Execute_ShouldStartProcessOnlyWithStandardArguments_WhenNoAdditionalArgumentsSet()
        {
            var log = Mock.Create<ILog>();
            Mock.Arrange(() => log.Info(Arg.AnyString));

            var consoleArgumentsProvider = Mock.Create<IConsoleArgumentsProvider>();
            Mock.Arrange(() => consoleArgumentsProvider.StandardArguments).Returns(IpConfigArgument);
            var processExecutionProvider = new ProcessExecutionProvider(CmdFileName, consoleArgumentsProvider, log);

            processExecutionProvider.Execute();

            Assert.IsNotNull(processExecutionProvider.CurrentProcess);
            Assert.IsNotNull(processExecutionProvider.CurrentProcess.StartInfo);
            Assert.AreEqual(CmdFileName, processExecutionProvider.CurrentProcess.StartInfo.FileName);
            Assert.AreEqual(IpConfigArgument, processExecutionProvider.CurrentProcess.StartInfo.Arguments);
            processExecutionProvider.CurrentProcess.Kill();
        }
    }
}
