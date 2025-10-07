using System;
using System.Globalization;
using AbeXP.Models;

namespace AbeXP.Helpers
{
    public class IsExpenseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is ExpenseTransactionModel;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }


}

