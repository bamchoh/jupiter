using System;
using System.Collections;
using System.Collections.Generic;
using Jupiter;
using Jupiter.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Opc.Ua;
using System.Collections.ObjectModel;
using Prism.Events;

namespace UnitTestJupiter
{
    public class TestOneTimeAccessOperator : IOneTimeAccessOperator
    {
        public ResponseHeader Read(ReadValueIdCollection itemsToRead, out DataValueCollection values, out DiagnosticInfoCollection diagnosticInfos)
        {
            values = new DataValueCollection();
            diagnosticInfos = new DiagnosticInfoCollection();

            foreach(var item in itemsToRead)
            {
                values.Add(new DataValue(12345, new StatusCode(0)));
            }

            return new ResponseHeader();
        }

        public void Write(IList<VariableInfoBase> items)
        {
            throw new NotImplementedException();
        }
    }

    public class TestVariableConfiguration : Jupiter.Interfaces.IVariableConfiguration
    {
        public NodeClass NodeClass { get; set; }

        public BuiltInType TestBuiltInType { get; set; }

        public string DisplayName { get; set; }

        public NodeId TestVariableNodeId { get; set; }

        public object Value { get; set; }

        public BuiltInType BuiltInType()
        {
            return TestBuiltInType;
        }

        public NodeId VariableNodeId()
        {
            return TestVariableNodeId;
        }
    }

    public class TestPattern
    {
        public string Name { get; set; }
        public BuiltInType Type { get; set; }
        public object Value { get; set; }
    }

    [TestClass]
    public class OneTimeAccessModelTest
    {
        [TestMethod]
        public void TestAddCommand()
        {
            var connection = new TestConnection();
            var otao = new TestOneTimeAccessOperator();
            var varinfomgr = new VariableInfo();
            var ota = new Jupiter.Models.OneTimeAccessModel(connection, otao, varinfomgr);
            Add(ota, "Var1", 12345, BuiltInType.Integer);
            Assert.AreEqual(ota.OneTimeAccessItems.Count, 1);
        }

        [TestMethod]
        public void TestConnect()
        {
            var connection = new TestConnection();
            var otao = new TestOneTimeAccessOperator();
            var varinfomgr = new VariableInfo();
            var ota = new Jupiter.Models.OneTimeAccessModel(connection, otao, varinfomgr);
            Add(ota, "Var1", 12345, BuiltInType.Integer);
            connection.Connected = true;
            Assert.AreEqual(ota.OneTimeAccessItems.Count, 1);
            connection.Connected = false;
            Assert.AreEqual(ota.OneTimeAccessItems.Count, 0);
        }

        [TestMethod]
        public void TestStateOfDeleteCommand()
        {
            var connection = new TestConnection();
            var otao = new TestOneTimeAccessOperator();
            var varinfomgr = new VariableInfo();
            var ota = new Jupiter.Models.OneTimeAccessModel(connection, otao, varinfomgr);
            bool flag;
            flag = ota.DeleteOneTimeAccessItemsCommand.CanExecute(null);
            Assert.AreEqual(flag, false);
            connection.Connected = true;
            flag = ota.DeleteOneTimeAccessItemsCommand.CanExecute(null);
            Assert.AreEqual(flag, false);
            Add(ota, "Var1", 12345, BuiltInType.Integer);
            flag = ota.DeleteOneTimeAccessItemsCommand.CanExecute(null);
            Assert.AreEqual(flag, true);
        }

        [TestMethod]
        public void TestStateOfReadCommand()
        {
            var connection = new TestConnection();
            var otao = new TestOneTimeAccessOperator();
            var varinfomgr = new VariableInfo();
            var ota = new Jupiter.Models.OneTimeAccessModel(connection, otao, varinfomgr);
            bool flag;
            flag = ota.ReadCommand.CanExecute(null);
            Assert.AreEqual(flag, false);
            connection.Connected = true;
            flag = ota.ReadCommand.CanExecute(null);
            Assert.AreEqual(flag, false);
            Add(ota, "Var1", 12345, BuiltInType.Integer);
            flag = ota.ReadCommand.CanExecute(null);
            Assert.AreEqual(flag, true);
        }

