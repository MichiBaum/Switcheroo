using System;
using System.Globalization;
using System.Windows.Data;

namespace Switcheroo {
    public class BoolConverter<T> : IValueConverter {
        public T IfTrue { get; set; }
        public T IfFalse { get; set; }

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return (bool)value ? IfTrue : IfFalse;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}