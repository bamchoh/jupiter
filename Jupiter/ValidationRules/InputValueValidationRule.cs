using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;
using System.Windows.Markup;
using System.ComponentModel;

namespace Jupiter.ValidationRules
{
    [ContentProperty("InputValue")]
    class InputValueValidationRule : ValidationRule
    {
        public InputValue InputValue { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                InputValue.Value.Validate(value);
                return ValidationResult.ValidResult;
            }
            catch (Exception)
            {
                return new ValidationResult(false, "Invalid Value");
            }
        }
    }

    public class InputValue : DependencyObject
    {   
        private static readonly VariableInfo defaultValue = new VariableInfo(0, "", Opc.Ua.BuiltInType.Byte);

        public VariableInfo Value
        {
            get { return (VariableInfo)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            nameof(Value),
            typeof(VariableInfo),
            typeof(InputValue),
            new PropertyMetadata(defaultValue));
    }
}