        [TestMethod]
        public void TestStateOfWriteCommand()
        {
            var connection = new TestConnection();
            var otao = new TestOneTimeAccessOperator();
            var varinfomgr = new VariableInfo();
            var ota = new Jupiter.Models.OneTimeAccessModel(connection, otao, varinfomgr);
            bool flag;
            flag = ota.WriteCommand.CanExecute(null);
            Assert.AreEqual(flag, false);
            connection.Connected = true;
            flag = ota.WriteCommand.CanExecute(null);
            Assert.AreEqual(flag, false);
            Add(ota, "Var1", 12345, BuiltInType.Integer);
            flag = ota.ReadCommand.CanExecute(null);
            Assert.AreEqual(flag, true);
        }

        [TestMethod]
        public void TestDeleteCommandWhen0RegisteredItems0Selected()
        {
            var connection = new TestConnection();
            var otao = new TestOneTimeAccessOperator();
            var varinfomgr = new VariableInfo();
            var ota = new Jupiter.Models.OneTimeAccessModel(connection, otao, varinfomgr);
            ota.DeleteOneTimeAccessItemsCommand.Execute(null);
            Assert.AreEqual(ota.OneTimeAccessItems.Count, 0);
        }

        [TestMethod]
        public void TestDeleteCommandWhen1RegisteredItems0Selected()
        {
            var connection = new TestConnection();
            var otao = new TestOneTimeAccessOperator();
            var varinfomgr = new VariableInfo();
            var ota = new Jupiter.Models.OneTimeAccessModel(connection, otao, varinfomgr);
            Add(ota, "Var1", 12345, BuiltInType.Integer);
            ((VariableInfoBase)ota.OneTimeAccessItems[0]).PropertyChanged += (object sender, System.ComponentModel.PropertyChangedEventArgs e) => {
                ota.OneTimeAccessSelectedItems = new ObservableCollection<VariableInfoBase>();
                ota.OneTimeAccessSelectedItems.Add(sender);
            };
            ota.DeleteOneTimeAccessItemsCommand.Execute(null);
            Assert.AreEqual(ota.OneTimeAccessItems.Count, 0);
        }

        [TestMethod]
        public void TestDeleteCommandWhen1RegisteredItems1Selected()
        {
            var connection = new TestConnection();
            var otao = new TestOneTimeAccessOperator();
            var varinfomgr = new VariableInfo();
            var ota = new Jupiter.Models.OneTimeAccessModel(connection, otao, varinfomgr);
            Add(ota, "Var1", 12345, BuiltInType.Integer);
            ((VariableInfoBase)ota.OneTimeAccessItems[0]).PropertyChanged += (object sender, System.ComponentModel.PropertyChangedEventArgs e) => {
                ota.OneTimeAccessSelectedItems = new ObservableCollection<VariableInfoBase>();
                ota.OneTimeAccessSelectedItems.Add(sender);
            };
            ((VariableInfoBase)ota.OneTimeAccessItems[0]).IsSelected = true;
            ota.DeleteOneTimeAccessItemsCommand.Execute(null);
            Assert.AreEqual(ota.OneTimeAccessItems.Count, 0);
        }

        [TestMethod]
        public void TestDeleteCommandWhen2RegisteredItems1Selected()
        {
            var connection = new TestConnection();
            var otao = new TestOneTimeAccessOperator();
            var varinfomgr = new VariableInfo();
            var ota = new Jupiter.Models.OneTimeAccessModel(connection, otao, varinfomgr);
            Add(ota, "Var1", 12345, BuiltInType.Integer);
            Add(ota, "Var2", 23456, BuiltInType.Integer);
            ota.OneTimeAccessSelectedItems = new ObservableCollection<VariableInfoBase>();
            foreach (VariableInfoBase vib in ota.OneTimeAccessItems)
            {
                vib.PropertyChanged += (object sender, System.ComponentModel.PropertyChangedEventArgs e) => {
                    ota.OneTimeAccessSelectedItems.Add(sender);
                };
            }
            ((VariableInfoBase)ota.OneTimeAccessItems[0]).IsSelected = true;
            ota.DeleteOneTimeAccessItemsCommand.Execute(null);
            Assert.AreEqual(ota.OneTimeAccessItems.Count, 1);
            Assert.AreEqual(((VariableInfoBase)ota.OneTimeAccessItems[0]).NodeId, "Var2");
        }

        private Dictionary<string, BuiltInType> _readCommandBase_CreateTestVariables(List<TestPattern> testPatterns)
        {
            var variables = new Dictionary<string, BuiltInType>();
            foreach (var p in testPatterns)
            {
                variables[p.Name] = p.Type;
            }
            return variables;
        }

