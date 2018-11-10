using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jupiter;
using Opc.Ua;
using System.Collections.ObjectModel;

namespace UnitTestJupiter
{
    public class TestNodeInfoGetter : Jupiter.Interfaces.INodeInfoGetter
    {
        public List<NodeInfo> GetNodeInfoList(ExpandedNodeId nodeid)
        {
            if(nodeid == new ExpandedNodeId("1"))
            {
                return new List<NodeInfo>()
                {
                    new NodeInfo() { Name = "変数1", Value = "1234" },
                    new NodeInfo() { Name = "変数2", Value = "1234567890" },
                    new NodeInfo() { Name = "変数3", Value = "-1234.5678" },
                    new NodeInfo() { Name = "変数4", Value = "abcdefghij" },
                    new NodeInfo() { Name = "変数5", Value = "あいうえお" },
                };
            }

            return new List<NodeInfo>()
                {
                    new NodeInfo() { Name = "Var1", Value = "1234" },
                    new NodeInfo() { Name = "Var2", Value = "1234567890" },
                    new NodeInfo() { Name = "Var3", Value = "-1234.5678" },
                    new NodeInfo() { Name = "Var4", Value = "abcdefghij" },
                };
        }
    }

    /// <summary>
    /// NodeInfoDataGridModelTest の概要の説明
    /// </summary>
    [TestClass]
    public class NodeInfoDataGridModelTest
    {
        public NodeInfoDataGridModelTest()
        {
            //
            // TODO: コンストラクター ロジックをここに追加します
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///現在のテストの実行についての情報および機能を
        ///提供するテスト コンテキストを取得または設定します。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 追加のテスト属性
        //
        // テストを作成する際には、次の追加属性を使用できます:
        //
        // クラス内で最初のテストを実行する前に、ClassInitialize を使用してコードを実行してください
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // クラス内のテストをすべて実行したら、ClassCleanup を使用してコードを実行してください
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // 各テストを実行する前に、TestInitialize を使用してコードを実行してください
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // 各テストを実行した後に、TestCleanup を使用してコードを実行してください
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void CheckNodeInfoListAccordingToConnectionStatus()
        {
            var connection = new TestConnection();
            var nodeinfogetter = new TestNodeInfoGetter();
            var model = new Jupiter.Models.NodeInfoDataGridModel(connection, nodeinfogetter);
            var reference = new OPCUAReference(null, null, null);
            model.Update(reference);
            var expectedNodeInfoList0 = new ObservableCollection<NodeInfo>() {
                new NodeInfo() { Name = "Var1", Value = "1234" },
                new NodeInfo() { Name = "Var2", Value = "1234567890" },
                new NodeInfo() { Name = "Var3", Value = "-1234.5678" },
                new NodeInfo() { Name = "Var4", Value = "abcdefghij" },
             };
            Assert.AreEqual(model.NodeInfoList.Count, 4);
            for (int i = 0; i < 4; i++)
            {
                var actual = model.NodeInfoList[i];
                var expect = expectedNodeInfoList0[i];
                Assert.AreEqual(actual.Name, expect.Name);
                Assert.AreEqual(actual.Value, expect.Value);
            }
            connection.Connected = true;
            connection.Connected = false;
            Assert.AreEqual(model.NodeInfoList.Count, 0);
        }

        [TestMethod]
        public void CheckNodeInfoListAfterExecutingUpdate()
        {
            var connection = new TestConnection();
            var nodeinfogetter = new TestNodeInfoGetter();
            var model = new Jupiter.Models.NodeInfoDataGridModel(connection, nodeinfogetter);
            var reference = new OPCUAReference(null, null, null);
            model.Update(reference);
            var expectedNodeInfoList0 = new ObservableCollection<NodeInfo>() {
                new NodeInfo() { Name = "Var1", Value = "1234" },
                new NodeInfo() { Name = "Var2", Value = "1234567890" },
                new NodeInfo() { Name = "Var3", Value = "-1234.5678" },
                new NodeInfo() { Name = "Var4", Value = "abcdefghij" },
             };
            Assert.AreEqual(model.NodeInfoList.Count, 4);
            for(int i = 0; i < 4; i++)
            {
                var actual = model.NodeInfoList[i];
                var expect = expectedNodeInfoList0[i];
                Assert.AreEqual(actual.Name, expect.Name);
                Assert.AreEqual(actual.Value, expect.Value);
            }

            reference.NodeId = new ExpandedNodeId("1");
            model.Update(reference);
            var expectedNodeInfoList1 = new ObservableCollection<NodeInfo>() {
                new NodeInfo() { Name = "変数1", Value = "1234" },
                new NodeInfo() { Name = "変数2", Value = "1234567890" },
                new NodeInfo() { Name = "変数3", Value = "-1234.5678" },
                new NodeInfo() { Name = "変数4", Value = "abcdefghij" },
                new NodeInfo() { Name = "変数5", Value = "あいうえお" },
             };
            Assert.AreEqual(model.NodeInfoList.Count, 5);
            for (int i = 0; i < 4; i++)
            {
                var actual = model.NodeInfoList[i];
                var expect = expectedNodeInfoList1[i];
                Assert.AreEqual(actual.Name, expect.Name);
                Assert.AreEqual(actual.Value, expect.Value);
            }
        }
    }
}
