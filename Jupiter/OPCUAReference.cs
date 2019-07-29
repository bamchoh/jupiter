using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Opc.Ua;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Data;
using Prism.Events;
using Unity.Attributes;
using Jupiter.Interfaces;

namespace Jupiter
{
    public class OPCUAReference : INotifyPropertyChanged, Interfaces.IReference
    {
        #region Event
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        public IEventAggregator EventAggregator { get; set; }

        public IEventAggregator GetEventAggregator()
        {
            return this.EventAggregator;
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private static readonly OPCUAReference DummyChild = new OPCUAReference();
        private readonly ObservableCollection<OPCUAReference> children;

        public OPCUAReference(Interfaces.IReferenceFetchable client, OPCUAReference parent, IEventAggregator ea)
        {
            this.client = client;

            this.Parent = parent;

            this.EventAggregator = ea;

            this.children = new ObservableCollection<OPCUAReference>();

            BindingOperations.EnableCollectionSynchronization(this.children, new object());

            this.children.Add(DummyChild);

            this.DisplayName = "";
        }

        private OPCUAReference()
        {
        }

        private Interfaces.IReferenceFetchable client;

        public Interfaces.IReference Parent { get; }

        public string DisplayName { get; set; }

        public ExpandedNodeId NodeId { get; set; }
        public NodeClass Type { get; set; }

        public string TypeString
        {
            get
            {
                return Type.ToString();
            }
        }

        public IList Children
        {
            get
            {
                return children;
            }
        }

        private bool isExpanded;
        public bool IsExpanded
        {
            get
            {
                return isExpanded;
            }

            set
            {
                if (isExpanded == value)
                    return;

                if (value == true)
                {
                    isExpanded = this.UpdateChildren();
                } else
                {
                    isExpanded = false;
                }
            }
        }

        private bool isSelected;
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }

            set
            {
                if (isSelected == value)
                    return;

                isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        public void AddChildren(ReferenceDescriptionCollection srcRef)
        {
            foreach (var r in srcRef)
            {
                var child = new OPCUAReference(this.client, this, this.EventAggregator)
                {
                    DisplayName = r.DisplayName.ToString(),
                    NodeId = r.NodeId,
                    Type = r.NodeClass,
                };
                this.Children.Add(child);
            }
        }

        public Interfaces.IReference NewReference(string name)
        {
            return new OPCUAReference(this.client, null, this.EventAggregator) { DisplayName = name };
        }

        private ReferenceDescriptionCollection FetchObjectReferences(ExpandedNodeId enodeid)
        {
            try
            {
                NodeId id;
                if(enodeid == null)
                    id = Objects.ObjectsFolder;
                else
                    id = client.ToNodeId(enodeid);

                var mask = (uint)NodeClass.Object;
                var refs = client.FetchReferences(id);
                client.Browse(id, mask, out refs);
                return refs;
            }
            catch (Exception ex)
            {
                this.EventAggregator
                    .GetEvent<Events.ErrorNotificationEvent>()
                    .Publish(new Events.ErrorNotification(ex));

                return null;
            }
        }

        private bool UpdateChildren()
        {
            var refs = FetchObjectReferences(this.NodeId);
            if (refs == null || refs.Count == 0)
            {
                return false;
            }

            this.Children.Clear();
            this.AddChildren(refs);

            return true;
        }

        public void UpdateReferences()
        {
            var refs = FetchObjectReferences(null);

            if(refs == null)
            {
                return;
            }

            this.Children.Clear();

            this.AddChildren(refs);

            OnPropertyChanged("Children");
        }
    }
}
