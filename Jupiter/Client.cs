using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Opc.Ua;
using Opc.Ua.Client;
using System.Collections;
using System.Runtime.CompilerServices;
using Prism.Mvvm;
using Opc.Ua.Configuration;

using Prism.Events;

namespace Jupiter
{
    public class Client : BindableBase, Interfaces.IConnection, Interfaces.IReferenceFetchable, Interfaces.INodeInfoGetter, Interfaces.ISubscriptionOperatable, Interfaces.IOneTimeAccessOperator
    {
        public IEventAggregator EventAggregator { get; set; }

        #region Private Fields
        private Session session;
        private Subscription subscription;
        private Opc.Ua.ApplicationConfiguration config;
        private Interfaces.IVariableInfoManager variableInfoManager;

        private bool connected;
        #endregion

        #region Public Event
        public event ClientNotificationEventHandler SessionNotification;
        #endregion

        #region Constructor
        public Client(Interfaces.IVariableInfoManager variableInfoManager, IEventAggregator ea)
        {
            this.variableInfoManager = variableInfoManager;

            this.EventAggregator = ea;

            this.config = ApplicationConfiguration.Load(null);
            config.CertificateValidator.CertificateValidation += new CertificateValidationEventHandler(validateCerts);
        }
        #endregion

        #region Properties
        public bool Connected
        {
            get { return connected; }
            set { SetProperty(ref connected, value); }
        }
        #endregion

        #region Methods
        public List<NodeInfo> GetNodeInfoList(ExpandedNodeId nodeid)
        {
            var nodeInfoList = new List<NodeInfo>();

            var n = FindNode(nodeid) as ILocalNode;

            if (n == null)
            {
                return null;
            }

            uint[] attributesIds = Attributes.GetIdentifiers();

            foreach (uint attributesId in attributesIds)
            {
                if (!n.SupportsAttribute(attributesId))
                {
                    continue;
                }

                var _value = new DataValue(StatusCodes.BadWaitingForInitialData);

                ServiceResult result = n.Read(null, attributesId, _value);

                if (ServiceResult.IsBad(result))
                {
                    _value = new DataValue(result.StatusCode);
                }

                var _name = Attributes.GetBrowseName(attributesId);

                nodeInfoList.Add(new NodeInfo { Name = _name, Value = _value.ToString() });
            }

            IList<IReference> references = n.References.Find(ReferenceTypes.HasProperty, false, true, session.TypeTree);

            for (int ii = 0; ii < references.Count; ii++)
            {
                IReference reference = references[ii];

                ILocalNode property = FindNode(reference.TargetId) as ILocalNode;

                if (property == null)
                {
                    break;
                }

                var _name = Utils.Format("{0}", property.DisplayName);
                var _value = new DataValue(StatusCodes.BadWaitingForInitialData);

                ServiceResult result = property.Read(null, Attributes.Value, _value);

                if (ServiceResult.IsBad(result))
                {
                    _value = new DataValue(result.StatusCode);
                }

                nodeInfoList.Add(new NodeInfo { Name = _name, Value = _value.ToString() });
            }

            return nodeInfoList;
        }

