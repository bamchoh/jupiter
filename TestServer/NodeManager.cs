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
using System.Diagnostics;
using System.Xml;
using System.IO;
using System.Threading;
using Opc.Ua;
using Opc.Ua.Server;

namespace TestServer
{
    /// <summary>
    /// A node manager for a server that exposes several variables.
    /// </summary>
    public class NodeManager : CustomNodeManager2
    {
        Dictionary<string, BuiltInType> m_variables;
        List<BaseDataVariableState> m_dynamicVars;

        #region Constructors
        /// <summary>
        /// Initializes the node manager.
        /// </summary>
        public NodeManager(IServerInternal server, ApplicationConfiguration configuration, Dictionary<string, BuiltInType> variable)
        :
            base(server, configuration, Namespaces.Empty)
        {
            m_variables = variable;

            SystemContext.NodeIdFactory = this;

            // get the configuration for the node manager.
            m_configuration = configuration.ParseExtension<ServerConfiguration>();

            // use suitable defaults if no configuration exists.
            if (m_configuration == null)
            {
                m_configuration = new ServerConfiguration();
            }
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// An overrideable version of the Dispose.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // TBD
            }
        }
        #endregion

        #region INodeIdFactory Members
        /// <summary>
        /// Creates the NodeId for the specified node.
        /// </summary>
        public override NodeId New(ISystemContext context, NodeState node)
        {
            // generate a new numeric id in the instance namespace.
            return node.NodeId;
        }
        #endregion

