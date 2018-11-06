using System;
using System.Collections;
using System.Collections.Generic;
using Jupiter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Opc.Ua;

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

    public class TestVariableInfoManager : Jupiter.Interfaces.IVariableInfoManager
    {
        public IList<VariableInfoBase> NewVariableInfo(IList objs)
        {
            return new List<VariableInfoBase>()
            {
                new BooleanVariableInfo(),
                new SByteVariableInfo(),
                new Int16VariableInfo()
            };
        }

        public VariableInfoBase NewVariableInfo(ExpandedNodeId id)
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
            var varinfomgr = new TestVariableInfoManager();
            var ota = new Jupiter.Models.OneTimeAccessModel(connection, otao, varinfomgr);
            ota.AddToReadWrite(new List<string>() { "a", "b", "c" });
        }

        [TestMethod]
        public void TestDeleteCommandWhenNoItemsNoSelected()
        {
            var connection = new TestConnection();
            var otao = new TestOneTimeAccessOperator();
            var varinfomgr = new TestVariableInfoManager();
            var ota = new Jupiter.Models.OneTimeAccessModel(connection, otao, varinfomgr);
            ota.DeleteOneTimeAccessItemsCommand.Execute(null);
        }

        [TestMethod]
        public void TestDeleteCommandWhenOneRegisterdItemsNoSelected()
        {
            var connection = new TestConnection();
            var otao = new TestOneTimeAccessOperator();
            var varinfomgr = new TestVariableInfoManager();
            var ota = new Jupiter.Models.OneTimeAccessModel(connection, otao, varinfomgr);
            ota.DeleteOneTimeAccessItemsCommand.Execute(null);
        }
    }
}