        public void RemoveMonitoredItem(List<uint> chList)
        {
            try
            {
                var miList = new List<MonitoredItem>();
                foreach (var ch in chList)
                {
                    foreach (var mi in subscription.MonitoredItems)
                    {
                        if (mi.ClientHandle == ch)
                        {
                            miList.Add(mi);
                            break;
                        }
                    }
                }

                subscription.RemoveItems(miList);
                subscription.ApplyChanges();

                if (subscription.MonitoredItemCount == 0)
                {
                    if(!session.RemoveSubscription(subscription))
                    {
                        throw new Exception("Removing subscription was failed.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessagePassing(ex);

                Close();
            }
        }

        public async Task<IList<VariableInfoBase>> AddToSubscription(IList objs)
        {

            /*
            var id = this.ToNodeId(expandedNodeId);
            var mask = (uint)NodeClass.Variable;
            ReferenceDescriptionCollection refs;
            this.Browse(id, mask, out refs);

            try
            {
                FindNodes(refs, ref nodes);
            }
            catch (Exception ex)
            {

            }
            */

            System.Diagnostics.Debug.WriteLine("---------------------------------------------------------");
            System.Diagnostics.Debug.WriteLine("----------- (START) Add To Subscription -----------------");

            try
            {
                if (objs == null || objs.Count == 0)
                    return null;

                var viList = await Task.Run(() => variableInfoManager.GenerateVariableInfoList(objs));

                if (viList.Count == 0)
                    return viList;

                if (session.Subscriptions.Count() == 0)
                {
                    subscription = new Subscription(session.DefaultSubscription) { PublishingInterval = 100 };
                    session.AddSubscription(subscription);
                    subscription.Create();
                }
                else
                {
                    subscription = session.Subscriptions.ElementAt(0);
                }

                var list = new List<MonitoredItem>();
                foreach (var vi in viList)
                {
                    var monitoredItem = new MonitoredItem(subscription.DefaultItem)
                    {
                        StartNodeId = vi.NodeId
                    };

                    list.Add(monitoredItem);

                    vi.ClientHandle = monitoredItem.ClientHandle;
                    vi.PropertyChanged += Vi_PropertyChanged;
                }

                subscription.AddItems(list);
                subscription.ApplyChanges();

                subscription.Session.Notification += Session_Notification;

                return viList;
            }
            catch(Exception ex)
            {
                MessagePassing(ex);

                Close();

                return null;
            }
            finally
            {
                System.Diagnostics.Debug.WriteLine("----------- (END) Add To Subscription -----------------");
                System.Diagnostics.Debug.WriteLine("-------------------------------------------------------");
            }
        }

        public INode FindNode(ExpandedNodeId id)
        {
            return session.NodeCache.Find(id);
        }

        public NodeId ToNodeId(ExpandedNodeId id)
        {
            return ExpandedNodeId.ToNodeId(id, session.NamespaceUris);
        }

        public ITypeTable TypeTable
        {
            get
            {
                return session.TypeTree;
            }
        }

        public ResponseHeader Browse(NodeId id, uint mask, out ReferenceDescriptionCollection refs)
        {
            byte[] continuationPoint;

            ResponseHeader resp;

            resp = session.Browse(
                    null,
                    null,
                    id,
                    0u,
                    BrowseDirection.Forward,
                    ReferenceTypeIds.HierarchicalReferences,
                    true,
                    mask,
                    out continuationPoint,
                    out refs);

            while (continuationPoint != null)
            {
                byte[] revisedContinuationPoint;
                ReferenceDescriptionCollection additionalDescription;

                resp = session.BrowseNext(
                    null,
                    false,
                    continuationPoint,
                    out revisedContinuationPoint,
                    out additionalDescription);

                continuationPoint = revisedContinuationPoint;

                refs.AddRange(additionalDescription);
            }

            return resp;
        }

        public ReferenceDescriptionCollection FetchReferences(NodeId id)
        {
            return session.FetchReferences(id);
        }

        public EndpointDescriptionCollection Discover(string discoveryUrl, int operationTimeout)
        {
            Uri uri = new Uri(discoveryUrl);

            EndpointConfiguration configuration = EndpointConfiguration.Create();
            if (operationTimeout > 0)
            {
                configuration.OperationTimeout = operationTimeout;
            }

            var endpoints = new EndpointDescriptionCollection();

            using (DiscoveryClient client = DiscoveryClient.Create(uri, configuration))
            {
                EndpointDescriptionCollection endpointCollection = null;

                client.GetEndpoints(null, client.Endpoint.EndpointUrl, null, null, out endpointCollection);

                for(int i=0;i<endpointCollection.Count();i++)
                {
                    var got = endpointCollection[i];
                    if (got.EndpointUrl == discoveryUrl)
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("## [{2}] Endpoint URL:{0}#{1}", got.EndpointUrl, got.SecurityMode, i));
                        endpoints.Add(got);
                    }
                }
            }

            return endpoints;
        }

        private async Task _createSession(EndpointDescription endpointDescription, string username, string password)
        {
            UserIdentity uid;
            if (string.IsNullOrEmpty(username))
            {
                uid = new UserIdentity(new AnonymousIdentityToken());
            }
            else
            {
                if (string.IsNullOrEmpty(password))
                {
                    password = "";
                }

                uid = new UserIdentity(username, password);
            }

            var endpointConfiguration = EndpointConfiguration.Create(config);
            var endpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfiguration);
            session = await Session.Create(
                config,
                endpoint,
                false,
                config.ApplicationName,
                60000,
                uid,
                null);

            session.KeepAlive += Session_KeepAlive;
            session.PublishError += Session_PublishError;
            Connected = session.Connected;
        }

