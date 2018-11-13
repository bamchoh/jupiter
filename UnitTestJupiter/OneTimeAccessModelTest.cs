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

    public class TestReference : Jupiter.Interfaces.IVariableConfiguration
    {
        public NodeClass Type { get; set; }

        public BuiltInType TestBuiltInType { get; set; }

        public NodeId TestVariableNodeId { get; set; }

        public BuiltInType BuiltInType()
        {
            return TestBuiltInType;
        }

        public NodeId VariableNodeId()
        {
            return TestVariableNodeId;
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

        private void Add(Jupiter.Models.OneTimeAccessModel ota, string name, Variant value, BuiltInType type)
        {
            ota.AddToReadWrite(new List<Jupiter.Interfaces.IVariableConfiguration>() {
                new TestReference() {
                    Type = NodeClass.Variable,
                    TestBuiltInType = type,
                    TestVariableNodeId = new NodeId(name),
                }
            });
        }
    }
}
