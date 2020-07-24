using System.Windows;
using System.Windows.Controls;

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
            if (item == null)
            {
                return base.SelectTemplate(item, container);
            }

            if (item is BooleanVariableInfo)
                return BoolTemplate;

            if (item is SByteVariableInfo)
                return DefaultTemplate;

            if (item is ByteVariableInfo)
                return DefaultTemplate;

            if (item is Int16VariableInfo)
                return DefaultTemplate;

            if (item is UInt16VariableInfo)
                return DefaultTemplate;

            if (item is Int32VariableInfo)
                return DefaultTemplate;

            if (item is UInt32VariableInfo)
                return DefaultTemplate;

            if (item is Int64VariableInfo)
                return DefaultTemplate;

            if (item is UInt64VariableInfo)
                return DefaultTemplate;

            if (item is FloatVariableInfo)
                return DefaultTemplate;

            if (item is DoubleVariableInfo)
                return DefaultTemplate;

            if (item is StringVariableInfo)
                return DefaultTemplate;

            if (item is DateTimeVariableInfo)
                return DefaultTemplate;

            if (item is VariantVariableInfo)
                return VariantTemplate;

            return base.SelectTemplate(item, container);
        }
    }

}
