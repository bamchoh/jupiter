using System.ComponentModel;
using System.Collections.ObjectModel;
using Opc.Ua;
using System.Collections;

namespace Jupiter
{
    public class OPCUAReference : INotifyPropertyChanged, Interfaces.IReference
    {
        #region Event
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private static readonly OPCUAReference DummyChild = new OPCUAReference();
        private readonly ObservableCollection<OPCUAReference> children;
        private readonly Interfaces.IReference parent;

        public OPCUAReference(Interfaces.IReferenceFetchable client, OPCUAReference parent)
        {
            this.client = client;

            this.parent = parent;

            this.children = new ObservableCollection<OPCUAReference>();

            this.children.Add(DummyChild);

            this.DisplayName = "";
        }

        private OPCUAReference()
        {
        }

        private Interfaces.IReferenceFetchable client;

        public Interfaces.IReference Parent
        {
            get
            {
                return parent;
            }
        }

        public string DisplayName { get; set; }

        public ExpandedNodeId NodeId { get; set; }

        public NodeClass Type { get; set; }

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

                isExpanded = value;

                if (isExpanded == true)
                {
                    this.UpdateChildren();
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
                var child = new OPCUAReference(this.client, this)
                {
                    DisplayName = r.DisplayName.ToString(),
                    NodeId = r.NodeId,
                    Type = r.NodeClass,
                };
                this.Children.Add(child);
            }
        }

        private void UpdateChildren()
        {
            var refs = client.FetchReferences(this.NodeId);

            this.Children.Clear();
            if (refs != null || refs.Count != 0)
            {
                this.AddChildren(refs);
            }
        }

        public void UpdateReferences()
        {
            var refs = client.FetchRootReferences();

            this.Children.Clear();

            this.AddChildren(refs);

            OnPropertyChanged("Children");
        }
    }
}