        public async Task CreateSession(string endpointURI)
        {
            var endpoints = await Task.Run(() => Discover(endpointURI, 15000));

            var securityList = new List<string>();
            foreach (var ed in endpoints)
            {
                var discoveryURL = "";
                if (ed.Server.DiscoveryUrls.Count() != 0)
                    discoveryURL = ed.Server.DiscoveryUrls[0];

                securityList.Add(string.Format("{0} - {1} - {2}",
                    ed.Server.ApplicationName,
                    Opc.Ua.SecurityPolicies.GetDisplayName(ed.SecurityPolicyUri),
                    ed.SecurityMode));
            }

            var sem = new SemaphoreSlim(1, 1);
            var result = new Events.NowLoading()
            {
                SecurityList = securityList,
                Endpoint = endpointURI,
                UserName = "",
                Password = "",
                Semaphore = sem,
            };
            this.EventAggregator
                .GetEvent<Events.NowLoadingEvent>()
                .Publish(result);
            await sem.WaitAsync();

            try
            {
                var i = result.SelectedIndex;
                var username = result.UserName;
                var password = result.Password;
                await _createSession(endpoints[i], username, password);
            }
            finally
            {
                sem.Release();
            }
        }

        private object MessageLockObject = new object();

        private void Session_KeepAlive(Session session, KeepAliveEventArgs e)
        {
            lock(MessageLockObject)
            {
                if (e.Status != null)
                {
                    var message = string.Format("[KeepAlive Error]{0}", e.Status.LocalizedText.Text);

                    MessagePassing(new Exception(message));

                    Close();
                }
            }
        }

        private void Session_PublishError(Session session, PublishErrorEventArgs e)
        {
            lock (MessageLockObject)
            {
                if (e.Status != null)
                {
                    var message = string.Format("[Publish Error]{0}", e.Status.LocalizedText.Text);

                    MessagePassing(new Exception(message));

                    Close();
                }
            }
        }

        public void Close()
        {
            if (subscription != null)
            {
                subscription.Delete(true);
            }

            if (session != null && session.Connected)
            {
                session.Close();

                Connected = session.Connected;
            }
        }

        public ResponseHeader Read(ReadValueIdCollection itemsToRead, out DataValueCollection values, out DiagnosticInfoCollection diagnosticInfos)
        {
            return session.Read(
                null,
                0,
                TimestampsToReturn.Both,
                itemsToRead,
                out values,
                out diagnosticInfos);
        }

        public void Write(IList<VariableInfoBase> items)
        {
            Func<VariableInfoBase, object> func = (vi) => vi.GetPrepareValue();

            innerWrite(items, func);
        }

        public void FetchVariableReferences(ExpandedNodeId expandedNodeId, ref List<VariableNode> nodes)
        {
            var id = this.ToNodeId(expandedNodeId);

            var mask = (uint)NodeClass.Variable;
            ReferenceDescriptionCollection refs;
            this.Browse(id, mask, out refs);

            try
            {
                foreach (var r in refs)
                {
                    var rid = this.ToNodeId(r.NodeId);
                    var node = session.ReadNode(rid);
                    nodes.Add((VariableNode)node);
                }
            }
            catch (Exception ex)
            {
                string Message;
                string StackTrace;
                if (ex.InnerException != null)
                {
                    Message = ex.InnerException.Message;
                    StackTrace = ex.InnerException.StackTrace;
                }
                else
                {
                    Message = ex.Message;
                    StackTrace = ex.StackTrace;
                }

                System.Diagnostics.Trace.WriteLine("--- Exception in FindNodes {{{");
                System.Diagnostics.Trace.WriteLine(Message);
                System.Diagnostics.Trace.WriteLine(StackTrace);
                System.Diagnostics.Trace.WriteLine("--- Exception in FindNodes }}}");
            }

            /*
            ReferenceDescriptionCollection refs;
            this.Browse(id, mask, out refs);

            try
            {
                FindNodes(refs, ref nodes);
            }
            catch(Exception ex)
            {
                string Message;
                string StackTrace;
                if (ex.InnerException != null)
                {
                    Message = ex.InnerException.Message;
                    StackTrace = ex.InnerException.StackTrace;
                }
                else
                {
                    Message = ex.Message;
                    StackTrace = ex.StackTrace;
                }

                System.Diagnostics.Trace.WriteLine("--- Exception in FindNodes {{{");
                System.Diagnostics.Trace.WriteLine(Message);
                System.Diagnostics.Trace.WriteLine(StackTrace);
                System.Diagnostics.Trace.WriteLine("--- Exception in FindNodes }}}");
            }
            */
        }

