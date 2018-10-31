using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jupiter;

namespace UnitTestJupiter
{
    /// <summary>
    /// UnitTest2 の概要の説明
    /// </summary>
    [TestClass]
    public class ConnectionModelTest
    {
        public ConnectionModelTest()
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
        public void ConnectStatusCheck()
        {
            var connection = new TestConnection();
            var model = new Jupiter.Models.ConnectionModel(connection);
            model.ConnectCommand.Execute(null);
            Assert.AreEqual(connection.Connected, true);
            Assert.AreEqual(model.ConnectButtonContent, "Disconnect");
            model.ConnectCommand.Execute(null);
            Assert.AreEqual(connection.Connected, false);
            Assert.AreEqual(model.ConnectButtonContent, "Connect");
        }

        [TestMethod]
        public void CreateSessionExceptionCheck()
        {
            var connection = new TestConnection();
            connection.Exception = true;
            var model = new Jupiter.Models.ConnectionModel(connection);
            model.ConnectCommand.Execute(null);
        }
    }
}
