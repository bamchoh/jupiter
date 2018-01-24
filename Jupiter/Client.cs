using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace Jupiter
{
    public class Client : BindableBase, Interfaces.IConnection, Interfaces.IReferenceFetchable, Interfaces.INodeInfoGetter, Interfaces.ISubscriptionOperatable, Interfaces.IOneTimeAccessOperator, Interfaces.IVariableInfoManager
    {
        #region Private Fields
        private Session session;
        private Subscription subscription;
        private ApplicationConfiguration config;

        private bool connected;
        #endregion

        #region Public Singleton
        public static Client Instance = new Client();
        #endregion

        #region Public Event
        public event ClientNotificationEventHandler SessionNotification;
        #endregion

        #region Constructor
        private Client()
        {
            Endpoint = "opc.tcp://127.0.0.1:62541";

            config = new ApplicationConfiguration()
            {
                ApplicationName = "Jupiter",
                ApplicationType = ApplicationType.Client,
                ApplicationUri = "urn:" + Utils.GetHostName() + ":bamchoh:Jupiter",
                SecurityConfiguration = new SecurityConfiguration
                {
                    ApplicationCertificate = new CertificateIdentifier
                    {
                        StoreType = "X509Store",
                        StorePath = "CurrentUser\\My",
                        SubjectName = "UA Core Sample Client"
                    },
                    TrustedPeerCertificates = new CertificateTrustList
                    {
                        StoreType = "Directory",
                        StorePath = "OPC Foundation/CertificateStores/UA Applications",
                    },
                    TrustedIssuerCertificates = new CertificateTrustList
                    {
                        StoreType = "Directory",
                        StorePath = "OPC Foundation/CertificateStores/UA Certificate Authorities",
                    },
                    RejectedCertificateStore = new CertificateTrustList
                    {
                        StoreType = "Directory",
                        StorePath = "OPC Foundation/CertificateStores/RejectedCertificates",
                    },
                    NonceLength = 32,
                    AutoAcceptUntrustedCertificates = true
                },
                TransportConfigurations = new TransportConfigurationCollection(),
                TransportQuotas = new TransportQuotas { OperationTimeout = 15000 },
                ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 60000 },
            };

            initialize(config).Wait();

            config.CertificateValidator.CertificateValidation += new CertificateValidationEventHandler(validateCerts);
        }
        #endregion

        #region Properties
        public string Endpoint { get; set; }

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

            var n = session.NodeCache.Find(nodeid) as ILocalNode;

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

                ILocalNode property = session.NodeCache.Find(reference.TargetId) as ILocalNode;

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
            }
        }

        public IList<VariableInfoBase> AddToSubscription(IList objs)
        {
            try
            {
                if (objs == null || objs.Count == 0)
                    return null;

                var viList = NewVariableInfo(objs);

                if (viList.Count == 0)
                    return viList;

                if (session.Subscriptions.Count() == 0)
                {
                    subscription = new Subscription(session.DefaultSubscription) { PublishingInterval = 5000 };
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
                return null;
            }
        }

        public IList<VariableInfoBase> NewVariableInfo(IList objs)
        {
            if (objs == null || objs.Count == 0)
                return null;

            var addList = new List<VariableInfoBase>();
            foreach(Interfaces.IReference obj in objs)
            {
                if (obj.Type != NodeClass.Variable)
                {
                    continue;
                }

                var vi = NewVariableInfo(obj.NodeId);
                addList.Add(vi);
            }
            return addList;
        }

        public ReferenceDescriptionCollection FetchRootReferences()
        {
            try
            {
                var refs = session.FetchReferences(ObjectIds.ObjectsFolder);
                byte[] continuationPoint;

                session.Browse(
                    null,
                    null,
                    ObjectIds.ObjectsFolder,
                    0u,
                    BrowseDirection.Forward,
                    ReferenceTypeIds.HierarchicalReferences,
                    true,
                    (uint)NodeClass.Object | (uint)NodeClass.Method,
                    out continuationPoint,
                    out refs);

                return refs;
            }
            catch (Exception ex)
            {
                MessagePassing(ex);
                Close();
                return null;
            }
        }

        public ReferenceDescriptionCollection FetchReferences(ExpandedNodeId nodeid, bool onlyVariable = false)
        {
            try
            {
                ReferenceDescriptionCollection refs;
                byte[] continuationPoint;

                uint mask;
                if (onlyVariable)
                {
                    mask = (uint)NodeClass.Variable;
                }
                else
                {
                    mask = (uint)NodeClass.Object | (uint)NodeClass.Method;
                }

                session.Browse(
                    null,
                    null,
                    ExpandedNodeId.ToNodeId(nodeid, session.NamespaceUris),
                    0u,
                    BrowseDirection.Forward,
                    ReferenceTypeIds.HierarchicalReferences,
                    true,
                    mask,
                    out continuationPoint,
                    out refs);

                while (continuationPoint != null) {
                    byte[] revisedContinuationPoint;
                    ReferenceDescriptionCollection additionalDescription;

                    session.BrowseNext(
                        null,
                        false,
                        continuationPoint,
                        out revisedContinuationPoint,
                        out additionalDescription);

                    continuationPoint = revisedContinuationPoint;

                    refs.AddRange(additionalDescription);
                }

                return refs;
            }
            catch (Exception ex)
            {
                MessagePassing(ex);
                Close();
                return null;
            }
        }

        public async Task CreateSession()
        {
            var selectedEndpoint = CoreClientUtils.SelectEndpoint(Endpoint, false, 15000);
            var endpointConfiguration = EndpointConfiguration.Create(config);
            var endpoint = new ConfiguredEndpoint(null, selectedEndpoint, endpointConfiguration);
            session = await Session.Create(
                config, 
                endpoint, 
                false, 
                "Jupiter", 
                60000,
                new UserIdentity(new AnonymousIdentityToken()),
                null);

            session.PublishError += Session_PublishError;
            Connected = session.Connected;
        }

        private void Session_PublishError(Session session, PublishErrorEventArgs e)
        {
            session.Close();
            Connected = session.Connected;
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

        public void Read(IList<VariableInfoBase> items)
        {
            try
            {
                var itemsToRead = new ReadValueIdCollection();

                foreach (var vi in items)
                {
                    var rv = new ReadValueId()
                    {
                        NodeId = vi.NodeId,
                        AttributeId = Attributes.Value,
                        IndexRange = null,
                        DataEncoding = null,
                    };

                    itemsToRead.Add(rv);
                }

                DataValueCollection values;
                DiagnosticInfoCollection diagnosticInfos;

                ResponseHeader responseHeader = session.Read(
                    null,
                    0,
                    TimestampsToReturn.Both,
                    itemsToRead,
                    out values,
                    out diagnosticInfos);

                ClientBase.ValidateResponse(values, itemsToRead);
                ClientBase.ValidateDiagnosticInfos(diagnosticInfos, itemsToRead);

                for (int i = 0; i < values.Count; i++)
                {
                    var vi = NewVariableInfo(items[i].NodeId);
                    vi.SetItem(items[i].NodeId, items[i].ClientHandle, values[i]);
                    var isSelected = items[i].IsSelected;
                    items[i] = vi;
                    vi.IsSelected = isSelected;
                }

                return;
            }
            catch (Exception ex)
            {
                MessagePassing(ex);
                Close();
                return;
            }
        }

        public void Write(IList<VariableInfoBase> items)
        {
            Func<VariableInfoBase, object> func = (vi) => vi.GetPrepareValue();

            innerWrite(items, func);
        }

        protected virtual void OnNotified(ClientNotificationEventArgs e)
        {
            SessionNotification?.Invoke(this, e);
        }

        private async Task initialize(ApplicationConfiguration config)
        {
            await config.Validate(ApplicationType.Client);
        }

        private VariableInfoBase NewVariableInfo(MonitoredItem m, MonitoredItemNotification n)
        {
            var vi = NewVariableInfo(m.StartNodeId);
            vi.SetItem(m.StartNodeId, m.ClientHandle, n?.Value);
            return vi;
        }

        public VariableInfoBase NewVariableInfo(ExpandedNodeId id)
        {
            var node = session.NodeCache.Find(id) as VariableNode;
            BuiltInType builtinType = TypeInfo.GetBuiltInType(node.DataType, session.TypeTree);
            var type = TypeInfo.GetSystemType(builtinType, node.ValueRank);

            VariableInfoBase vi;
            switch (builtinType)
            {
                case BuiltInType.Boolean:
                    vi = new BooleanVariableInfo();
                    break;
                case BuiltInType.SByte:
                    vi = new SByteVariableInfo();
                    break;
                case BuiltInType.Byte:
                    vi = new ByteVariableInfo();
                    break;
                case BuiltInType.Int16:
                    vi = new Int16VariableInfo();
                    break;
                case BuiltInType.UInt16:
                    vi = new UInt16VariableInfo();
                    break;
                case BuiltInType.Int32:
                    vi = new Int32VariableInfo();
                    break;
                case BuiltInType.UInt32:
                    vi = new UInt32VariableInfo();
                    break;
                case BuiltInType.Int64:
                    vi = new Int64VariableInfo();
                    break;
                case BuiltInType.UInt64:
                    vi = new UInt64VariableInfo();
                    break;
                case BuiltInType.Float:
                    vi = new FloatVariableInfo();
                    break;
                case BuiltInType.Double:
                    vi = new DoubleVariableInfo();
                    break;
                case BuiltInType.String:
                    vi = new StringVariableInfo();
                    break;
                case BuiltInType.DateTime:
                    vi = new DateTimeVariableInfo();
                    break;
                default:
                    vi = new VariantVariableInfo();
                    break;
            }

            vi.NodeId = node.NodeId;
            vi.Type = builtinType.ToString();
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
                    var vi = NewVariableInfo(items[i].NodeId);
                    values[i].Value.ServerTimestamp = items[i].ServerTimestamp;
                    values[i].Value.SourceTimestamp = items[i].SourceTimestamp;
                    values[i].Value.StatusCode = results[i];
                    vi.SetItem(items[i].NodeId, items[i].ClientHandle, values[i].Value);
                    var isSelected = items[i].IsSelected;
                    items[i] = vi;
                    vi.IsSelected = isSelected;
                }
            }
            catch (Exception ex)
            {
                MessagePassing(ex);
            }
        }

        private void MessagePassing(Exception ex)
        {
            var msgbox = Commands.ShowMessageCommand.Command;
            if (ex.InnerException != null)
            {
                msgbox.Execute(ex.InnerException.Message);
            }
            else
            {
                msgbox.Execute(ex.Message);
            }
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

        private void Session_Notification(Session session, NotificationEventArgs e)
        {
            // get the changes.
            var changes = new List<VariableInfoBase>();

            foreach (var change in e.NotificationMessage.GetDataChanges(false))
            {
                var found = subscription.FindItemByClientHandle(change.ClientHandle);
                if (found == null)
                    continue;

                var vi = NewVariableInfo(found, change);
                changes.Add(vi);
            }

            // check if nothing more to do.
            if (changes.Count == 0)
            {
                return;
            }

            OnNotified(new ClientNotificationEventArgs(changes));
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

    public class NodeInfo
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

}
