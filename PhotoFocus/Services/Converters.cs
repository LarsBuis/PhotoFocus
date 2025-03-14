using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace PhotoFocus.Services
{
    // Converts a Boolean membership status to a user-friendly text.
    public class BoolToMembershipStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isMember = (bool)value;
            return isMember ? "Membership Active" : "No Active Membership";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Inverts a Boolean value (true becomes false and vice versa).
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
                return !b;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
                return !b;
            return false;
        }
    }
}
