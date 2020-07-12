using System;
using Opc.Ua;
using Opc.Ua.Client;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections;

namespace Jupiter
{

    public class VariableConfiguration : Interfaces.IVariableConfiguration
    {
        public VariableConfiguration(NodeId id, string displayName, NodeClass nodeClass, BuiltInType builtInType)
        {
            this.displayName = displayName;
            this.nodeClass = nodeClass;
            this.builtInType = builtInType;
            this.id = id;
        }

        string displayName;
        public string DisplayName {
            get
            {
                return displayName;
            }

            set { /* NOPE */ }
        }

        NodeClass nodeClass;
        public NodeClass NodeClass {
            get
            {
                return nodeClass;
            }

            set { /* NOPE */ }
        }

        BuiltInType builtInType;
        public BuiltInType BuiltInType()
        {
            return builtInType;
        }

        NodeId id;
        public NodeId VariableNodeId()
        {
            return id;
        }
    }

    public class VariableInfo : Interfaces.IVariableInfoManager
    {
        public IList<VariableInfoBase> GenerateVariableInfoList(IList objs)
        {
            if (objs == null || objs.Count == 0)
                return null;

            var addList = new List<VariableInfoBase>();
            foreach (Interfaces.IVariableConfiguration obj in objs)
            {
                if (obj.NodeClass != NodeClass.Variable)
                {
                    continue;
                }

                var vi = NewVariableInfo(obj);
                addList.Add(vi);
            }
            return addList;
        }

        public VariableInfoBase NewVariableInfo(Interfaces.IVariableConfiguration conf)
        {
            VariableInfoBase vi;
            switch (conf.BuiltInType())
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

            vi.VariableConfiguration = conf;
            vi.NodeId = conf.VariableNodeId();
            vi.DisplayName = conf.DisplayName;
            vi.Type = conf.BuiltInType().ToString();
            return vi;
        }
    }

    public class BooleanVariableInfo : VariableInfoBase
    {
        public bool Value
        {
            get
            {
                if (DataValue.Value == null)
                    return false;
                else
                    return (bool)DataValue.Value;
            }

            // bool は Editモードに入らない為、Value側のsetでWriteValueのイベントをraiseしてあげる必要がある
            // Value自身のイベントは親クラスのSetItemでのみ発生する為問題なし
            // だが、この方法は暗にViewの実装を意識しているため、良くない。
            // TODO : 他のデータタイプとの統一を図る
            set
            {
                DataValue.Value = value;
                this.SetProperty("WriteValue");
            }
        }

        public bool PrepareValue { get; set; }
        public override object GetPrepareValue()
        {
            return PrepareValue;
        }

        public override void SetPrepareValue(object obj)
        {
            PrepareValue = (bool)obj;
        }
    }

    public class SByteVariableInfo : VariableInfoBase
    {
        public sbyte Value
        {
            get
            {
                if (DataValue.Value == null)
                    return 0;
                else
                    return (sbyte)DataValue.Value;
            }
        }

        public sbyte WriteValue
        {
            get
            {
                if (DataValue.Value == null)
                    return 0;
                else
                    return (sbyte)DataValue.Value;
            }

            set
            {
                DataValue.Value = value;
                this.SetProperty("WriteValue");
            }
        }

        public sbyte PrepareValue { get; set; }
        public override object GetPrepareValue()
        {
            return PrepareValue;
        }

        public override void SetPrepareValue(object obj)
        {
            PrepareValue = (sbyte)obj;
        }
    }

    public class ByteVariableInfo : VariableInfoBase
    {
        public byte Value
        {
            get
            {
                if (DataValue.Value == null)
                    return 0;
                else
                    return (byte)DataValue.Value;
            }
        }

        public byte WriteValue
        {
            get
            {
                if (DataValue.Value == null)
                    return 0;
                else
                    return (byte)DataValue.Value;
            }

            set
            {
                DataValue.Value = value;
                this.SetProperty("WriteValue");
            }
        }

        public byte PrepareValue { get; set; }
        public override object GetPrepareValue()
        {
            return PrepareValue;
        }

        public override void SetPrepareValue(object obj)
        {
            PrepareValue = (byte)obj;
        }
    }

    public class UInt16VariableInfo : VariableInfoBase
    {
        public UInt16 Value
        {
            get
            {
                if (DataValue.Value == null)
                    return 0;
                else
                    return (UInt16)DataValue.Value;
            }
        }

        public UInt16 WriteValue
        {
            get
            {
                if (DataValue.Value == null)
                    return 0;
                else
                    return (UInt16)DataValue.Value;
            }

            set
            {
                DataValue.Value = value;
                this.SetProperty("WriteValue");
            }
        }

        public UInt16 PrepareValue { get; set; }
        public override object GetPrepareValue()
        {
            return PrepareValue;
        }

        public override void SetPrepareValue(object obj)
        {
            PrepareValue = (UInt16)obj;
        }
    }

