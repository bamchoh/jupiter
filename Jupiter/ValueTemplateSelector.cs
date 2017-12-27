using System.Windows;
using System.Windows.Controls;

namespace Jupiter
{
    public class ValueTemplateSelector : DataTemplateSelector
    {
        public DataTemplate VariantTemplate
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
                return VariantTemplate;

            if (item is ByteVariableInfo)
                return VariantTemplate;

            if (item is Int16VariableInfo)
                return VariantTemplate;

            if (item is UInt16VariableInfo)
                return VariantTemplate;

            if (item is Int32VariableInfo)
                return VariantTemplate;

            if (item is UInt32VariableInfo)
                return VariantTemplate;

            if (item is Int64VariableInfo)
                return VariantTemplate;

            if (item is UInt64VariableInfo)
                return VariantTemplate;

            if (item is FloatVariableInfo)
                return VariantTemplate;

            if (item is DoubleVariableInfo)
                return VariantTemplate;

            if (item is StringVariableInfo)
                return VariantTemplate;

            if (item is DateTimeVariableInfo)
                return VariantTemplate;

            if (item is VariantVariableInfo)
                return VariantTemplate;

            return base.SelectTemplate(item, container);
        }
    }

}