        public void FindNodes(ReferenceDescriptionCollection refs, ref List<VariableNode> nodes)
        {
            // build list of values to read.
            ReadValueIdCollection itemsToRead = new ReadValueIdCollection();

            var attr4node = new List<SortedDictionary<uint, DataValue>>();

            foreach (var reference in refs)
            {
                // build list of attributes.
                SortedDictionary<uint, DataValue> attributes = new SortedDictionary<uint, DataValue>();

                attributes.Add(Attributes.NodeId, null);
                attributes.Add(Attributes.NodeClass, null);
                attributes.Add(Attributes.BrowseName, null);
                attributes.Add(Attributes.DisplayName, null);
                attributes.Add(Attributes.Description, null);
                attributes.Add(Attributes.WriteMask, null);
                attributes.Add(Attributes.UserWriteMask, null);
                attributes.Add(Attributes.DataType, null);
                attributes.Add(Attributes.ValueRank, null);
                attributes.Add(Attributes.ArrayDimensions, null);
                attributes.Add(Attributes.AccessLevel, null);
                attributes.Add(Attributes.UserAccessLevel, null);
                attributes.Add(Attributes.Historizing, null);
                attributes.Add(Attributes.MinimumSamplingInterval, null);
                attributes.Add(Attributes.EventNotifier, null);
                attributes.Add(Attributes.Executable, null);
                attributes.Add(Attributes.UserExecutable, null);
                attributes.Add(Attributes.IsAbstract, null);
                attributes.Add(Attributes.InverseName, null);
                attributes.Add(Attributes.Symmetric, null);
                attributes.Add(Attributes.ContainsNoLoops, null);
                attributes.Add(Attributes.DataTypeDefinition, null);
                // attributes.Add(Attributes.RolePermissions, null);
                // attributes.Add(Attributes.UserRolePermissions, null);
                // attributes.Add(Attributes.AccessRestrictions, null);
                attributes.Add(Attributes.AccessLevelEx, null);

                attr4node.Add(attributes);

                foreach (uint attributeId in attributes.Keys)
                {
                    ReadValueId itemToRead = new ReadValueId();

                    itemToRead.NodeId = ToNodeId(reference.NodeId);
                    itemToRead.AttributeId = attributeId;

                    itemsToRead.Add(itemToRead);
                }
            }

            // read from server.
            DataValueCollection values = null;
            DiagnosticInfoCollection diagnosticInfos = null;

            ResponseHeader responseHeader = this.session.Read(
                null,
                0,
                TimestampsToReturn.Neither,
                itemsToRead,
                out values,
                out diagnosticInfos);

            ClientBase.ValidateResponse(values, itemsToRead);
            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, itemsToRead);

            var nodeClasses = new int?[attr4node.Count];

            for (int ii = 0, jj = -1; ii < itemsToRead.Count; ii++)
            {
                if (ii % attr4node[0].Count == 0)
                    jj++;

                uint attributeId = itemsToRead[ii].AttributeId;

                // the node probably does not exist if the node class is not found.
                if (attributeId == Attributes.NodeClass)
                {
                    if (!DataValue.IsGood(values[ii]))
                    {
                        throw ServiceResultException.Create(values[ii].StatusCode, ii, diagnosticInfos, responseHeader.StringTable);
                    }

                    // check for valid node class.
                    nodeClasses[jj] = values[ii].Value as int?;

                    if (nodeClasses[jj] == null)
                    {
                        throw ServiceResultException.Create(StatusCodes.BadUnexpectedError, "Node does not have a valid value for NodeClass: {0}.", values[ii].Value);
                    }
                }
                else
                {
                    if (!DataValue.IsGood(values[ii]))
                    {
                        // check for unsupported attributes.
                        if (values[ii].StatusCode == StatusCodes.BadAttributeIdInvalid)
                        {
                            continue;
                        }

                        // all supported attributes must be readable.
                        if (attributeId != Attributes.Value)
                        {
                            throw ServiceResultException.Create(values[ii].StatusCode, ii, diagnosticInfos, responseHeader.StringTable);
                        }
                    }
                }

                attr4node[jj][attributeId] = values[ii];
            }

            for(int jj = 0;jj < attr4node.Count; jj++)
            {
                var vnode = (VariableNode)GenerateNode(nodeClasses[jj], attr4node[jj]);
                nodes.Add(vnode);
            }
        }

