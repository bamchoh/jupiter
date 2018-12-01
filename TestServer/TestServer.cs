/* ========================================================================
 * Copyright (c) 2005-2017 The OPC Foundation, Inc. All rights reserved.
 *
 * OPC Foundation MIT License 1.00
 * 
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 * 
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 *
 * The complete license agreement can be found here:
 * http://opcfoundation.org/License/MIT/1.00/
 * ======================================================================*/

using System;
using System.Collections.Generic;
using System.Text;
using Opc.Ua;
using Opc.Ua.Server;
using System.Linq;
using System.Threading.Tasks;
using Opc.Ua.Configuration;
using System.IO;

namespace TestServer
{
    /// <summary>
    /// Implements a basic Quickstart Server.
    /// </summary>
    /// <remarks>
    /// Each server instance must have one instance of a StandardServer object which is
    /// responsible for reading the configuration file, creating the endpoints and dispatching
    /// incoming requests to the appropriate handler.
    /// 
    /// This sub-class specifies non-configurable metadata such as Product Name and initializes
    /// the BoilerNodeManager which provides access to the data exposed by the Server.
    /// </remarks>
    public class TestServer : StandardServer
    {
        /// <summary>
        /// var variables = new Dictionary<string, BuiltInType>()
        /// {
        /// 	{"Var1", BuiltInType.UInt32 },
        /// 	{"Var2", BuiltInType.UInt16 },
        /// 	{"Var3", BuiltInType.Byte },
        /// };
        /// 
        /// var s = TestServer.StartServer(variables);
        /// 
        /// System.Threading.Thread.Sleep(10000);
        /// 
        /// s.SetValue("Var1", 1234);
        /// s.SetValue("Var2", 12345);
        /// s.SetValue("Var3", 123);
        /// 
        /// Task.Run(() => {
        /// 		while (true)
        /// 		{
        /// 		System.Threading.Thread.Sleep(5000);
        /// 		}
        /// 		}).Wait();
        /// </summary>
        /// <param name="variables"></param>
        /// <returns></returns>
        public static TestServer StartServer(ApplicationInstance application, Dictionary<string, BuiltInType> variables)
        {

            var s = new TestServer(variables);

            application.Start(s).Wait();

            return s;
        }

        Dictionary<string, BuiltInType> variables;
        NodeManager nodeManager;

        public TestServer(Dictionary<string, BuiltInType> variables)
        {
            this.variables = variables;
        }

        public bool AddValue(string name, object val)
        {
            return nodeManager == null ? false : nodeManager.AddValue(name, val);
        }

        public bool SetValue(string name, object val)
        {
            return nodeManager == null ? false : nodeManager.SetValue(name, val);
        }

        #region Overridden Methods
        /// <summary>
        /// Creates the node managers for the server.
        /// </summary>
        /// <remarks>
        /// This method allows the sub-class create any additional node managers which it uses. The SDK
        /// always creates a CoreNodeManager which handles the built-in nodes defined by the specification.
        /// Any additional NodeManagers are expected to handle application specific nodes.
        /// </remarks>
        protected override MasterNodeManager CreateMasterNodeManager(IServerInternal server, ApplicationConfiguration configuration)
        {
            Utils.Trace("Creating the Node Managers.");

            List<INodeManager> nodeManagers = new List<INodeManager>();

            // create the custom node managers.
            nodeManager = new NodeManager(server, configuration, variables);
            nodeManagers.Add(nodeManager);

            // create master node manager.
            return new MasterNodeManager(server, configuration, null, nodeManagers.ToArray());
        }

        /// <summary>
        /// Loads the non-configurable properties for the application.
        /// </summary>
        /// <remarks>
        /// These properties are exposed by the server but cannot be changed by administrators.
        /// </remarks>
        protected override ServerProperties LoadServerProperties()
        {
            ServerProperties properties = new ServerProperties();

            properties.ManufacturerName = "bamchoh";
            properties.ProductName = "Test Server";
            properties.ProductUri = "http://opcfoundation.org/TestServer/v1.0";
            properties.SoftwareVersion = Utils.GetAssemblySoftwareVersion();
            properties.BuildNumber = Utils.GetAssemblyBuildNumber();
            properties.BuildDate = Utils.GetAssemblyTimestamp();

            // TBD - All applications have software certificates that need to added to the properties.

            return properties;
        }
        #endregion
    }
}