    public class Int16VariableInfo : VariableInfoBase
    {
        public Int16 Value
        {
            get
            {
                if (DataValue.Value == null)
                    return 0;
                else
                    return (Int16)DataValue.Value;
            }
        }

        public Int16 WriteValue
        {
            get
            {
                if (DataValue.Value == null)
                    return 0;
                else
                    return (Int16)DataValue.Value;
            }

            set
            {
                DataValue.Value = value;
                this.SetProperty("WriteValue");
            }
        }

        public Int16 PrepareValue { get; set; }
        public override object GetPrepareValue()
        {
            return PrepareValue;
        }

        public override void SetPrepareValue(object obj)
        {
            PrepareValue = (Int16)obj;
        }
    }

    public class UInt32VariableInfo : VariableInfoBase
    {
        public UInt32 Value
        {
            get
            {
                if (DataValue.Value == null)
                    return 0;
                else
                    return (UInt32)DataValue.Value;
            }
        }

        public UInt32 WriteValue
        {
            get
            {
                if (DataValue.Value == null)
                    return 0;
                else
                    return (UInt32)DataValue.Value;
            }

            set
            {
                DataValue.Value = value;
                this.SetProperty("WriteValue");
            }
        }

        public UInt32 PrepareValue { get; set; }
        public override object GetPrepareValue()
        {
            return PrepareValue;
        }

        public override void SetPrepareValue(object obj)
        {
            PrepareValue = (UInt32)obj;
        }
    }

    public class Int32VariableInfo : VariableInfoBase
    {
        public Int32 Value
        {
            get
            {
                if (DataValue.Value == null)
                    return 0;
                else
                    return (Int32)DataValue.Value;
            }
        }

        public Int32 WriteValue
        {
            get
            {
                if (DataValue.Value == null)
                    return 0;
                else
                    return (Int32)DataValue.Value;
            }

            set
            {
                DataValue.Value = value;
                this.SetProperty("WriteValue");
            }
        }

        public Int32 PrepareValue { get; set; }
        public override object GetPrepareValue()
        {
            return PrepareValue;
        }

        public override void SetPrepareValue(object obj)
        {
            PrepareValue = (Int32)obj;
        }
    }

    public class Int64VariableInfo : VariableInfoBase
    {
        public Int64 Value
        {
            get
            {
                if (DataValue.Value == null)
                    return 0;
                else
                    return (Int64)DataValue.Value;
            }
        }

        public Int64 WriteValue
        {
            get
            {
                if (DataValue.Value == null)
                    return 0;
                else
                    return (Int64)DataValue.Value;
            }

            set
            {
                DataValue.Value = value;
                this.SetProperty("WriteValue");
            }
        }

        public Int64 PrepareValue { get; set; }
        public override object GetPrepareValue()
        {
            return PrepareValue;
        }

        public override void SetPrepareValue(object obj)
        {
            PrepareValue = (Int64)obj;
        }
    }

    public class UInt64VariableInfo : VariableInfoBase
    {
        public UInt64 Value
        {
            get
            {
                if (DataValue.Value == null)
                    return 0;
                else
                    return (UInt64)DataValue.Value;
            }
        }

        public UInt64 WriteValue
        {
            get
            {
                if (DataValue.Value == null)
                    return 0;
                else
                    return (UInt64)DataValue.Value;
            }

            set
            {
                DataValue.Value = value;
                this.SetProperty("WriteValue");
            }
        }

        public UInt64 PrepareValue { get; set; }
        public override object GetPrepareValue()
        {
            return PrepareValue;
        }

        public override void SetPrepareValue(object obj)
        {
            PrepareValue = (UInt64)obj;
        }
    }

    public class FloatVariableInfo : VariableInfoBase
    {
        public float Value
        {
            get
            {
                if (DataValue.Value == null)
                    return 0;
                else
                    return (float)DataValue.Value;
            }
        }

        public float WriteValue
        {
            get
            {
                if (DataValue.Value == null)
                    return 0;
                else
                    return (float)DataValue.Value;
            }

            set
            {
                DataValue.Value = value;
                this.SetProperty("WriteValue");
            }
        }

        public float PrepareValue { get; set; }
        public override object GetPrepareValue()
        {
            return PrepareValue;
        }

        public override void SetPrepareValue(object obj)
        {
            PrepareValue = (float)obj;
        }
    }

    public class DoubleVariableInfo : VariableInfoBase
    {
        public Double Value
        {
            get
            {
                if (DataValue.Value == null)
                    return 0;
                else
                    return (Double)DataValue.Value;
            }
        }

        public Double WriteValue
        {
            get
            {
                if (DataValue.Value == null)
                    return 0;
                else
                    return (Double)DataValue.Value;
            }

            set
            {
                DataValue.Value = value;
                this.SetProperty("WriteValue");
            }
        }

        public Double PrepareValue { get; set; }
        public override object GetPrepareValue()
        {
            return PrepareValue;
        }

        public override void SetPrepareValue(object obj)
        {
            PrepareValue = (Double)obj;
        }
    }