        private Node GenerateNode(int? nodeClass, SortedDictionary<uint, DataValue> attributes)
        {
            Node node;
            DataValue value;


            switch ((NodeClass)nodeClass.Value)
            {
                default:
                    {
                        throw ServiceResultException.Create(StatusCodes.BadUnexpectedError, "Node does not have a valid value for NodeClass: {0}.", nodeClass.Value);
                    }

                case NodeClass.Object:
                    {
                        ObjectNode objectNode = new ObjectNode();

                        value = attributes[Attributes.EventNotifier];

                        if (value == null)
                        {
                            throw ServiceResultException.Create(StatusCodes.BadUnexpectedError, "Object does not support the EventNotifier attribute.");
                        }

                        objectNode.EventNotifier = (byte)attributes[Attributes.EventNotifier].GetValue(typeof(byte));
                        node = objectNode;
                        break;
                    }

                case NodeClass.ObjectType:
                    {
                        ObjectTypeNode objectTypeNode = new ObjectTypeNode();

                        value = attributes[Attributes.IsAbstract];

                        if (value == null)
                        {
                            throw ServiceResultException.Create(StatusCodes.BadUnexpectedError, "ObjectType does not support the IsAbstract attribute.");
                        }

                        objectTypeNode.IsAbstract = (bool)attributes[Attributes.IsAbstract].GetValue(typeof(bool));
                        node = objectTypeNode;
                        break;
                    }

                case NodeClass.Variable:
                    {
                        VariableNode variableNode = new VariableNode();

                        // DataType Attribute
                        value = attributes[Attributes.DataType];

                        if (value == null)
                        {
                            throw ServiceResultException.Create(StatusCodes.BadUnexpectedError, "Variable does not support the DataType attribute.");
                        }

                        variableNode.DataType = (NodeId)attributes[Attributes.DataType].GetValue(typeof(NodeId));

                        // ValueRank Attribute
                        value = attributes[Attributes.ValueRank];

                        if (value == null)
                        {
                            throw ServiceResultException.Create(StatusCodes.BadUnexpectedError, "Variable does not support the ValueRank attribute.");
                        }

                        variableNode.ValueRank = (int)attributes[Attributes.ValueRank].GetValue(typeof(int));

                        // ArrayDimensions Attribute
                        value = attributes[Attributes.ArrayDimensions];

                        if (value != null)
                        {
                            if (value.Value == null)
                            {
                                variableNode.ArrayDimensions = new uint[0];
                            }
                            else
                            {
                                variableNode.ArrayDimensions = (uint[])value.GetValue(typeof(uint[]));
                            }
                        }

                        // AccessLevel Attribute
                        value = attributes[Attributes.AccessLevel];

                        if (value == null)
                        {
                            throw ServiceResultException.Create(StatusCodes.BadUnexpectedError, "Variable does not support the AccessLevel attribute.");
                        }

                        variableNode.AccessLevel = (byte)attributes[Attributes.AccessLevel].GetValue(typeof(byte));

                        // UserAccessLevel Attribute
                        value = attributes[Attributes.UserAccessLevel];

                        if (value == null)
                        {
                            throw ServiceResultException.Create(StatusCodes.BadUnexpectedError, "Variable does not support the UserAccessLevel attribute.");
                        }

                        variableNode.UserAccessLevel = (byte)attributes[Attributes.UserAccessLevel].GetValue(typeof(byte));

                        // Historizing Attribute
                        value = attributes[Attributes.Historizing];

                        if (value == null)
                        {
                            throw ServiceResultException.Create(StatusCodes.BadUnexpectedError, "Variable does not support the Historizing attribute.");
                        }

                        variableNode.Historizing = (bool)attributes[Attributes.Historizing].GetValue(typeof(bool));

                        // MinimumSamplingInterval Attribute
                        value = attributes[Attributes.MinimumSamplingInterval];

                        if (value != null)
                        {
                            variableNode.MinimumSamplingInterval = Convert.ToDouble(attributes[Attributes.MinimumSamplingInterval].Value);
                        }

                        // AccessLevelEx Attribute
                        value = attributes[Attributes.AccessLevelEx];

                        if (value != null)
                        {
                            variableNode.AccessLevelEx = (uint)attributes[Attributes.AccessLevelEx].GetValue(typeof(uint));
                        }

                        node = variableNode;
                        break;
                    }

                case NodeClass.VariableType:
                    {
                        VariableTypeNode variableTypeNode = new VariableTypeNode();

                        // IsAbstract Attribute
                        value = attributes[Attributes.IsAbstract];

                        if (value == null)
                        {
                            throw ServiceResultException.Create(StatusCodes.BadUnexpectedError, "VariableType does not support the IsAbstract attribute.");
                        }

                        variableTypeNode.IsAbstract = (bool)attributes[Attributes.IsAbstract].GetValue(typeof(bool));

                        // DataType Attribute
                        value = attributes[Attributes.DataType];

                        if (value == null)
                        {
                            throw ServiceResultException.Create(StatusCodes.BadUnexpectedError, "VariableType does not support the DataType attribute.");
                        }

                        variableTypeNode.DataType = (NodeId)attributes[Attributes.DataType].GetValue(typeof(NodeId));

                        // ValueRank Attribute
                        value = attributes[Attributes.ValueRank];

                        if (value == null)
                        {
                            throw ServiceResultException.Create(StatusCodes.BadUnexpectedError, "VariableType does not support the ValueRank attribute.");
                        }

                        variableTypeNode.ValueRank = (int)attributes[Attributes.ValueRank].GetValue(typeof(int));

                        // ArrayDimensions Attribute
                        value = attributes[Attributes.ArrayDimensions];

                        if (value != null && value.Value != null)
                        {
                            variableTypeNode.ArrayDimensions = (uint[])attributes[Attributes.ArrayDimensions].GetValue(typeof(uint[]));
                        }

                        node = variableTypeNode;
                        break;
                    }

                case NodeClass.Method:
                    {
                        MethodNode methodNode = new MethodNode();

                        // Executable Attribute
                        value = attributes[Attributes.Executable];

                        if (value == null)
                        {
                            throw ServiceResultException.Create(StatusCodes.BadUnexpectedError, "Method does not support the Executable attribute.");
                        }

                        methodNode.Executable = (bool)attributes[Attributes.Executable].GetValue(typeof(bool));

                        // UserExecutable Attribute
                        value = attributes[Attributes.UserExecutable];

                        if (value == null)
                        {
                            throw ServiceResultException.Create(StatusCodes.BadUnexpectedError, "Method does not support the UserExecutable attribute.");
                        }

                        methodNode.UserExecutable = (bool)attributes[Attributes.UserExecutable].GetValue(typeof(bool));

                        node = methodNode;
                        break;
                    }

                case NodeClass.DataType:
                    {
                        DataTypeNode dataTypeNode = new DataTypeNode();

                        // IsAbstract Attribute
                        value = attributes[Attributes.IsAbstract];

                        if (value == null)
                        {
                            throw ServiceResultException.Create(StatusCodes.BadUnexpectedError, "DataType does not support the IsAbstract attribute.");
                        }

                        dataTypeNode.IsAbstract = (bool)attributes[Attributes.IsAbstract].GetValue(typeof(bool));

                        // DataTypeDefinition Attribute
                        value = attributes[Attributes.DataTypeDefinition];

                        if (value != null)
                        {
                            dataTypeNode.DataTypeDefinition = new ExtensionObject(attributes[Attributes.DataTypeDefinition].Value);
                        }

                        node = dataTypeNode;
                        break;
                    }

                case NodeClass.ReferenceType:
                    {
                        ReferenceTypeNode referenceTypeNode = new ReferenceTypeNode();

                        // IsAbstract Attribute
                        value = attributes[Attributes.IsAbstract];

                        if (value == null)
                        {
                            throw ServiceResultException.Create(StatusCodes.BadUnexpectedError, "ReferenceType does not support the IsAbstract attribute.");
                        }

                        referenceTypeNode.IsAbstract = (bool)attributes[Attributes.IsAbstract].GetValue(typeof(bool));

                        // Symmetric Attribute
                        value = attributes[Attributes.Symmetric];

                        if (value == null)
                        {
                            throw ServiceResultException.Create(StatusCodes.BadUnexpectedError, "ReferenceType does not support the Symmetric attribute.");
                        }

                        referenceTypeNode.Symmetric = (bool)attributes[Attributes.IsAbstract].GetValue(typeof(bool));

                        // InverseName Attribute
                        value = attributes[Attributes.InverseName];

                        if (value != null && value.Value != null)
                        {
                            referenceTypeNode.InverseName = (LocalizedText)attributes[Attributes.InverseName].GetValue(typeof(LocalizedText));
                        }

                        node = referenceTypeNode;
                        break;
                    }

                case NodeClass.View:
                    {
                        ViewNode viewNode = new ViewNode();

                        // EventNotifier Attribute
                        value = attributes[Attributes.EventNotifier];

                        if (value == null)
                        {
                            throw ServiceResultException.Create(StatusCodes.BadUnexpectedError, "View does not support the EventNotifier attribute.");
                        }

                        viewNode.EventNotifier = (byte)attributes[Attributes.EventNotifier].GetValue(typeof(byte));

                        // ContainsNoLoops Attribute
                        value = attributes[Attributes.ContainsNoLoops];

                        if (value == null)
                        {
                            throw ServiceResultException.Create(StatusCodes.BadUnexpectedError, "View does not support the ContainsNoLoops attribute.");
                        }

                        viewNode.ContainsNoLoops = (bool)attributes[Attributes.ContainsNoLoops].GetValue(typeof(bool));

                        node = viewNode;
                        break;
                    }
            }

            // NodeId Attribute
            value = attributes[Attributes.NodeId];

            if (value == null)
            {
                throw ServiceResultException.Create(StatusCodes.BadUnexpectedError, "Node does not support the NodeId attribute.");
            }

            node.NodeId = (NodeId)attributes[Attributes.NodeId].GetValue(typeof(NodeId));
            node.NodeClass = (NodeClass)nodeClass.Value;

            // BrowseName Attribute
            value = attributes[Attributes.BrowseName];

            if (value == null)
            {
                throw ServiceResultException.Create(StatusCodes.BadUnexpectedError, "Node does not support the BrowseName attribute.");
            }

            node.BrowseName = (QualifiedName)attributes[Attributes.BrowseName].GetValue(typeof(QualifiedName));

            // DisplayName Attribute
            value = attributes[Attributes.DisplayName];

            if (value == null)
            {
                throw ServiceResultException.Create(StatusCodes.BadUnexpectedError, "Node does not support the DisplayName attribute.");
            }

            node.DisplayName = (LocalizedText)attributes[Attributes.DisplayName].GetValue(typeof(LocalizedText));

            // Description Attribute
            value = attributes[Attributes.Description];

            if (value != null && value.Value != null)
            {
                node.Description = (LocalizedText)attributes[Attributes.Description].GetValue(typeof(LocalizedText));
            }

            // WriteMask Attribute
            value = attributes[Attributes.WriteMask];

            if (value != null)
            {
                node.WriteMask = (uint)attributes[Attributes.WriteMask].GetValue(typeof(uint));
            }

            // UserWriteMask Attribute
            value = attributes[Attributes.UserWriteMask];

            if (value != null)
            {
                node.WriteMask = (uint)attributes[Attributes.UserWriteMask].GetValue(typeof(uint));
            }

            // RolePermissions Attribute
            /*
            value = attributes[Attributes.RolePermissions];

            if (value != null)
            {
                ExtensionObject[] rolePermissions = attributes[Attributes.RolePermissions].Value as ExtensionObject[];

                if (rolePermissions != null)
                {
                    node.RolePermissions = new RolePermissionTypeCollection();

                    foreach (ExtensionObject rolePermission in rolePermissions)
                    {
                        node.RolePermissions.Add(rolePermission.Body as RolePermissionType);
                    }
                }
            }
            */

            // UserRolePermissions Attribute
            /*
            value = attributes[Attributes.UserRolePermissions];

            if (value != null)
            {
                ExtensionObject[] userRolePermissions = attributes[Attributes.UserRolePermissions].Value as ExtensionObject[];

                if (userRolePermissions != null)
                {
                    node.UserRolePermissions = new RolePermissionTypeCollection();

                    foreach (ExtensionObject rolePermission in userRolePermissions)
                    {
                        node.UserRolePermissions.Add(rolePermission.Body as RolePermissionType);
                    }
                }
            }
            */

            // AccessRestrictions Attribute
            /*
            value = attributes[Attributes.AccessRestrictions];

            if (value != null)
            {
                node.AccessRestrictions = (ushort)attributes[Attributes.AccessRestrictions].GetValue(typeof(ushort));
            }
            */

            return node;
        }

