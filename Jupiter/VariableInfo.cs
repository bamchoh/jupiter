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
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Infrastructure;
using System.Windows.Input;
using Unity.Injection;

namespace Jupiter
{
    public abstract class DataValueBase
    {
        protected DataValue dataValue;

        protected object preparedValue;

        public DataValueBase(DataValue dataValue)
        {
            this.dataValue = dataValue;
        }

        public StatusCode StatusCode
        {
            get { return dataValue.StatusCode; }
        }

        public DateTime ServerTimestamp
        {
            get { return dataValue.ServerTimestamp; }
        }

        public DateTime SourceTimestamp
        {
            get { return dataValue.SourceTimestamp; }
        }

        public BuiltInType Type
        {
            get { return dataValue?.WrappedValue.TypeInfo?.BuiltInType ?? BuiltInType.Null; }
        }

        public List<string> Formats { get; protected set; }

        public string FormatSelectedItem { get; set; }

        public bool FormatIsEnabled { get; set; }

        public void UpdateDataValue(DataValue dv)
        {
            this.dataValue = dv;
        }

        public object GetRawValue()
        {
            return this.dataValue.Value;
        }

        public object GetPreparedValue()
        {
            return preparedValue;
        }

        public object ConvertValue()
        {
            return _convertValue(dataValue.Value);
        }

        public void ConvertValueBack(object value)
        {
            try
            {
                dataValue.Value = _convertBackValue(value);
            }
            catch(Exception)
            {

            }
        }

        public object ConvertPreparedValue()
        {
            return _convertValue(preparedValue);
        }

        public void ConvertBackPreparedValue(object value)
        {
            try
            {
                preparedValue = _convertBackValue(value);
            }
            catch(Exception)
            {

            }
        }

