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
            ota.AddToReadWrite(new List<string>() { "a", "b", "c" });
        }

        [TestMethod]
        public void TestDeleteCommandWhenNoItemsNoSelected()
        {
            var connection = new TestConnection();
            var otao = new TestOneTimeAccessOperator();
            var varinfomgr = new VariableInfo();
            var ota = new Jupiter.Models.OneTimeAccessModel(connection, otao, varinfomgr);
            ota.DeleteOneTimeAccessItemsCommand.Execute(null);
        }

        [TestMethod]
        public void TestDeleteCommandWhenOneRegisterdItemsNoSelected()
        {
            var connection = new TestConnection();
            var otao = new TestOneTimeAccessOperator();
            var varinfomgr = new VariableInfo();
            var ota = new Jupiter.Models.OneTimeAccessModel(connection, otao, varinfomgr);
            ota.DeleteOneTimeAccessItemsCommand.Execute(null);
        }
    }
}