    public class StringVariableInfo : VariableInfoBase
    {
        public String Value
        {
            get
            {
                if (DataValue.Value == null)
                {
                    return null;
                }
                else
                {
                    if(DataValue.Value is String[])
                    {
                        var ss = "";
                        foreach(var s in (String[])DataValue.Value)
                        {
                            ss += s + ";";
                        }
                        return ss;
                    }
                    else
                    {
                        return DataValue.Value.ToString();
                    }
                }
            }
        }

        public String WriteValue
        {
            get
            {
                if (DataValue.Value == null)
                    return null;
                else
                    return (String)DataValue.Value;
            }

            set
            {
                DataValue.Value = value;
                this.SetProperty("WriteValue");
            }
        }

        public String PrepareValue { get; set; }
        public override object GetPrepareValue()
        {
            return PrepareValue;
        }

        public override void SetPrepareValue(object obj)
        {
            PrepareValue = (String)obj;
        }
    }

    public class DateTimeVariableInfo : VariableInfoBase
    {
        public DateTime Value
        {
            get
            {
                if (DataValue.Value == null)
                    return DateTime.Now;
                else
                    return (DateTime)DataValue.Value;
            }
        }

        public DateTime WriteValue
        {
            get
            {
                if (DataValue.Value == null)
                    return DateTime.Now;
                else
                    return (DateTime)DataValue.Value;
            }

            set
            {
                DataValue.Value = value;
                this.SetProperty("WriteValue");
            }
        }

        public DateTime PrepareValue { get; set; }
        public override object GetPrepareValue()
        {
            return PrepareValue;
        }

        public override void SetPrepareValue(object obj)
        {
            PrepareValue = (DateTime)obj;
        }
    }

    public class VariantVariableInfo : VariableInfoBase
    {
        public object Value
        {
            get
            {
                if (DataValue.Value == null)
                    return null;
                else
                    return DataValue.Value;
            }
        }

        public object WriteValue
        {
            get
            {
                return DataValue.Value;
            }

            set
            {
                DataValue.Value = value;
                this.SetProperty("WriteValue");
            }
        }

        public object PrepareValue { get; set; }

        public override object GetPrepareValue()
        {
            return PrepareValue;
        }

        public override void SetPrepareValue(object obj)
        {
            PrepareValue = obj;
        }
    }

    public abstract class VariableInfoBase : INotifyPropertyChanged
    {
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
                this.SetProperty("IsSelected");
            }
        }

        public VariableInfoBase()
        {
            this.DataValue = new DataValue(StatusCodes.UncertainInitialValue);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void SetProperty(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Type { get; set; }

        public Interfaces.IVariableConfiguration VariableConfiguration { get; set; }

        private NodeId nodeId;
        public NodeId NodeId
        {
            get
            {
                return nodeId;
            }

            set
            {
                if (nodeId == value)
                    return;

                nodeId = value;
                this.SetProperty("NodeId");
            }
        }

        private string displayname;
        public string DisplayName
        {
            get
            {
                return displayname;
            }

            set
            {
                if (displayname == value)
                    return;

                displayname = value;
                this.SetProperty("DisplayName");
            }
        }

        public StatusCode StatusCode
        {
            get
            {
                return DataValue.StatusCode;
            }

            set
            {
                if (DataValue != null && DataValue.StatusCode != value)
                    DataValue.StatusCode = value;
            }
        }

        public DateTime ServerTimestamp
        {
            get
            {
                return DataValue.ServerTimestamp;
            }

            set
            {
                if (DataValue != null && DataValue.ServerTimestamp != value)
                    DataValue.ServerTimestamp = value;
            }
        }

        public DateTime SourceTimestamp
        {
            get
            {
                return DataValue.SourceTimestamp;
            }

            set
            {
                if (DataValue != null && DataValue.SourceTimestamp != value)
                    DataValue.SourceTimestamp = value;
            }
        }

        public uint ClientHandle;

        public DataValue DataValue;

        public void Update(VariableInfoBase vi)
        {
            this.DataValue = vi.DataValue;
            this.Type = vi.DataValue.WrappedValue.TypeInfo.BuiltInType.ToString();
            this.SetProperty("Type");
            this.SetProperty("Value");
            this.SetProperty("StatusCode");
            this.SetProperty("ServerTimestamp");
            this.SetProperty("SourceTimestamp");
        }

        public void SetItem(NodeId nodeid, string displayname, uint handle, DataValue dv)
        {
            if(nodeid != null)
            {
                this.NodeId = nodeid;
            }

            this.DisplayName = displayname;

            if(handle != 0)
            {
                this.ClientHandle = handle;
            }

            if (dv == null)
                return;

            this.DataValue = dv;
            this.SetProperty("Value");
            this.SetProperty("StatusCode");
            this.SetProperty("ServerTimestamp");
            this.SetProperty("SourceTimestamp");
        }

        public abstract object GetPrepareValue();

        public abstract void SetPrepareValue(object obj);
    }
}
