using System.Windows;
using System.Windows.Controls;
using Opc.Ua;

namespace Jupiter
{
    public class ValueTemplateSelector : DataTemplateSelector
    {
        public DataTemplate VariantTemplate
        { get; set; }

        public DataTemplate DefaultTemplate
        { get; set; }

        public DataTemplate BoolTemplate
        { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            System.Diagnostics.Trace.WriteLine(item);

            if (item == null)
            {
                return base.SelectTemplate(item, container);
            }

            var variableInfo = (VariableInfo)item;

            if (variableInfo.Type == BuiltInType.Boolean)
                return BoolTemplate;

            if (variableInfo.Type == BuiltInType.SByte)
                return DefaultTemplate;

            if (variableInfo.Type == BuiltInType.Byte)
                return DefaultTemplate;

            if (variableInfo.Type == BuiltInType.Int16)
                return DefaultTemplate;

            if (variableInfo.Type == BuiltInType.UInt16)
                return DefaultTemplate;

            if (variableInfo.Type == BuiltInType.Int32)
                return DefaultTemplate;

            if (variableInfo.Type == BuiltInType.UInt32)
                return DefaultTemplate;

            if (variableInfo.Type == BuiltInType.Int64)
                return DefaultTemplate;

            if (variableInfo.Type == BuiltInType.UInt64)
                return DefaultTemplate;

            if (variableInfo.Type == BuiltInType.Float)
                return DefaultTemplate;

            if (variableInfo.Type == BuiltInType.Double)
                return DefaultTemplate;

            if (variableInfo.Type == BuiltInType.String)
                return DefaultTemplate;

            if (variableInfo.Type == BuiltInType.DateTime)
                return DefaultTemplate;

            if (variableInfo.Type == BuiltInType.Null)
                return DefaultTemplate;

            return base.SelectTemplate(item, container);
        }
    }

}