        protected virtual void OnNotified(ClientNotificationEventArgs e)
        {
            SessionNotification?.Invoke(this, e);
        }

        private VariableInfoBase NewVariableInfo(MonitoredItem m, MonitoredItemNotification n)
        {
            var node = (VariableNode)session.ReadNode(m.StartNodeId);
            var type = TypeInfo.GetBuiltInType(node.DataType, session.TypeTree);
            var conf = new VariableConfiguration(node, type);
            var vi = variableInfoManager.NewVariableInfo(conf);
            vi.SetItem(m.StartNodeId, m.DisplayName, m.ClientHandle, n?.Value);
            return vi;
        }

        private void innerWrite(IList<VariableInfoBase> items, Func<VariableInfoBase, object> func)
        {
            try
            {
                var values = new WriteValueCollection();

                foreach (var vi in items)
                {
                    var value = new WriteValue();

                    value.NodeId = vi.NodeId;
                    value.AttributeId = Attributes.Value;
                    value.IndexRange = null;
                    value.Value.StatusCode = StatusCodes.Good;
                    value.Value.ServerTimestamp = DateTime.MinValue;
                    value.Value.SourceTimestamp = DateTime.MinValue;
                    value.Value.Value = func(vi);

                    values.Add(value);
                }

                foreach (WriteValue nodeToWrite in values)
                {
                    NumericRange indexRange;
                    ServiceResult result = NumericRange.Validate(nodeToWrite.IndexRange, out indexRange);

                    if (ServiceResult.IsGood(result) && indexRange != NumericRange.Empty)
                    {
                        // apply the index range.
                        object valueToWrite = nodeToWrite.Value.Value;

                        result = indexRange.ApplyRange(ref valueToWrite);

                        if (ServiceResult.IsGood(result))
                        {
                            nodeToWrite.Value.Value = valueToWrite;
                        }
                    }
                }
                StatusCodeCollection results = null;
                DiagnosticInfoCollection diagnosticInfos = null;

                ResponseHeader responseHeader = session.Write(
                    null,
                    values,
                    out results,
                    out diagnosticInfos);

                ClientBase.ValidateResponse(results, values);
                ClientBase.ValidateDiagnosticInfos(diagnosticInfos, values);

                for (int i = 0; i < values.Count; i++)
                {
                    var conf = items[i].VariableConfiguration;
                    var vi = variableInfoManager.NewVariableInfo(conf);
                    vi.ServerTimestamp = items[i].ServerTimestamp;
                    vi.SourceTimestamp = items[i].SourceTimestamp;
                    vi.StatusCode = results[i];
                    if (StatusCode.IsNotGood(results[i]))
                    {
                        values[i].Value.Value = items[i].DataValue.Value;
                    }
                    vi.SetItem(items[i].NodeId, items[i].DisplayName, items[i].ClientHandle, values[i].Value);
                    var isSelected = items[i].IsSelected;
                    vi.SetPrepareValue(items[i].GetPrepareValue());
                    items[i] = vi;
                    vi.IsSelected = isSelected;
                }
            }
            catch (Exception ex)
            {
                MessagePassing(ex);

                Close();
            }
        }

