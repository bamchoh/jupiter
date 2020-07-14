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
using System.CodeDom;
using System.Net;

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

                for (int i=0;i<endpointCollection.Count();i++)
                {
                    var got = endpointCollection[i];

                    endpoints.Add(got);
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

            var securityList = new SortedDictionary<string, List<string>>();
            var endpointIndex = new SortedDictionary<string, List<int>>();
            for(int i = 0;i<endpoints.Count;i++)
            {
                var ed = endpoints[i];
                var discoveryURL = "";
                if (ed.Server.DiscoveryUrls.Count() != 0)
                    discoveryURL = ed.Server.DiscoveryUrls[0];

                var key = string.Format("{0} - {1}",
                    ed.EndpointUrl,
                    ed.Server.ApplicationName);

                if (!securityList.ContainsKey(key))
                    securityList[key] = new List<string>();

                securityList[key].Add(string.Format("{0} - {1}",
                    Opc.Ua.SecurityPolicies.GetDisplayName(ed.SecurityPolicyUri),
                    ed.SecurityMode));

                if (!endpointIndex.ContainsKey(key))
                    endpointIndex[key] = new List<int>();

                endpointIndex[key].Add(i);
            }

            var sem = new SemaphoreSlim(1, 1);
            var result = new Events.NowLoading()
            {
                SecurityList = securityList,
                Endpoints = securityList.Keys.ToList(),
                UserName = "",
                Password = "",
                Semaphore = sem,
            };
            this.EventAggregator
                .GetEvent<Events.NowLoadingEvent>()
                .Publish(result);
            await sem.WaitAsync();

            System.Diagnostics.Trace.WriteLine("--- endpoints ---");
            foreach(var ed in endpoints)
            {
                var key = string.Format("{0} - {1} - {2} - {3}",
                    ed.EndpointUrl,
                    ed.Server.ApplicationName,
                    Opc.Ua.SecurityPolicies.GetDisplayName(ed.SecurityPolicyUri),
                    ed.SecurityMode);

                System.Diagnostics.Trace.WriteLine(key);
            }

            System.Diagnostics.Trace.WriteLine("--- security lists ---");
            foreach(var s in securityList)
            {
                System.Diagnostics.Trace.WriteLine(string.Format("--- key: {0}", s.Key));
                foreach(var v in s.Value)
                {
                    System.Diagnostics.Trace.WriteLine(string.Format("---   val: {0}", v));
                }
            }

            try
            {
                var k = result.SelectedItem;
                var i = endpointIndex[k][result.SelectedIndex];

                System.Diagnostics.Trace.WriteLine("--- selected item ---");
                System.Diagnostics.Trace.WriteLine(string.Format("---   {0}", k));

                System.Diagnostics.Trace.WriteLine("--- selected index ---");
                System.Diagnostics.Trace.WriteLine(string.Format("---   {0}", result.SelectedIndex));

                System.Diagnostics.Trace.WriteLine("--- index ---");
                System.Diagnostics.Trace.WriteLine(string.Format("---   {0}", i));

                System.Diagnostics.Trace.WriteLine("--- selected endpoint ---");
                System.Diagnostics.Trace.WriteLine(string.Format("{0} - {1} - {2} - {3}",
                    endpoints[i].EndpointUrl,
                    endpoints[i].Server.ApplicationName,
                    Opc.Ua.SecurityPolicies.GetDisplayName(endpoints[i].SecurityPolicyUri),
                    endpoints[i].SecurityMode));

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

        public ObservableCollection<VariableConfiguration> FetchVariableReferences(ExpandedNodeId expandedNodeId)
        {
            var id = this.ToNodeId(expandedNodeId);

            // TODO
            // Attributes
            // https://github.com/OPCFoundation/UA-.NETStandard/blob/1.4.359.31/SampleApplications/Samples/ClientControls.Net4/Common/Client/AttrributesListViewCtrl.cs#L107
            // TreeView(References)
            // https://github.com/OPCFoundation/UA-.NETStandard/blob/1.4.359.31/SampleApplications/Samples/ClientControls.Net4/Common/Client/BrowseTreeViewCtrl.cs#L266
            // ここを参考に書き直す。

            var mask = (uint)NodeClass.Variable;
            ReferenceDescriptionCollection refs;
            this.Browse(id, mask, out refs);

            List<BuiltInType> types;
            this.ReadBuiltInType(refs, out types);

            var varConfigurations = new ObservableCollection<VariableConfiguration>();

            for(int i=0;i<refs.Count;i++)
            {
                var r = refs[i];
                var t = types[i];
                varConfigurations.Add(new VariableConfiguration(this.ToNodeId(r.NodeId), r.DisplayName.Text, r.NodeClass, t));
            }

            return varConfigurations;
        }

        private void ReadBuiltInType(ReferenceDescriptionCollection refs, out List<BuiltInType> types)
        {
            if (refs.Count == 0)
            {
                types = new List<BuiltInType>();
                return;
            }

            var nodesToRead = new ReadValueIdCollection();
            foreach (var r in refs)
            {
                var nodeToRead = new ReadValueId();
                nodeToRead.NodeId = this.ToNodeId(r.NodeId);
                nodeToRead.AttributeId = Attributes.Value;
                nodesToRead.Add(nodeToRead);
            }

            DataValueCollection results = null;
            DiagnosticInfoCollection diagnosticInfos = null;

            session.Read(
                null,
                0,
                TimestampsToReturn.Neither,
                nodesToRead,
                out results,
                out diagnosticInfos);

            ClientBase.ValidateResponse(results, nodesToRead);
            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, nodesToRead);

            types = new List<BuiltInType>();
            foreach(var r in results)
            {
                types.Add(r.WrappedValue.TypeInfo.BuiltInType);
            }
        }

        protected virtual void OnNotified(ClientNotificationEventArgs e)
        {
            SessionNotification?.Invoke(this, e);
        }

        private VariableInfoBase NewVariableInfo(MonitoredItem m, MonitoredItemNotification n)
        {
            var builtInType = n.Value.WrappedValue.TypeInfo.BuiltInType;
            var conf = new VariableConfiguration(m.StartNodeId, m.DisplayName, m.NodeClass, builtInType);
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