        private Opc.Ua.Configuration.ApplicationInstance _readCommandBase_CreateApplication(string endpoint)
        {
            var application = TestServer.Application.GetApplicationInstance();
            {
                var ba = application.ApplicationConfiguration.ServerConfiguration.BaseAddresses;
                ba.Add(endpoint);
            }
            return application;
        }

        private void _readCommandBase_ServerInit(TestServer.TestServer s, List<TestPattern> testPatterns)
        {
            foreach (var p in testPatterns)
            {
                s.SetValue(p.Name, p.Value);
            }
        }

        private void ReadCommandBase(string endpoint, List<TestPattern> testPatterns)
        {
            var variables = _readCommandBase_CreateTestVariables(testPatterns);

            var application = _readCommandBase_CreateApplication(endpoint);

            using (var s = TestServer.TestServer.StartServer(application, variables))
            {
                _readCommandBase_ServerInit(s, testPatterns);

                var ea = new Prism.Events.EventAggregator();
                var msg = "";

                ea.GetEvent<Jupiter.Events.ErrorNotificationEvent>()
                    .Subscribe(x => msg = x.Message);

                var varinfomgr = new VariableInfo();
                var c = new Client(varinfomgr, ea);
                var references = new OPCUAReference(c, null, ea);
                var ota = new Jupiter.Models.OneTimeAccessModel(c, c, varinfomgr);
                ota.EventAggregator = ea;
                var nodegrid = new Jupiter.Models.NodeInfoDataGridModel(c, c);
                var nodetree = new Jupiter.Models.NodeTreeModel(c, references, null, ota);
                nodetree.EventAggregator = ea;

                c.EventAggregator
                    .GetEvent<Jupiter.Events.NowLoadingEvent>()
                    .Subscribe((x) =>
                    {
                        for(int i=0;i<x.SecurityList.Count;i++)
                        {
                            if(x.SecurityList[i].EndsWith("None"))
                            {
                                Console.WriteLine(x.SecurityList[i]);
                                x.SelectedIndex = i;
                                break;
                            }
                        }
                    });

                c.CreateSession(endpoint).Wait();
                foreach (OPCUAReference ch in references.Children)
                {
                    if (ch.DisplayName != "TestData")
                        continue;

                    nodetree.UpdateVariableNodeListCommand.Execute(ch);
                    foreach (VariableConfiguration vn in nodetree.VariableNodes)
                    {
                        nodetree.AddToReadWriteCommand.Execute(new List<VariableConfiguration>() { vn });
                    }
                };
                ota.ReadCommand.Execute(null);
                Assert.AreEqual(msg, "");
                Assert.AreEqual(ota.OneTimeAccessItems.Count, testPatterns.Count);
                for (int i = 0; i < testPatterns.Count; i++)
                {
                    var got = ((VariableInfoBase)ota.OneTimeAccessItems[i]).DataValue.Value.ToString();
                    var want = testPatterns[i].Value.ToString();
                    Assert.AreEqual(got, want);
                }
            }
        }

        [TestMethod]
        public void TestReadCommand1()
        {
            var endpoint = "opc.tcp://localhost:62548";

            var testPattern = new List<TestPattern>()
            {
                new TestPattern { Name = "Var1", Type = BuiltInType.UInt32, Value = 1234},
            };

            ReadCommandBase(endpoint, testPattern);
        }

        [TestMethod]
        public void TestReadCommand2()
        {
            var endpoint = "opc.tcp://localhost:62548";

            var testPattern = new List<TestPattern>();

            for(int i = 0; i < 1000; i++)
            {
                var tp = new TestPattern { Name = "Var"+i.ToString(), Type = BuiltInType.UInt32, Value = i };
                testPattern.Add(tp);
            };

            ReadCommandBase(endpoint, testPattern);
        }

        private void Add(Jupiter.Models.OneTimeAccessModel ota, string name, object value, BuiltInType type)
        {
            ota.AddToReadWrite(new List<IVariableConfiguration>() {
                new TestVariableConfiguration() {
                    NodeClass = NodeClass.Variable,
                    TestBuiltInType = type,
                    TestVariableNodeId = new NodeId(name),
                    Value = value,
                }
            });
        }
    }
}