        private void MessagePassing(Exception ex)
        {
            this.EventAggregator
                .GetEvent<Events.ErrorNotificationEvent>()
                .Publish(new Events.ErrorNotification(ex));
        }

        private void Vi_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "WriteValue")
            {
                var items = new List<VariableInfoBase>() { (VariableInfoBase)sender };

                innerWrite(items, (vi) => vi.DataValue.Value);
            }
        }

        private void validateCerts(Opc.Ua.CertificateValidator sender, Opc.Ua.CertificateValidationEventArgs e)
        {
            e.Accept = true;
        }

        object syncObject = new object();

        private void Session_Notification(Session session, NotificationEventArgs e)
        {
            lock(syncObject)
            {
                System.Diagnostics.Debug.WriteLine("---------------------------------------------------------");
                System.Diagnostics.Debug.WriteLine("----------- (START) Session Notification ----------------");

                try
                {
                    // get the changes.
                    var changes = new List<VariableInfoBase>();

                    var datachanges = e.NotificationMessage.GetDataChanges(false);

                    System.Diagnostics.Debug.WriteLine("Data Changes Count: {0}", datachanges.Count());

                    foreach (var change in datachanges)
                    {
                        var found = subscription.FindItemByClientHandle(change.ClientHandle);
                        if(found == null)
                            System.Diagnostics.Debug.WriteLine("** FindItemByClientHandle: null");
                        else
                            System.Diagnostics.Debug.WriteLine(string.Format("** FindItemByClientHandle: {0}", found.DisplayName));


                        if (found == null)
                            continue;

                        System.Diagnostics.Debug.WriteLine("** (Start) NewVariableInfo");
                        var vi = NewVariableInfo(found, change);
                        System.Diagnostics.Debug.WriteLine("** ( End ) NewVariableInfo");
                        changes.Add(vi);
                    }

                    // check if nothing more to do.
                    if (changes.Count == 0)
                    {
                        return;
                    }

                    OnNotified(new ClientNotificationEventArgs(changes));
                }
                finally
                {
                    System.Diagnostics.Debug.WriteLine("----------- ( END ) Session Notification ----------------");
                    System.Diagnostics.Debug.WriteLine("---------------------------------------------------------");
                }
            }
        }


        #endregion

    }

    public class ClientNotificationEventArgs : EventArgs
    {
        private readonly List<VariableInfoBase> items;
        public ClientNotificationEventArgs(List<VariableInfoBase> items)
        {
            this.items = items;
        }

        public List<VariableInfoBase> Items
        {
            get { return this.items; }
        }
    }

    public delegate void ClientNotificationEventHandler(object sender, ClientNotificationEventArgs e);
}
