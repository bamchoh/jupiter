using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Documents;
using Microsoft.ClearScript.V8;
using Opc.Ua;

namespace Jupiter.ScriptEngine
{
    public class ScriptVariableInfo : Jupiter.Interfaces.IVariableInfo
    {
        public NodeId NodeId { get; set; }

        public object Value { get; set; }

        public ScriptVariableInfo(string nodeid)
        {
            NodeId = new NodeId(nodeid);
        }

        public void SetValue(BuiltInType builtInType, object rawValue)
        {
            switch(builtInType)
            {
                case BuiltInType.Null:
                    if (rawValue == null)
                    {
                        Value = rawValue;
                        return;
                    }
                    break;
                case BuiltInType.Boolean:
                    if (rawValue is bool)
                    {
                        Value = rawValue;
                        return;
                    }
                    break;
                case BuiltInType.SByte:
                    {
                        sbyte val;
                        if (SByte.TryParse(rawValue.ToString(), out val))
                        {
                            Value = val;
                            return;
                        }
                    }
                    break;
                case BuiltInType.Byte:
                    {
                        byte val;
                        if (Byte.TryParse(rawValue.ToString(), out val))
                        {
                            Value = val;
                            return;
                        }
                    }
                    break;
                case BuiltInType.Int16:
                    {
                        Int16 val;
                        if (Int16.TryParse(rawValue.ToString(), out val))
                        {
                            Value = val;
                            return;
                        }
                    }
                    break;
                case BuiltInType.UInt16:
                    {
                        UInt16 val;
                        if (UInt16.TryParse(rawValue.ToString(), out val))
                        {
                            Value = val;
                            return;
                        }
                    }
                    break;
                case BuiltInType.Int32:
                    {
                        Int32 val;
                        if (Int32.TryParse(rawValue.ToString(), out val))
                        {
                            Value = val;
                            return;
                        }
                    }
                    break;
                case BuiltInType.UInt32:
                    {
                        UInt32 val;
                        if (UInt32.TryParse(rawValue.ToString(), out val))
                        {
                            Value = val;
                            return;
                        }
                    }
                    break;
                case BuiltInType.Int64:
                    {
                        Int64 val;
                        if (Int64.TryParse(rawValue.ToString(), out val))
                        {
                            Value = val;
                            return;
                        }
                    }
                    break;
                case BuiltInType.UInt64:
                    {
                        UInt64 val;
                        if (UInt64.TryParse(rawValue.ToString(), out val))
                        {
                            Value = val;
                            return;
                        }
                    }
                    break;
                case BuiltInType.Float:
                    {
                        float val;
                        if (float.TryParse(rawValue.ToString(), out val))
                        {
                            Value = val;
                            return;
                        }
                    }
                    break;
                case BuiltInType.Double:
                    {
                        double val;
                        if (double.TryParse(rawValue.ToString(), out val))
                        {
                            Value = val;
                            return;
                        }
                    }
                    break;
                case BuiltInType.String:
                    if(rawValue is string)
                    {
                        Value = rawValue;
                        return;
                    }
                    break;
                case BuiltInType.DateTime:
                    if(rawValue is string)
                    {
                        Value = rawValue;
                        return;
                    }
                    break;
            }

            Value = rawValue;
        }
    }

    public class ScriptClient
    {
        private Client client;

        public ScriptClient(Client client)
        {
            this.client = client;
        }

        public dynamic Read(IList nodeids)
        {
            try
            {
                var itemsToRead = new ReadValueIdCollection();

                foreach (string nodeid in nodeids)
                {
                    var rv = new ReadValueId()
                    {
                        NodeId = new NodeId(nodeid),
                        AttributeId = Attributes.Value,
                        IndexRange = null,
                        DataEncoding = null,
                    };
                    itemsToRead.Add(rv);
                }


                DataValueCollection values;
                DiagnosticInfoCollection diagnosticInfos;

                ResponseHeader responseHeader = client.Read(
                    itemsToRead,
                    out values,
                    out diagnosticInfos);

                ClientBase.ValidateResponse(values, itemsToRead);
                ClientBase.ValidateDiagnosticInfos(diagnosticInfos, itemsToRead);

                return values;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                return null;
            }
        }

        public dynamic Write(IList items)
        {
            var viList = new List<ScriptVariableInfo>();

            foreach(dynamic dataValue in items)
            {
                viList.Add(new ScriptVariableInfo(dataValue.node_id));
            }

            var builtInTypes = new List<BuiltInType>();

            client.ReadBuiltInType(viList, out builtInTypes);

            for(int i = 0;i<viList.Count;i++)
            {
                var vi = viList[i];
                var t = builtInTypes[i];
                dynamic item = items[i];

                vi.SetValue(t, item.value);
            }

            try
            {
                var values = new WriteValueCollection();

                foreach (var vi in viList)
                {
                    var value = new WriteValue();

                    value.NodeId = vi.NodeId;
                    value.AttributeId = Attributes.Value;
                    value.IndexRange = null;
                    value.Value.StatusCode = StatusCodes.Good;
                    value.Value.ServerTimestamp = DateTime.MinValue;
                    value.Value.SourceTimestamp = DateTime.MinValue;
                    value.Value.Value = vi.Value;

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

                ResponseHeader responseHeader = client.CoreWrite(values, out results, out diagnosticInfos);

                ClientBase.ValidateResponse(results, values);
                ClientBase.ValidateDiagnosticInfos(diagnosticInfos, values);

                for (int i = 0; i < values.Count; i++)
                {
                    values[i].Value.StatusCode = results[i];
                }

                return values;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                return null;
            }
        }
    }
}