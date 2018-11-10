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
    public class TestOneTimeAccessOperator : Jupiter.Interfaces.IOneTimeAccessOperator
    {
        public void Read(IList<VariableInfoBase> items)
        {
            throw new NotImplementedException();
        }

        public void Write(IList<VariableInfoBase> items)
        {
            throw new NotImplementedException();
        }
    }

    public class TestReference : Jupiter.Interfaces.IReference
    {
        public Jupiter.Interfaces.IReference Parent => throw new NotImplementedException();

        public IList Children => throw new NotImplementedException();

        public ExpandedNodeId NodeId { get; set; }
        public NodeClass Type { get; set; }

        public INode Node { get; set; }

        public ITypeTable TypeTable {get;set;}

        public ReferenceDescriptionCollection FetchVariableReferences()
        {
            throw new NotImplementedException();
        }

        public IEventAggregator GetEventAggregator()
        {
            throw new NotImplementedException();
        }

        public Jupiter.Interfaces.IReference NewReference(string name)
        {
            throw new NotImplementedException();
        }

        public void UpdateReferences()
        {
            throw new NotImplementedException();
        }
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
            Add(ota, "Var1", 12345);
            Assert.AreEqual(ota.OneTimeAccessItems.Count, 1);
        }

        [TestMethod]
        public void TestConnect()
        {
            var connection = new TestConnection();
            var otao = new TestOneTimeAccessOperator();
            var varinfomgr = new VariableInfo();
            var ota = new Jupiter.Models.OneTimeAccessModel(connection, otao, varinfomgr);
            Add(ota, "Var1", 12345);
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
            Add(ota, "Var1", 12345);
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
            Add(ota, "Var1", 12345);
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
            Add(ota, "Var1", 12345);
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
            Add(ota, "Var1", 12345);
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
            Add(ota, "Var1", 12345);
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
            Add(ota, "Var1", 12345);
            Add(ota, "Var2", 23456);
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
            Assert.AreEqual(((VariableInfoBase)ota.OneTimeAccessItems[0]).NodeId, "Var1");
        }

        private void Add(Jupiter.Models.OneTimeAccessModel ota, string name, Variant value)
        {
            ota.AddToReadWrite(new List<Jupiter.Interfaces.IReference>() { new TestReference() {
                NodeId = new ExpandedNodeId(name),
                Type = NodeClass.Variable,
                Node = new VariableNode() {
                    DisplayName = name, Value = value
                },
            } });
        }
    }
}