        public bool Validate(object value)
        {
            try
            {
                _convertBackValue(value);
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        protected abstract object _convertValue(object value);

        protected abstract object _convertBackValue(object value);
    }

    public class NullDataValue : DataValueBase
    {
        public NullDataValue(DataValue dataValue) : base(dataValue)
        {
            this.dataValue = dataValue;

            Formats = new List<string>
            {
                FormatType.NULL
            };

            FormatSelectedItem = FormatType.NULL;

            FormatIsEnabled = false;
        }

        protected override object _convertValue(object value)
        {
            return "";
        }

        protected override object _convertBackValue(object value)
        {
            return null;
        }
    }

    public class BooleanDataValue : DataValueBase
    {
        public BooleanDataValue(DataValue dv) : base(dv)
        {
            preparedValue = false;

            Formats = new List<string>
            {
                FormatType.BOOL,
            };

            FormatSelectedItem = FormatType.BOOL;

            FormatIsEnabled = false;
        }

        protected override object _convertValue(object value)
        {
            return value;
        }

        protected override object _convertBackValue(object value)
        {
            return value;
        }
    }

    public class ByteDataValue : DataValueBase
    {
        public ByteDataValue(DataValue dataValue) : base(dataValue)
        {
            preparedValue = (byte)0;

            Formats = new List<string>
            {
                FormatType.DEC,
                FormatType.HEX,
                FormatType.OCT,
            };

            FormatSelectedItem = FormatType.DEC;

            FormatIsEnabled = true;
        }

        protected override object _convertValue(object value)
        {
            switch (FormatSelectedItem)
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

        protected override object _convertBackValue(object value)
        {
            var inputString = value as string;
            switch (FormatSelectedItem)
            {
                case FormatType.DEC:
                    return System.Convert.ToByte(inputString, 10);
                case FormatType.HEX:
                    return System.Convert.ToByte(inputString, 16);
                case FormatType.OCT:
                    return System.Convert.ToByte(inputString, 8);
            }
            return null;
        }
    }

    public class SByteDataValue : DataValueBase
    {
        public SByteDataValue(DataValue dataValue) : base(dataValue)
        {
            preparedValue = (sbyte)0;

            Formats = new List<string>
            {
                FormatType.DEC,
                FormatType.HEX,
                FormatType.OCT,
            };

            FormatSelectedItem = FormatType.DEC;

            FormatIsEnabled = true;
        }

        protected override object _convertValue(object value)
        {
            switch (FormatSelectedItem)
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

        protected override object _convertBackValue(object value)
        {
            var inputString = value as string;
            switch (FormatSelectedItem)
            {
                case FormatType.DEC:
                    return System.Convert.ToSByte(inputString, 10);
                case FormatType.HEX:
                    return System.Convert.ToSByte(inputString, 16);
                case FormatType.OCT:
                    return System.Convert.ToSByte(inputString, 8);
            }
            return null;
        }
    }

    public class UInt16DataValue : DataValueBase
    {
        public UInt16DataValue(DataValue dataValue) : base(dataValue)
        {
            preparedValue = (UInt16)0;

            Formats = new List<string>
            {
                FormatType.DEC,
                FormatType.HEX,
                FormatType.OCT,
            };

            FormatSelectedItem = FormatType.DEC;

            FormatIsEnabled = true;
        }

        protected override object _convertValue(object value)
        {
            switch (FormatSelectedItem)
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

        protected override object _convertBackValue(object value)
        {
            var inputString = value as string;
            switch (FormatSelectedItem)
            {
                case FormatType.DEC:
                    return System.Convert.ToUInt16(inputString, 10);
                case FormatType.HEX:
                    return System.Convert.ToUInt16(inputString, 16);
                case FormatType.OCT:
                    return System.Convert.ToUInt16(inputString, 8);
            }
            return null;
        }
    }

    public class Int16DataValue : DataValueBase
    {
        public Int16DataValue(DataValue dataValue) : base(dataValue)
        {
            preparedValue = (Int16)0;

            Formats = new List<string>
            {
                FormatType.DEC,
                FormatType.HEX,
                FormatType.OCT,
            };

            FormatSelectedItem = FormatType.DEC;

            FormatIsEnabled = true;
        }

        protected override object _convertValue(object value)
        {
            switch (FormatSelectedItem)
            {
                case FormatType.DEC:
                    return Convert.ToString((Int16)value, 10);
                case FormatType.HEX:
                    return ((Int16)value).ToString("X4");
                case FormatType.OCT:
                    var convVal = (UInt16)((0x7FFF & (Int16)value) + (0x8000 & (Int16)value));
                    return Convert.ToString(convVal, 8);
            }
            return dataValue.Value.ToString();
        }

        protected override object _convertBackValue(object value)
        {
            var inputString = value as string;
            switch (FormatSelectedItem)
            {
                case FormatType.DEC:
                    return System.Convert.ToInt16(inputString, 10);
                case FormatType.HEX:
                    return System.Convert.ToInt16(inputString, 16);
                case FormatType.OCT:
                    return System.Convert.ToInt16(inputString, 8);
            }
            return this;
        }
    }

    public class UInt32DataValue : DataValueBase
    {
        public UInt32DataValue(DataValue dataValue) : base(dataValue)
        {
            preparedValue = (UInt32)0;

            Formats = new List<string>
            {
                FormatType.DEC,
                FormatType.HEX,
                FormatType.OCT,
            };

            FormatSelectedItem = FormatType.DEC;

            FormatIsEnabled = true;
        }

        protected override object _convertValue(object value)
        {
            switch (FormatSelectedItem)
            {
                case FormatType.DEC:
                    return Convert.ToString((UInt32)value, 10);
                case FormatType.HEX:
                    return ((UInt32)value).ToString("X8");
                case FormatType.OCT:
                    return Convert.ToString((UInt32)value, 8);
            }
            return dataValue.Value.ToString();
        }

        protected override object _convertBackValue(object value)
        {
            var inputString = value as string;
            switch (FormatSelectedItem)
            {
                case FormatType.DEC:
                    return System.Convert.ToUInt32(inputString, 10);
                case FormatType.HEX:
                    return System.Convert.ToUInt32(inputString, 16);
                case FormatType.OCT:
                    return System.Convert.ToUInt32(inputString, 8);
            }
            return this;
        }
    }

    public class Int32DataValue : DataValueBase
    {
        public Int32DataValue(DataValue dataValue) : base(dataValue)
        {
            preparedValue = (Int32)0;

            Formats = new List<string>
            {
                FormatType.DEC,
                FormatType.HEX,
                FormatType.OCT,
            };

            FormatSelectedItem = FormatType.DEC;

            FormatIsEnabled = true;
        }

        protected override object _convertValue(object value)
        {
            switch (FormatSelectedItem)
            {
                case FormatType.DEC:
                    return Convert.ToString((Int32)value, 10);
                case FormatType.HEX:
                    return ((Int32)value).ToString("X8");
                case FormatType.OCT:
                    var convVal = (UInt32)(((UInt32)0x7FFFFFFF & (Int32)value) + ((UInt32)0x80000000 & (Int32)value));
                    return Convert.ToString(convVal, 8);
            }
            return dataValue.Value.ToString();
        }

        protected override object _convertBackValue(object value)
        {
            var inputString = value as string;
            switch (FormatSelectedItem)
            {
                case FormatType.DEC:
                    return System.Convert.ToInt32(inputString, 10);
                case FormatType.HEX:
                    return System.Convert.ToInt32(inputString, 16);
                case FormatType.OCT:
                    return System.Convert.ToInt32(inputString, 8);
            }
            return this;
        }
    }

    public class UInt64DataValue : DataValueBase
    {
        public UInt64DataValue(DataValue dataValue) : base(dataValue)
        {
            preparedValue = (UInt64)0;

            Formats = new List<string>
            {
                FormatType.DEC,
                FormatType.HEX,
                FormatType.OCT,
            };

            FormatSelectedItem = FormatType.DEC;

            FormatIsEnabled = true;
        }

        protected override object _convertValue(object value)
        {
            switch (FormatSelectedItem)
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
            return dataValue.Value.ToString();
        }

        protected override object _convertBackValue(object value)
        {
            var inputString = value as string;
            switch (FormatSelectedItem)
            {
                case FormatType.DEC:
                    return System.Convert.ToUInt64(inputString, 10);
                case FormatType.HEX:
                    return System.Convert.ToUInt64(inputString, 16);
                case FormatType.OCT:
                    return System.Convert.ToUInt64(inputString, 8);
            }
            return this;
        }
    }

    public class Int64DataValue : DataValueBase
    {
        public Int64DataValue(DataValue dataValue) : base(dataValue)
        {
            preparedValue = (Int64)0;

            Formats = new List<string>
            {
                FormatType.DEC,
                FormatType.HEX,
                FormatType.OCT,
            };

            FormatSelectedItem = FormatType.DEC;

            FormatIsEnabled = true;
        }

        protected override object _convertValue(object value)
        {
            switch (FormatSelectedItem)
            {
                case FormatType.DEC:
                    return Convert.ToString((Int64)value, 10);
                case FormatType.HEX:
                    return ((Int64)value).ToString("X16");
                case FormatType.OCT:
                    return Convert.ToString((Int64)value, 8);
            }
            return dataValue.Value.ToString();
        }

        protected override object _convertBackValue(object value)
        {
            var inputString = value as string;
            switch (FormatSelectedItem)
            {
                case FormatType.DEC:
                    return System.Convert.ToInt64(inputString, 10);
                case FormatType.HEX:
                    return System.Convert.ToInt64(inputString, 16);
                case FormatType.OCT:
                    return System.Convert.ToInt64(inputString, 8);
            }
            return this;
        }
    }

    public class FloatDataValue : DataValueBase
    {
        public FloatDataValue(DataValue dataValue) : base(dataValue)
        {
            preparedValue = (float)0;

            Formats = new List<string>
            {
                FormatType.FLOAT,
            };

            FormatSelectedItem = FormatType.FLOAT;

            FormatIsEnabled = false;
        }

        protected override object _convertValue(object value)
        {
            return value.ToString();
        }

        protected override object _convertBackValue(object value)
        {
            var inputString = value as string;
            return float.Parse(inputString, CultureInfo.InvariantCulture.NumberFormat);
        }
    }

    public class DoubleDataValue : DataValueBase
    {
        public DoubleDataValue(DataValue dataValue) : base(dataValue)
        {
            preparedValue = (double)0;

            Formats = new List<string>
            {
                FormatType.FLOAT,
            };

            FormatSelectedItem = FormatType.FLOAT;

            FormatIsEnabled = false;
        }

        protected override object _convertValue(object value)
        {
            return value.ToString();
        }

        protected override object _convertBackValue(object value)
        {
            var inputString = value as string;
            return double.Parse(inputString, CultureInfo.InvariantCulture.NumberFormat);
        }
    }

    public class StringDataValue : DataValueBase
    {
        public StringDataValue(DataValue dataValue) : base(dataValue)
        {
            preparedValue = "";

            Formats = new List<string>
            {
                FormatType.STRING,
            };

            FormatSelectedItem = FormatType.STRING;

            FormatIsEnabled = false;
        }

        protected override object _convertValue(object value)
        {
            if (value == null)
                return "";
            else
                return value.ToString();
        }

        protected override object _convertBackValue(object value)
        {
            var inputValue = value as string;
            if (inputValue == null)
            {
                return "";
            }
            else
            {
                return inputValue;
            }
        }
    }

    public class DateTimeDataValue : DataValueBase
    {
        private readonly string DATE_TIME_FORMAT = "yyyy-MM-dd HH:mm:ss.fff";

        public DateTimeDataValue(DataValue dataValue) : base(dataValue)
        {
            preparedValue = new DateTime();

            Formats = new List<string>
            {
                FormatType.DATE_AND_TIME,
            };

            FormatSelectedItem = FormatType.DATE_AND_TIME;

            FormatIsEnabled = false;
        }

        protected override object _convertValue(object value)
        {
            return ((DateTime)value).ToString(DATE_TIME_FORMAT);
        }

        protected override object _convertBackValue(object value)
        {
            return DateTime.ParseExact((string)value, DATE_TIME_FORMAT, null);
        }
    }

    public class VariableInfo : BindableBase, IDataErrorInfo, Interfaces.IVariableInfo
    {
        public static NullDataValue NullDataValue = new NullDataValue(new DataValue(StatusCodes.BadWaitingForInitialData));

        #region Properties(DataGridView)
        private NodeId nodeId;
        public NodeId NodeId {
            get { return nodeId; }
            set { this.SetProperty(ref nodeId, value); }
        }

        private string displayname;
        public string DisplayName
        {
            get { return displayname; }
            set { this.SetProperty(ref displayname, value); }
        }

        public BuiltInType Type
        {
            get { return _value.Type; }
        }

        public List<string> Formats
        {
            get { return _value.Formats; }
        }

        public bool FormatIsEnabled
        {
            get { return _value.FormatIsEnabled; }
        }

        private DataValueBase _value;
        public object Value
        {
            get { return _value.ConvertValue(); }
        }

        public object BoolValue
        {
            get
            {
                if (Type != BuiltInType.Boolean)
                    return false;

                return _value.ConvertValue();
            }
            set
            {
                if (Type != BuiltInType.Boolean)
                    return;

                _value.ConvertValueBack(value);
                RaisePropertyChanged("WriteValue");
                RaisePropertyChanged("BoolValue");
            }
        }

        private object _writeValue; 
        public object WriteValue
        {
            get {
                if (this["WriteValue"] != null)
                    return _writeValue;
                else
                    return _value.ConvertValue();
            }

            set
            {
                if (_value.Validate(value))
                {
                    Errors.Remove("WriteValue");
                    _value.ConvertValueBack(value);
                }
                else
                {
                    Errors["WriteValue"] = "Invalid Input Value";
                }
                SetProperty(ref _writeValue, value);
            }
        }

        private object _preparedValue;
        public object PreparedValue
        {
            get {
                if (this["PreparedValue"] != null)
                    return _preparedValue;
                else
                    return _value.ConvertPreparedValue();
            }

            set {
                if (_value.Validate(value))
                {
                    Errors.Remove("PreparedValue");
                    _value.ConvertBackPreparedValue(value);
                }
                else
                {
                    Errors["PreparedValue"] = "Invalid Input Value";
                }
                SetProperty(ref _preparedValue, value);
            }
        }

        public void ResetValue()
        {
            _writeValue = null;
            Errors.Remove("WriteValue");
            Errors.Remove("PreparedValue");
        }

        public object PreparedBoolValue
        {
            get
            {
                if (Type != BuiltInType.Boolean)
                    return false;

                return _value.ConvertPreparedValue();
            }

            set
            {
                if (Type != BuiltInType.Boolean)
                    return;

                _value.ConvertBackPreparedValue(value);
                RaisePropertyChanged("PreparedBoolValue");
            }
        }

        public StatusCode StatusCode
        {
            get { return _value.StatusCode; }
        }

        public DateTime ServerTimestamp
        {
            get { return _value.ServerTimestamp; }
        }

        public DateTime SourceTimestamp
        {
            get { return _value.SourceTimestamp; }
        }
        #endregion

        public VariableInfo Self
        {
            get { return this; }
        }

        public void Validate(object value)
        {
            _value.Validate(value);
        }

        #region Properties(Others)
        public uint ClientHandle;

        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set { this.SetProperty(ref isSelected, value); }
        }

        public string FormatSelectedItem
        {
            get { return _value.FormatSelectedItem; }

            set
            {
                this._value.FormatSelectedItem = value;
                this.RaisePropertyChanged("FormatSelectedItem");
                this.RaisePropertyChanged("Value");
                this.RaisePropertyChanged("PreparedValue");
            }
        }
        #endregion

        private void InitializeClass(NodeId nodeId, string displayName)
        {
            this.nodeId = nodeId;

            this.displayname = displayName;

            this.CancelCommand = new DelegateCommand(() =>
            {
                ResetValue();
            });
        }

        public VariableInfo(NodeId nodeId, string displayName)
        {
            InitializeClass(nodeId, displayName);

            this._value = NullDataValue;
        }

        public VariableInfo(NodeId nodeId, string displayName, BuiltInType builtInType)
        {
            InitializeClass(nodeId, displayName);

            object defaultValue;
            switch(builtInType)
            {
                case BuiltInType.String:
                    defaultValue = "";
                    break;
                default:
                    defaultValue = TypeInfo.GetDefaultValue(builtInType);
                    break;
            }
            var typeInfo = TypeInfo.Construct(defaultValue);
            var dv = new DataValue(new Variant(defaultValue, typeInfo), StatusCodes.BadWaitingForInitialData);

            NewDataValue(dv);
        }

        public object GetRawValue()
        {
            return _value.GetRawValue();
        }

        public object GetPreparedValue()
        {
            return _value.GetPreparedValue();
        }

        public void UpdateType(BuiltInType builtInType)
        {
            var defaultValue = TypeInfo.GetDefaultValue(builtInType);
            var typeInfo = TypeInfo.Construct(defaultValue);
            var dv = new DataValue(new Variant(defaultValue, typeInfo));
            this._value.UpdateDataValue(dv);

            this.RaisePropertyChanged("Type");
        }

        public void UpdateDataValue(DataValue dv)
        {
            if(this._value.Type != BuiltInType.Null && dv.Value == null)
            {
                var defaultValue = TypeInfo.GetDefaultValue(this._value.Type);
                var typeInfo = TypeInfo.Construct(defaultValue);
                var newdv = new DataValue(new Variant(defaultValue, typeInfo));

                newdv.StatusCode = dv.StatusCode;
                newdv.ServerTimestamp = dv.ServerTimestamp;
                newdv.SourceTimestamp = dv.SourceTimestamp;

                this._value.UpdateDataValue(newdv);
            }
            else
            {
                this._value.UpdateDataValue(dv);
            }

            this.RaisePropertyChanged("Value");
            this.RaisePropertyChanged("BoolValue");
            this.RaisePropertyChanged("StatusCode");
            this.RaisePropertyChanged("ServerTimestamp");
            this.RaisePropertyChanged("SourceTimestamp");
        }

        public void NewDataValue(DataValue dv)
        {
            var builtInType = dv?.WrappedValue.TypeInfo?.BuiltInType;
            switch(builtInType)
            {
                case BuiltInType.Null:
                    this._value = new NullDataValue(dv);
                    break;
                case BuiltInType.Boolean:
                    this._value = new BooleanDataValue(dv);
                    break;
                case BuiltInType.SByte:
                    this._value = new SByteDataValue(dv);
                    break;
                case BuiltInType.Byte:
                    this._value = new ByteDataValue(dv);
                    break;
                case BuiltInType.Int16:
                    this._value = new Int16DataValue(dv);
                    break;
                case BuiltInType.UInt16:
                    this._value = new UInt16DataValue(dv);
                    break;
                case BuiltInType.Int32:
                    this._value = new Int32DataValue(dv);
                    break;
                case BuiltInType.UInt32:
                    this._value = new UInt32DataValue(dv);
                    break;
                case BuiltInType.Int64:
                    this._value = new Int64DataValue(dv);
                    break;
                case BuiltInType.UInt64:
                    this._value = new UInt64DataValue(dv);
                    break;
                case BuiltInType.Float:
                    this._value = new FloatDataValue(dv);
                    break;
                case BuiltInType.Double:
                    this._value = new DoubleDataValue(dv);
                    break;
                case BuiltInType.String:
                    this._value = new StringDataValue(dv);
                    break;
                case BuiltInType.DateTime:
                    this._value = new DateTimeDataValue(dv);
                    break;
                default:
                    this._value = new NullDataValue(dv);
                    break;
            }

            this.RaisePropertyChanged("Value");
            this.RaisePropertyChanged("BoolValue");
            this.RaisePropertyChanged("PreparedValue");
            this.RaisePropertyChanged("Type");
            this.RaisePropertyChanged("FormatSelectedItem");
            this.RaisePropertyChanged("Formats");
            this.RaisePropertyChanged("FormatIsEnabled");
            this.RaisePropertyChanged("StatusCode");
            this.RaisePropertyChanged("ServerTimestamp");
            this.RaisePropertyChanged("SourceTimestamp");
        }

        public ICommand CancelCommand { get; set; }

        private Dictionary<string, string> Errors = new Dictionary<string, string>();

        public string Error => throw new NotImplementedException();

        public string this[string columnName]
        {
            get
            {
                return Errors.ContainsKey(columnName) ? Errors[columnName] : null;
            }
        }
    }
}
