using System;
using Opc.Ua;
using Opc.Ua.Client;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Prism.Mvvm;
using Prism.Commands;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using Jupiter.Converter;
using System.Windows;
using System.Runtime.Serialization;
using Microsoft.SqlServer.Server;
using System.Windows.Forms;
using System.Globalization;

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

    public interface IWrappedValueConverter
    {
        object Convert();

        object ConvertBack(object value);
    }

    public class WrappedValue<T> : BindableBase, IWrappedValueConverter
    {
        public WrappedValue(T initValue, string format, Func<object, string, string> convFn, Func<object, string, object> convBackFn)
        {
            this.Value = initValue;
            this.Format = format;
            this.convFn = convFn;
            this.convBackFn = convBackFn;
        }

        public T Value { get; set; }

        public string Format { get; set; }

        private Func<object, string, string> convFn;
        public object Convert()
        {
            return convFn(Value, Format);
        }

        private Func<object, string, object> convBackFn;
        public object ConvertBack(object value)
        {
            return convBackFn(value, Format);
        }
    }

    public class BooleanVariableInfo : VariableInfoBase
    {
        public BooleanVariableInfo()
        {
            Formats = new List<string>
            {
                FormatType.BOOL,
            };

            FormatSelectedItem = FormatType.BOOL;
        }

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
                this.RaisePropertyChanged("WriteValue");
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
        public SByteVariableInfo()
        {
            Formats = new List<string>
            {
                FormatType.DEC,
                FormatType.HEX,
                FormatType.OCT,
            };

            FormatSelectedItem = FormatType.DEC;

            _value = NewValue(0, FormatSelectedItem);

            PrepareValue = NewValue(0, FormatSelectedItem);
        }

        private WrappedValue<sbyte> _value;
        public WrappedValue<sbyte> Value
        {
            get { return _value; }
        }

        public WrappedValue<sbyte> WriteValue
        {
            get { return Value; }

            set
            {
                DataValue.Value = value.Value;
                this.SetProperty(ref this._value, value);
            }
        }

        public WrappedValue<sbyte> PrepareValue { get; set; }

        public override object GetPrepareValue()
        {
            return PrepareValue.Value;
        }

        public override void SetPrepareValue(object obj)
        {
            PrepareValue.Value = (sbyte)obj;
        }

        public override void UpdateWrappedValue()
        {
            if (_value == null)
                return;

            if (DataValue.Value != null)
            {
                _value.Value = (sbyte)DataValue.Value;
            }
            _value.Format = FormatSelectedItem;
            PrepareValue.Format = FormatSelectedItem;
        }

        public static WrappedValue<sbyte> NewValue(sbyte newValue, string format)
        {
            return new WrappedValue<sbyte>(newValue, format, ConvertFn, ConvertBackFn);
        }

        private static string ConvertFn(object value, string format)
        {
            switch (format)
            {
                case FormatType.DEC:
                    return Convert.ToString((sbyte)value, 10);
                case FormatType.HEX:
                    return ((sbyte)value).ToString("X2");
                case FormatType.OCT:
                    var convVal = (byte)((0x7F & (sbyte)value) + (0x80 & (sbyte)value));
                    return Convert.ToString(convVal, 8);
            }
            return value.ToString();
        }

        private static object ConvertBackFn(object value, string format)
        {
            var inputString = value as string;
            switch (format)
            {
                case FormatType.DEC:
                    return NewValue(System.Convert.ToSByte(inputString, 10), format);
                case FormatType.HEX:
                    return NewValue(System.Convert.ToSByte(inputString, 16), format);
                case FormatType.OCT:
                    return NewValue(System.Convert.ToSByte(inputString, 8), format);
            }
            return value.ToString();
        }
    }

    public class ByteVariableInfo : VariableInfoBase
    {
        public ByteVariableInfo()
        {
            Formats = new List<string>
            {
                FormatType.DEC,
                FormatType.HEX,
                FormatType.OCT,
            };

            FormatSelectedItem = FormatType.DEC;

            _value = NewValue(0, FormatSelectedItem);

            PrepareValue = NewValue(0, FormatSelectedItem);
        }

        private WrappedValue<byte> _value;
        public WrappedValue<byte> Value
        {
            get { return _value; }
        }

        public WrappedValue<byte> WriteValue
        {
            get { return Value; }

            set
            {
                DataValue.Value = value.Value;
                this.SetProperty(ref this._value, value);
            }
        }

        public WrappedValue<byte> PrepareValue { get; set; }

        public override object GetPrepareValue()
        {
            return PrepareValue.Value;
        }

        public override void SetPrepareValue(object obj)
        {
            PrepareValue.Value = (byte)obj;
        }

        public override void UpdateWrappedValue()
        {
            if (_value == null)
                return;

            if(DataValue.Value != null)
            {
                _value.Value = (byte)DataValue.Value;
            }
            _value.Format = FormatSelectedItem;
            PrepareValue.Format = FormatSelectedItem;
        }

        public static WrappedValue<byte> NewValue(byte newValue, string format)
        {
            return new WrappedValue<byte>(newValue, format, ConvertFn, ConvertBackFn);
        }

        private static string ConvertFn(object value, string format)
        {
            switch (format)
            {
                case FormatType.DEC:
                    return Convert.ToString((byte)value, 10);
                case FormatType.HEX:
                    return ((byte)value).ToString("X2");
                case FormatType.OCT:
                    return Convert.ToString((byte)value, 8);
            }
            return value.ToString();
        }

        private static object ConvertBackFn(object value, string format)
        {
            var inputString = value as string;
            switch (format)
            {
                case FormatType.DEC:
                    return NewValue(System.Convert.ToByte(inputString, 10), format);
                case FormatType.HEX:
                    return NewValue(System.Convert.ToByte(inputString, 16), format);
                case FormatType.OCT:
                    return NewValue(System.Convert.ToByte(inputString, 8), format);
            }
            return value.ToString();
        }
    }

    public class UInt16VariableInfo : VariableInfoBase
    {
        public UInt16VariableInfo()
        {
            Formats = new List<string>
            {
                FormatType.DEC,
                FormatType.HEX,
                FormatType.OCT,
            };

            FormatSelectedItem = FormatType.DEC;

            _value = NewValue(0, FormatSelectedItem);

            PrepareValue = NewValue(0, FormatSelectedItem);
        }

        private WrappedValue<UInt16> _value;
        public WrappedValue<UInt16> Value
        {
            get { return _value; }
        }

        public WrappedValue<UInt16> WriteValue
        {
            get { return Value; }

            set
            {
                DataValue.Value = value.Value;
                this.SetProperty(ref this._value, value);
            }
        }

        public WrappedValue<UInt16> PrepareValue { get; set; }

        public override object GetPrepareValue()
        {
            return PrepareValue.Value;
        }

        public override void SetPrepareValue(object obj)
        {
            PrepareValue.Value = (UInt16)obj;
        }

        public override void UpdateWrappedValue()
        {
            if (_value == null)
                return;

            if (DataValue.Value != null)
            {
                _value.Value = (UInt16)DataValue.Value;
            }
            _value.Format = FormatSelectedItem;
            PrepareValue.Format = FormatSelectedItem;
        }

        public static WrappedValue<UInt16> NewValue(UInt16 newValue, string format)
        {
            return new WrappedValue<UInt16>(newValue, format, ConvertFn, ConvertBackFn);
        }

        private static string ConvertFn(object value, string format)
        {
            switch (format)
            {
                case FormatType.DEC:
                    return Convert.ToString((UInt16)value, 10);
                case FormatType.HEX:
                    return ((UInt16)value).ToString("X4");
                case FormatType.OCT:
                    return Convert.ToString((UInt16)value, 8);
            }
            return value.ToString();
        }

        private static object ConvertBackFn(object value, string format)
        {
            var inputString = value as string;
            switch (format)
            {
                case FormatType.DEC:
                    return NewValue(System.Convert.ToUInt16(inputString, 10), format);
                case FormatType.HEX:
                    return NewValue(System.Convert.ToUInt16(inputString, 16), format);
                case FormatType.OCT:
                    return NewValue(System.Convert.ToUInt16(inputString, 8), format);
            }
            return value.ToString();
        }
    }

    public class Int16VariableInfo : VariableInfoBase
    {
        public Int16VariableInfo()
        {
            Formats = new List<string>
            {
                FormatType.DEC,
                FormatType.HEX,
                FormatType.OCT,
            };

            FormatSelectedItem = FormatType.DEC;

            _value = NewValue(0, FormatSelectedItem);

            PrepareValue = NewValue(0, FormatSelectedItem);
        }

        private WrappedValue<Int16> _value;
        public WrappedValue<Int16> Value
        {
            get { return _value; }
        }

        public WrappedValue<Int16> WriteValue
        {
            get { return Value; }

            set
            {
                DataValue.Value = value.Value;
                this.SetProperty(ref this._value, value);
            }
        }

        public WrappedValue<Int16> PrepareValue { get; set; }

        public override object GetPrepareValue()
        {
            return PrepareValue.Value;
        }

        public override void SetPrepareValue(object obj)
        {
            PrepareValue.Value = (Int16)obj;
        }

        public override void UpdateWrappedValue()
        {
            if (_value == null)
                return;

            if (DataValue.Value != null)
            {
                _value.Value = (Int16)DataValue.Value;
            }
            _value.Format = FormatSelectedItem;
            PrepareValue.Format = FormatSelectedItem;
        }

        public static WrappedValue<Int16> NewValue(Int16 newValue, string format)
        {
            return new WrappedValue<Int16>(newValue, format, ConvertFn, ConvertBackFn);
        }

        private static string ConvertFn(object value, string format)
        {
            switch (format)
            {
                case FormatType.DEC:
                    return Convert.ToString((Int16)value, 10);
                case FormatType.HEX:
                    return ((Int16)value).ToString("X4");
                case FormatType.OCT:
                    var convVal = (UInt16)((0x7FFF & (Int16)value) + (0x8000 & (Int16)value));
                    return Convert.ToString(convVal, 8);
            }
            return value.ToString();
        }

        private static object ConvertBackFn(object value, string format)
        {
            var inputString = value as string;
            switch (format)
            {
                case FormatType.DEC:
                    return NewValue(System.Convert.ToInt16(inputString, 10), format);
                case FormatType.HEX:
                    return NewValue(System.Convert.ToInt16(inputString, 16), format);
                case FormatType.OCT:
                    return NewValue(System.Convert.ToInt16(inputString, 8), format);
            }
            return value.ToString();
        }
    }

    public class UInt32VariableInfo : VariableInfoBase
    {
        public UInt32VariableInfo()
        {
            Formats = new List<string>
            {
                FormatType.DEC,
                FormatType.HEX,
                FormatType.OCT,
            };

            FormatSelectedItem = FormatType.DEC;

            _value = NewValue(0, FormatSelectedItem);

            PrepareValue = NewValue(0, FormatSelectedItem);
        }

        private WrappedValue<UInt32> _value;
        public WrappedValue<UInt32> Value
        {
            get { return _value; }
        }

        public WrappedValue<UInt32> WriteValue
        {
            get { return Value; }

            set
            {
                DataValue.Value = value.Value;
                this.SetProperty(ref this._value, value);
            }
        }

        public WrappedValue<UInt32> PrepareValue { get; set; }

        public override object GetPrepareValue()
        {
            return PrepareValue.Value;
        }

        public override void SetPrepareValue(object obj)
        {
            PrepareValue.Value = (UInt32)obj;
        }

        public override void UpdateWrappedValue()
        {
            if (_value == null)
                return;

            if (DataValue.Value != null)
            {
                _value.Value = (UInt32)DataValue.Value;
            }
            _value.Format = FormatSelectedItem;
            PrepareValue.Format = FormatSelectedItem;
        }

        public static WrappedValue<UInt32> NewValue(UInt32 newValue, string format)
        {
            return new WrappedValue<UInt32>(newValue, format, ConvertFn, ConvertBackFn);
        }

        private static string ConvertFn(object value, string format)
        {
            switch (format)
            {
                case FormatType.DEC:
                    return Convert.ToString((UInt32)value, 10);
                case FormatType.HEX:
                    return ((UInt32)value).ToString("X8");
                case FormatType.OCT:
                    return Convert.ToString((UInt32)value, 8);
            }
            return value.ToString();
        }

        private static object ConvertBackFn(object value, string format)
        {
            var inputString = value as string;
            switch (format)
            {
                case FormatType.DEC:
                    return NewValue(System.Convert.ToUInt32(inputString, 10), format);
                case FormatType.HEX:
                    return NewValue(System.Convert.ToUInt32(inputString, 16), format);
                case FormatType.OCT:
                    return NewValue(System.Convert.ToUInt32(inputString, 8), format);
            }
            return value.ToString();
        }
    }

    public class Int32VariableInfo : VariableInfoBase
    {
        public Int32VariableInfo()
        {
            Formats = new List<string>
            {
                FormatType.DEC,
                FormatType.HEX,
                FormatType.OCT,
            };

            FormatSelectedItem = FormatType.DEC;

            _value = NewValue(0, FormatSelectedItem);

            PrepareValue = NewValue(0, FormatSelectedItem);
        }

        private WrappedValue<Int32> _value;
        public WrappedValue<Int32> Value
        {
            get { return _value; }
        }

        public WrappedValue<Int32> WriteValue
        {
            get { return Value; }

            set
            {
                DataValue.Value = value.Value;
                this.SetProperty(ref this._value, value);
            }
        }

        public WrappedValue<Int32> PrepareValue { get; set; }

        public override object GetPrepareValue()
        {
            return PrepareValue.Value;
        }

        public override void SetPrepareValue(object obj)
        {
            PrepareValue.Value = (Int32)obj;
        }

        public override void UpdateWrappedValue()
        {
            if (_value == null)
                return;

            if (DataValue.Value != null)
            {
                _value.Value = (Int32)DataValue.Value;
            }
            _value.Format = FormatSelectedItem;
            PrepareValue.Format = FormatSelectedItem;
        }

        public static WrappedValue<Int32> NewValue(Int32 newValue, string format)
        {
            return new WrappedValue<Int32>(newValue, format, ConvertFn, ConvertBackFn);
        }

        private static string ConvertFn(object value, string format)
        {
            switch (format)
            {
                case FormatType.DEC:
                    return Convert.ToString((Int32)value, 10);
                case FormatType.HEX:
                    return ((Int32)value).ToString("X8");
                case FormatType.OCT:
                    var convVal = (UInt32)(((UInt32)0x7FFFFFFF & (Int32)value) + ((UInt32)0x80000000 & (Int32)value));
                    return Convert.ToString(convVal, 8);
            }
            return value.ToString();
        }

        private static object ConvertBackFn(object value, string format)
        {
            var inputString = value as string;
            switch (format)
            {
                case FormatType.DEC:
                    return NewValue(System.Convert.ToInt32(inputString, 10), format);
                case FormatType.HEX:
                    return NewValue(System.Convert.ToInt32(inputString, 16), format);
                case FormatType.OCT:
                    return NewValue(System.Convert.ToInt32(inputString, 8), format);
            }
            return value.ToString();
        }
    }

    public class Int64VariableInfo : VariableInfoBase
    {
        public Int64VariableInfo()
        {
            Formats = new List<string>
            {
                FormatType.DEC,
                FormatType.HEX,
                FormatType.OCT,
            };

            FormatSelectedItem = FormatType.DEC;

            _value = NewValue(0, FormatSelectedItem);

            PrepareValue = NewValue(0, FormatSelectedItem);
        }

        private WrappedValue<Int64> _value;
        public WrappedValue<Int64> Value
        {
            get { return _value; }
        }

        public WrappedValue<Int64> WriteValue
        {
            get { return Value; }

            set
            {
                DataValue.Value = value.Value;
                this.SetProperty(ref this._value, value);
            }
        }

        public WrappedValue<Int64> PrepareValue { get; set; }

        public override object GetPrepareValue()
        {
            return PrepareValue.Value;
        }

        public override void SetPrepareValue(object obj)
        {
            PrepareValue.Value = (Int64)obj;
        }

        public override void UpdateWrappedValue()
        {
            if (_value == null)
                return;

            if (DataValue.Value != null)
            {
                _value.Value = (Int64)DataValue.Value;
            }
            _value.Format = FormatSelectedItem;
            PrepareValue.Format = FormatSelectedItem;
        }

        public static WrappedValue<Int64> NewValue(Int64 newValue, string format)
        {
            return new WrappedValue<Int64>(newValue, format, ConvertFn, ConvertBackFn);
        }

        private static string ConvertFn(object value, string format)
        {
            switch (format)
            {
                case FormatType.DEC:
                    return Convert.ToString((Int64)value, 10);
                case FormatType.HEX:
                    return ((Int64)value).ToString("X16");
                case FormatType.OCT:
                    return Convert.ToString((Int64)value, 8);
            }
            return value.ToString();
        }

        private static object ConvertBackFn(object value, string format)
        {
            var inputString = value as string;
            switch (format)
            {
                case FormatType.DEC:
                    return NewValue(System.Convert.ToInt64(inputString, 10), format);
                case FormatType.HEX:
                    return NewValue(System.Convert.ToInt64(inputString, 16), format);
                case FormatType.OCT:
                    return NewValue(System.Convert.ToInt64(inputString, 8), format);
            }
            return value.ToString();
        }
    }

    public class UInt64VariableInfo : VariableInfoBase
    {
        public UInt64VariableInfo()
        {
            Formats = new List<string>
            {
                FormatType.DEC,
                FormatType.HEX,
                FormatType.OCT,
            };

            FormatSelectedItem = FormatType.DEC;

            _value = NewValue(0, FormatSelectedItem);

            PrepareValue = NewValue(0, FormatSelectedItem);
        }

        private WrappedValue<UInt64> _value;
        public WrappedValue<UInt64> Value
        {
            get { return _value; }
        }

        public WrappedValue<UInt64> WriteValue
        {
            get { return Value; }

            set
            {
                DataValue.Value = value.Value;
                this.SetProperty(ref this._value, value);
            }
        }

        public WrappedValue<UInt64> PrepareValue { get; set; }

        public override object GetPrepareValue()
        {
            return PrepareValue.Value;
        }

        public override void SetPrepareValue(object obj)
        {
            PrepareValue.Value = (UInt64)obj;
        }

        public override void UpdateWrappedValue()
        {
            if (_value == null)
                return;

            if (DataValue.Value != null)
            {
                _value.Value = (UInt64)DataValue.Value;
            }
            _value.Format = FormatSelectedItem;
            PrepareValue.Format = FormatSelectedItem;
        }

        public static WrappedValue<UInt64> NewValue(UInt64 newValue, string format)
        {
            return new WrappedValue<UInt64>(newValue, format, ConvertFn, ConvertBackFn);
        }

        private static string ConvertFn(object value, string format)
        {
            switch (format)
            {
                case FormatType.DEC:
                    return Convert.ToString((UInt64)value);
                case FormatType.HEX:
                    return ((UInt64)value).ToString("X16");
                case FormatType.OCT:
                    var v = (UInt64)value;
                    List<byte> list = new List<byte>();
                    if (v == 0)
                    {
                        list.Add(0);
                    }
                    else
                    {
                        while (v > 0)
                        {
                            list.Add((byte)(v % 8));
                            v /= 8;
                        }
                    }

                    string s = "";
                    for (int i = list.Count - 1; i >= 0; i--)
                    {
                        s += list[i].ToString();
                    }
                    return s;
            }
            return value.ToString();
        }

        private static object ConvertBackFn(object value, string format)
        {
            var inputString = value as string;
            switch (format)
            {
                case FormatType.DEC:
                    return NewValue(System.Convert.ToUInt64(inputString, 10), format);
                case FormatType.HEX:
                    return NewValue(System.Convert.ToUInt64(inputString, 16), format);
                case FormatType.OCT:
                    return NewValue(System.Convert.ToUInt64(inputString, 8), format);
            }
            return value.ToString();
        }
    }

    public class FloatVariableInfo : VariableInfoBase
    {
        public FloatVariableInfo()
        {
            Formats = new List<string>
            {
                FormatType.FLOAT,
            };

            FormatSelectedItem = FormatType.FLOAT;

            _value = NewValue((float)0.0, FormatSelectedItem);

            PrepareValue = NewValue((float)0.0, FormatSelectedItem);
        }

        private WrappedValue<float> _value;
        public WrappedValue<float> Value
        {
            get { return _value; }
        }

        public WrappedValue<float> WriteValue
        {
            get { return Value; }

            set
            {
                DataValue.Value = value.Value;
                this.SetProperty(ref this._value, value);
            }
        }

        public WrappedValue<float> PrepareValue { get; set; }

        public override object GetPrepareValue()
        {
            return PrepareValue.Value;
        }

        public override void SetPrepareValue(object obj)
        {
            PrepareValue.Value = (float)obj;
        }

        public override void UpdateWrappedValue()
        {
            if (_value == null)
                return;

            if (DataValue.Value != null)
            {
                _value.Value = (float)DataValue.Value;
            }
            _value.Format = FormatSelectedItem;
            PrepareValue.Format = FormatSelectedItem;
        }

        public static WrappedValue<float> NewValue(float newValue, string format)
        {
            return new WrappedValue<float>(newValue, format, ConvertFn, ConvertBackFn);
        }

        private static string ConvertFn(object value, string format)
        {
            return value.ToString();
        }

        private static object ConvertBackFn(object value, string format)
        {
            var inputString = value as string;
            var newValue = float.Parse(inputString, CultureInfo.InvariantCulture.NumberFormat);
            return NewValue(newValue, format);
        }
    }

    public class DoubleVariableInfo : VariableInfoBase
    {
        public DoubleVariableInfo()
        {
            Formats = new List<string>
            {
                FormatType.FLOAT,
            };

            FormatSelectedItem = FormatType.FLOAT;

            _value = NewValue(0.0, FormatSelectedItem);

            PrepareValue = NewValue(0.0, FormatSelectedItem);
        }

        private WrappedValue<double> _value;
        public WrappedValue<double> Value
        {
            get { return _value; }
        }

        public WrappedValue<double> WriteValue
        {
            get { return Value; }

            set
            {
                DataValue.Value = value.Value;
                this.SetProperty(ref this._value, value);
            }
        }

        public WrappedValue<double> PrepareValue { get; set; }

        public override object GetPrepareValue()
        {
            return PrepareValue.Value;
        }

        public override void SetPrepareValue(object obj)
        {
            PrepareValue.Value = (double)obj;
        }

        public override void UpdateWrappedValue()
        {
            if (_value == null)
                return;

            if (DataValue.Value != null)
            {
                _value.Value = (double)DataValue.Value;
            }
            _value.Format = FormatSelectedItem;
            PrepareValue.Format = FormatSelectedItem;
        }

        public static WrappedValue<double> NewValue(double newValue, string format)
        {
            return new WrappedValue<double>(newValue, format, ConvertFn, ConvertBackFn);
        }

        private static string ConvertFn(object value, string format)
        {
            return value.ToString();
        }

        private static object ConvertBackFn(object value, string format)
        {
            var inputString = value as string;
            var newValue = double.Parse(inputString, CultureInfo.InvariantCulture.NumberFormat);
            return NewValue(newValue, format);
        }
    }

    public class StringVariableInfo : VariableInfoBase
    {
        public StringVariableInfo()
        {
            Formats = new List<string>
            {
                FormatType.STRING,
            };

            FormatSelectedItem = FormatType.STRING;

            _value = NewValue("", FormatSelectedItem);

            PrepareValue = NewValue("", FormatSelectedItem);
        }

        private WrappedValue<string> _value;
        public WrappedValue<string> Value
        {
            get { return _value; }
        }

        public WrappedValue<string> WriteValue
        {
            get { return _value; }

            set
            {
                DataValue.Value = value.Value;
                this.SetProperty(ref this._value, value);
            }
        }

        public WrappedValue<string> PrepareValue { get; set; }

        public override object GetPrepareValue()
        {
            return PrepareValue.Value;
        }

        public override void SetPrepareValue(object obj)
        {
            PrepareValue.Value = (String)obj;
        }

        public override void UpdateWrappedValue()
        {
            if (_value == null)
                return;

            if (DataValue.Value != null)
            {
                _value.Value = (string)DataValue.Value;
            }
            _value.Format = FormatSelectedItem;
            PrepareValue.Format = FormatSelectedItem;
        }

        public static WrappedValue<string> NewValue(string newValue, string format)
        {
            return new WrappedValue<string>(newValue, format, ConvertFn, ConvertBackFn);
        }

        private static string ConvertFn(object value, string format)
        {
            if (value == null)
                return "";
            else
                return value.ToString();
        }

        private static object ConvertBackFn(object value, string format)
        {
            var inputValue = value as string;
            if(inputValue== null)
            {
                return NewValue("", format);
            }
            else
            {
                return NewValue(inputValue, format);
            }
        }
    }

    public class DateTimeVariableInfo : VariableInfoBase
    {
        public DateTimeVariableInfo()
        {
            Formats = new List<string>
            {
                FormatType.DATE_AND_TIME,
            };

            FormatSelectedItem = FormatType.DATE_AND_TIME;

            _value = NewValue(DateTime.MinValue, FormatSelectedItem);

            PrepareValue = NewValue(DateTime.MinValue, FormatSelectedItem);
        }

        private WrappedValue<DateTime> _value;
        public WrappedValue<DateTime> Value
        {
            get { return _value; }
        }

        public WrappedValue<DateTime> WriteValue
        {
            get { return Value; }

            set
            {
                DataValue.Value = value.Value;
                this.SetProperty(ref this._value, value);
            }
        }

        public WrappedValue<DateTime> PrepareValue { get; set; }

        public override object GetPrepareValue()
        {
            return PrepareValue.Value;
        }

        public override void SetPrepareValue(object obj)
        {
            PrepareValue.Value = (DateTime)obj;
        }

        public override void UpdateWrappedValue()
        {
            if (_value == null)
                return;

            if (DataValue.Value != null)
            {
                _value.Value = (DateTime)DataValue.Value;
            }
            _value.Format = FormatSelectedItem;
            PrepareValue.Format = FormatSelectedItem;
        }

        private static readonly string DATE_TIME_FORMAT = "yyyy-MM-dd HH:mm:ss.fff";

        public static WrappedValue<DateTime> NewValue(DateTime newValue, string format)
        {
            return new WrappedValue<DateTime>(newValue, format, ConvertFn, ConvertBackFn);
        }

        private static string ConvertFn(object value, string format)
        {
            return ((DateTime)value).ToString(DATE_TIME_FORMAT);
        }

        private static object ConvertBackFn(object value, string format)
        {
            var dt = DateTime.ParseExact((string)value, DATE_TIME_FORMAT, null);
            return NewValue(dt, format);
        }
    }

    public class VariantVariableInfo : VariableInfoBase
    {
        public VariantVariableInfo()
        {
            Formats = new List<string>
            {
                FormatType.VARIANT
            };

            FormatSelectedItem = FormatType.VARIANT;
        }

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
                this.RaisePropertyChanged("WriteValue");
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

    public abstract class VariableInfoBase : BindableBase
    {
        public List<string> Formats { get; protected set; }

        public virtual object ConvertBack(object value)
        {
            return this;
        }

        public VariableInfoBase()
        {
            this.DataValue = new DataValue(StatusCodes.UncertainInitialValue);
            errorsContainer = new ErrorsContainer<string>(OnErrorsChanged);
        }

        public string Type { get; set; }

        public Interfaces.IVariableConfiguration VariableConfiguration { get; set; }

        private string fmtItem;
        public string FormatSelectedItem
        {
            get { return fmtItem; }
            set {
                this.SetProperty(ref fmtItem, value);
                this.UpdateWrappedValue();
                this.RaisePropertyChanged("Value");
                this.RaisePropertyChanged("PrepareValue");
            }
        }

        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set { this.SetProperty(ref isSelected, value); }
        }

        private NodeId nodeId;
        public NodeId NodeId
        {
            get { return nodeId; }
            set { this.SetProperty(ref nodeId, value); }
        }

        private string displayname;
        public string DisplayName
        {
            get { return displayname; }
            set { this.SetProperty(ref displayname, value); }
        }

        public StatusCode StatusCode
        {
            get { return DataValue.StatusCode; }

            set
            {
                if (DataValue != null && DataValue.StatusCode != value)
                    DataValue.StatusCode = value;
            }
        }

        public DateTime ServerTimestamp
        {
            get { return DataValue.ServerTimestamp; }

            set
            {
                if (DataValue != null && DataValue.ServerTimestamp != value)
                    DataValue.ServerTimestamp = value;
            }
        }

        public DateTime SourceTimestamp
        {
            get { return DataValue.SourceTimestamp; }

            set
            {
                if (DataValue != null && DataValue.SourceTimestamp != value)
                    DataValue.SourceTimestamp = value;
            }
        }

        public uint ClientHandle;

        private DataValue dataValue;
        public DataValue DataValue
        {
            get { return dataValue; }
            set
            {
                this.dataValue = value;
                this.UpdateWrappedValue();
                this.RaisePropertyChanged("Value");
                this.RaisePropertyChanged("StatusCode");
                this.RaisePropertyChanged("ServerTimestamp");
                this.RaisePropertyChanged("SourceTimestamp");
            }
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
        }

        public virtual void UpdateWrappedValue() { }

        public abstract object GetPrepareValue();

        public abstract void SetPrepareValue(object obj);

        private ErrorsContainer<string> errorsContainer;
        protected void OnErrorsChanged([CallerMemberName] string propertyName = null)
        {
            this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public System.Collections.IEnumerable GetErrors(string propertyName)
        {
            return this.errorsContainer.GetErrors(propertyName);
        }

        public bool HasErrors
        {
            get { return this.errorsContainer.HasErrors; }
        }

        protected bool ValidateProperty(object value, [CallerMemberName] string propertyName = null)
        {
            var context = new ValidationContext(this) { MemberName = propertyName };
            var validationErrors = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
            if (!Validator.TryValidateProperty(value, context, validationErrors))
            {
                var errors = validationErrors.Select(error => error.ErrorMessage);
                this.errorsContainer.SetErrors(propertyName, errors);
                return true;
            }
            else
            {
                this.errorsContainer.ClearErrors(propertyName);
                return false;
            }
        }
    }
}