        #region INodeManager Members
        /// <summary>
        /// Does any initialization required before the address space can be used.
        /// </summary>
        /// <remarks>
        /// The externalReferences is an out parameter that allows the node manager to link to nodes
        /// in other node managers. For example, the 'Objects' node is managed by the CoreNodeManager and
        /// should have a reference to the root folder node(s) exposed by this node manager.  
        /// </remarks>
        public override void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            lock (Lock)
            {
                var rootName = "TestData";
                FolderState root = NewFolder(rootName, null);

                // ensure trigger can be found via the server object. 
                IList<IReference> references = null;

                if (!externalReferences.TryGetValue(ObjectIds.ObjectsFolder, out references))
                {
                    externalReferences[ObjectIds.ObjectsFolder] = references = new List<IReference>();
                }

                root.AddReference(ReferenceTypeIds.Organizes, true, ObjectIds.ObjectsFolder);
                references.Add(new NodeStateReference(ReferenceTypeIds.Organizes, false, root.NodeId));

                m_dynamicVars = new List<BaseDataVariableState>();

                foreach(var v in m_variables)
                {
                    var vv = CreateAnalogItemVariable(root, rootName + "_" + v.Key, v.Key, (uint)v.Value);
                    m_dynamicVars.Add(vv);
                }

                // save in dictionary. 
                AddPredefinedNode(SystemContext, root);

                /*
                ReferenceTypeState referenceType = new ReferenceTypeState();

                referenceType.NodeId = new NodeId(3, NamespaceIndex);
                referenceType.BrowseName = new QualifiedName("IsTriggerSource", NamespaceIndex);
                referenceType.DisplayName = referenceType.BrowseName.Name;
                referenceType.InverseName = new LocalizedText("IsSourceOfTrigger");
                referenceType.SuperTypeId = ReferenceTypeIds.NonHierarchicalReferences;

                if (!externalReferences.TryGetValue(ObjectIds.Server, out references))
                {
                    externalReferences[ObjectIds.Server] = references = new List<IReference>();
                }

                trigger.AddReference(referenceType.NodeId, false, ObjectIds.Server);
                references.Add(new NodeStateReference(referenceType.NodeId, true, trigger.NodeId));

                // save in dictionary. 
                AddPredefinedNode(SystemContext, referenceType);
                */
            }
        }

        public bool SetValue(string name, object val)
        {
            lock (Lock)
            {
                if (m_dynamicVars == null)
                    return false;

                foreach (BaseDataVariableState variable in m_dynamicVars)
                {
                    if (variable.DisplayName != name)
                    {
                        continue;
                    }

                    var t = TypeInfo.GetBuiltInType(variable.DataType, Server.TypeTree);
                    switch (t)
                    {
                        case BuiltInType.Byte:
                            {
                                var v1 = 0;
                                var v2 = (int)val;
                                variable.Value = (byte)(v1 + v2);
                                break;
                            }
                        case BuiltInType.UInt16:
                            {
                                var v1 = 0;
                                var v2 = (int)val;
                                variable.Value = (ushort)(v1 + v2);
                                break;
                            }
                        case BuiltInType.UInt32:
                            {
                                var v1 = 0;
                                var v2 = (int)val;
                                variable.Value = (uint)(v1 + v2);
                                break;
                            }
                    }
                    variable.Timestamp = DateTime.UtcNow;
                    variable.ClearChangeMasks(SystemContext, false);
                    return true;
                }
            }
            return false;
        }

        public bool AddValue(string name, object val)
        {
            lock(Lock)
            {
                if (m_dynamicVars == null)
                    return false;

                foreach (BaseDataVariableState variable in m_dynamicVars)
                {
                    if(variable.DisplayName != name)
                    {
                        continue;
                    }

                    var t = TypeInfo.GetBuiltInType(variable.DataType, Server.TypeTree);
                    switch(t)
                    {
                        case BuiltInType.Byte:
                            {
                                var v1 = (byte)variable.Value;
                                var v2 = (int)val;
                                variable.Value = (byte)(v1 + v2);
                                break;
                            }
                        case BuiltInType.UInt16:
                            {
                                var v1 = (ushort)variable.Value;
                                var v2 = (int)val;
                                variable.Value = (ushort)(v1 + v2);
                                break;
                            }
                        case BuiltInType.UInt32:
                            {
                                var v1 = (uint)variable.Value;
                                var v2 = (int)val;
                                variable.Value = (uint)(v1 + v2);
                                break;
                            }
                    }
                    variable.Timestamp = DateTime.UtcNow;
                    variable.ClearChangeMasks(SystemContext, false);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Frees any resources allocated for the address space.
        /// </summary>
        public override void DeleteAddressSpace()
        {
            lock (Lock)
            {
                base.DeleteAddressSpace();
            }
        }

        /// <summary>
        /// Returns a unique handle for the node.
        /// </summary>
        protected override NodeHandle GetManagerHandle(ServerSystemContext context, NodeId nodeId, IDictionary<NodeId, NodeState> cache)
        {
            lock (Lock)
            {
                // quickly exclude nodes that are not in the namespace. 
                if (!IsNodeIdInNamespace(nodeId))
                {
                    return null;
                }

                NodeState node = null;

                if (!PredefinedNodes.TryGetValue(nodeId, out node))
                {
                    return null;
                }

                NodeHandle handle = new NodeHandle();

                handle.NodeId = nodeId;
                handle.Node = node;
                handle.Validated = true;

                return handle;
            }
        }

        /// <summary>
        /// Verifies that the specified node exists.
        /// </summary>
        protected override NodeState ValidateNode(
            ServerSystemContext context,
            NodeHandle handle,
            IDictionary<NodeId, NodeState> cache)
        {
            // not valid if no root.
            if (handle == null)
            {
                return null;
            }

            // check if previously validated.
            if (handle.Validated)
            {
                return handle.Node;
            }

            // TBD

            return null;
        }
        #endregion

        #region Private Fields
        private ServerConfiguration m_configuration;
        #endregion

        private FolderState NewFolder(string name, NodeState parent)
        {
            var folder = new FolderState(null);

            folder.SymbolicName = name;
            folder.ReferenceTypeId = ReferenceTypes.Organizes;
            folder.TypeDefinitionId = ObjectTypeIds.FolderType;
            folder.NodeId = new NodeId(1, NamespaceIndex);
            folder.BrowseName = new QualifiedName(name, NamespaceIndex);
            folder.DisplayName = folder.BrowseName.Name;
            folder.WriteMask = AttributeWriteMask.None;
            folder.UserWriteMask = AttributeWriteMask.None;
            folder.EventNotifier = EventNotifiers.None;

            return folder;
        }


        private AnalogItemState CreateAnalogItemVariable(NodeState parent, string path, string name, NodeId dataType)
        {
            int valueRank = ValueRanks.Scalar;
            var variable = new AnalogItemState(parent);
            variable.BrowseName = new QualifiedName(path, NamespaceIndex);
            variable.EngineeringUnits = new PropertyState<EUInformation>(variable);

            variable.InstrumentRange = new PropertyState<Opc.Ua.Range>(variable);

            variable.Create(
                SystemContext,
                new NodeId(path, NamespaceIndex),
                variable.BrowseName,
                null,
                true);

            variable.NodeId = new NodeId(path, NamespaceIndex);
            variable.SymbolicName = name;
            variable.DisplayName = name;
            variable.WriteMask = AttributeWriteMask.None;
            variable.UserWriteMask = AttributeWriteMask.None;
            variable.ReferenceTypeId = ReferenceTypes.Organizes;
            variable.DataType = dataType;
            variable.ValueRank = valueRank;
            variable.AccessLevel = AccessLevels.CurrentReadOrWrite;
            variable.UserAccessLevel = AccessLevels.CurrentReadOrWrite;
            variable.Historizing = false;

            variable.OnReadUserRolePermissions = OnReadUserRolePermissions;

            if (valueRank == ValueRanks.OneDimension)
            {
                variable.ArrayDimensions = new ReadOnlyList<uint>(new List<uint> { 0 });
            }
            else if (valueRank == ValueRanks.TwoDimensions)
            {
                variable.ArrayDimensions = new ReadOnlyList<uint>(new List<uint> { 0, 0 });
            }

            BuiltInType builtInType = Opc.Ua.TypeInfo.GetBuiltInType(dataType, Server.TypeTree);

            // Using anything but 120,-10 fails a few tests
            variable.InstrumentRange.Value = GetAnalogRange(builtInType); ;

            variable.EURange.Value = new Opc.Ua.Range(100, 0);
            variable.Value = Opc.Ua.TypeInfo.GetDefaultValue(dataType, valueRank, Server.TypeTree);

            variable.StatusCode = StatusCodes.Good;
            variable.Timestamp = DateTime.UtcNow;
            // The latest UNECE version (Rev 11, published in 2015) is available here:
            // http://www.opcfoundation.org/UA/EngineeringUnits/UNECE/rec20_latest_08052015.zip
            variable.EngineeringUnits.Value = new EUInformation("mV", "millivolt", "http://www.opcfoundation.org/UA/units/un/cefact");
            // The mapping of the UNECE codes to OPC UA(EUInformation.unitId) is available here:
            // http://www.opcfoundation.org/UA/EngineeringUnits/UNECE/UNECE_to_OPCUA.csv
            variable.EngineeringUnits.Value.UnitId = 12890; // "2Z"
            variable.OnWriteValue = OnWriteAnalog;
            variable.EURange.OnWriteValue = OnWriteAnalogRange;
            variable.EURange.AccessLevel = AccessLevels.CurrentReadOrWrite;
            variable.EURange.UserAccessLevel = AccessLevels.CurrentReadOrWrite;
            variable.EngineeringUnits.AccessLevel = AccessLevels.CurrentReadOrWrite;
            variable.EngineeringUnits.UserAccessLevel = AccessLevels.CurrentReadOrWrite;
            variable.InstrumentRange.OnWriteValue = OnWriteAnalogRange;
            variable.InstrumentRange.AccessLevel = AccessLevels.CurrentReadOrWrite;
            variable.InstrumentRange.UserAccessLevel = AccessLevels.CurrentReadOrWrite;

            if (parent != null)
            {
                parent.AddChild(variable);
            }

            return variable;
        }

        #region Private Helper Functions
        private static bool IsUnsignedAnalogType(BuiltInType builtInType)
        {
            if (builtInType == BuiltInType.Byte ||
                builtInType == BuiltInType.UInt16 ||
                builtInType == BuiltInType.UInt32 ||
                builtInType == BuiltInType.UInt64)
            {
                return true;
            }
            return false;
        }

        private static bool IsAnalogType(BuiltInType builtInType)
        {
            switch (builtInType)
            {
                case BuiltInType.Byte:
                case BuiltInType.UInt16:
                case BuiltInType.UInt32:
                case BuiltInType.UInt64:
                case BuiltInType.SByte:
                case BuiltInType.Int16:
                case BuiltInType.Int32:
                case BuiltInType.Int64:
                case BuiltInType.Float:
                case BuiltInType.Double:
                    return true;
            }
            return false;
        }

        private static Opc.Ua.Range GetAnalogRange(BuiltInType builtInType)
        {
            switch (builtInType)
            {
                case BuiltInType.UInt16:
                    return new Opc.Ua.Range(System.UInt16.MaxValue, System.UInt16.MinValue);
                case BuiltInType.UInt32:
                    return new Opc.Ua.Range(System.UInt32.MaxValue, System.UInt32.MinValue);
                case BuiltInType.UInt64:
                    return new Opc.Ua.Range(System.UInt64.MaxValue, System.UInt64.MinValue);
                case BuiltInType.SByte:
                    return new Opc.Ua.Range(System.SByte.MaxValue, System.SByte.MinValue);
                case BuiltInType.Int16:
                    return new Opc.Ua.Range(System.Int16.MaxValue, System.Int16.MinValue);
                case BuiltInType.Int32:
                    return new Opc.Ua.Range(System.Int32.MaxValue, System.Int32.MinValue);
                case BuiltInType.Int64:
                    return new Opc.Ua.Range(System.Int64.MaxValue, System.Int64.MinValue);
                case BuiltInType.Float:
                    return new Opc.Ua.Range(System.Single.MaxValue, System.Single.MinValue);
                case BuiltInType.Double:
                    return new Opc.Ua.Range(System.Double.MaxValue, System.Double.MinValue);
                case BuiltInType.Byte:
                    return new Opc.Ua.Range(System.Byte.MaxValue, System.Byte.MinValue);
                default:
                    return new Opc.Ua.Range(System.SByte.MaxValue, System.SByte.MinValue);
            }
        }
        #endregion

        private ServiceResult OnReadUserRolePermissions(
        ISystemContext context,
        NodeState node,
        ref RolePermissionTypeCollection value)
        {
            return StatusCodes.BadUserAccessDenied;
        }

        private ServiceResult OnWriteAnalog(
            ISystemContext context,
            NodeState node,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp)
        {
            AnalogItemState variable = node as AnalogItemState;

            // verify data type.
            Opc.Ua.TypeInfo typeInfo = Opc.Ua.TypeInfo.IsInstanceOfDataType(
                value,
                variable.DataType,
                variable.ValueRank,
                context.NamespaceUris,
                context.TypeTable);

            if (typeInfo == null || typeInfo == Opc.Ua.TypeInfo.Unknown)
            {
                return StatusCodes.BadTypeMismatch;
            }

            // check index range.
            if (variable.ValueRank >= 0)
            {
                if (indexRange != NumericRange.Empty)
                {
                    object target = variable.Value;
                    ServiceResult result = indexRange.UpdateRange(ref target, value);

                    if (ServiceResult.IsBad(result))
                    {
                        return result;
                    }

                    value = target;
                }
            }

            // check instrument range.
            else
            {
                if (indexRange != NumericRange.Empty)
                {
                    return StatusCodes.BadIndexRangeInvalid;
                }

                double number = Convert.ToDouble(value);

                if (variable.InstrumentRange != null && (number < variable.InstrumentRange.Value.Low || number > variable.InstrumentRange.Value.High))
                {
                    return StatusCodes.BadOutOfRange;
                }
            }

            return ServiceResult.Good;
        }

        private ServiceResult OnWriteAnalogRange(
            ISystemContext context,
            NodeState node,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp)
        {
            PropertyState<Opc.Ua.Range> variable = node as PropertyState<Opc.Ua.Range>;
            ExtensionObject extensionObject = value as ExtensionObject;
            TypeInfo typeInfo = TypeInfo.Construct(value);

            if (variable == null ||
                extensionObject == null ||
                typeInfo == null ||
                typeInfo == Opc.Ua.TypeInfo.Unknown)
            {
                return StatusCodes.BadTypeMismatch;
            }

            Opc.Ua.Range newRange = extensionObject.Body as Opc.Ua.Range;
            AnalogItemState parent = variable.Parent as AnalogItemState;
            if (newRange == null ||
                parent == null)
            {
                return StatusCodes.BadTypeMismatch;
            }

            if (indexRange != NumericRange.Empty)
            {
                return StatusCodes.BadIndexRangeInvalid;
            }

            TypeInfo parentTypeInfo = TypeInfo.Construct(parent.Value);
            Opc.Ua.Range parentRange = GetAnalogRange(parentTypeInfo.BuiltInType);
            if (parentRange.High < newRange.High ||
                parentRange.Low > newRange.Low)
            {
                return StatusCodes.BadOutOfRange;
            }

            value = newRange;

            return ServiceResult.Good;
        }

    }
}
