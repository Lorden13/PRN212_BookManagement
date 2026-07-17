using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using BookManagement.WPF.Models;

namespace BookManagement.WPF.Helpers
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
                return b ? Visibility.Visible : Visibility.Collapsed;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is Visibility v && v == Visibility.Visible;
        }
    }

    public class InverseBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
                return b ? Visibility.Collapsed : Visibility.Visible;
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is Visibility v && v == Visibility.Collapsed;
        }
    }

    public class InverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b) return !b;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b) return !b;
            return false;
        }
    }

    public class StatusToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = value?.ToString() ?? "";
            return status switch
            {
                "Pending" => new SolidColorBrush(Color.FromRgb(245, 158, 11)),
                "Approved" => new SolidColorBrush(Color.FromRgb(16, 185, 129)),
                "Rejected" => new SolidColorBrush(Color.FromRgb(239, 68, 68)),
                "Completed" => new SolidColorBrush(Color.FromRgb(16, 185, 129)),
                "Processing" => new SolidColorBrush(Color.FromRgb(245, 158, 11)),
                "Refunded" => new SolidColorBrush(Color.FromRgb(239, 68, 68)),
                "Active" => new SolidColorBrush(Color.FromRgb(16, 185, 129)),
                "Inactive" => new SolidColorBrush(Color.FromRgb(148, 163, 184)),
                _ => new SolidColorBrush(Color.FromRgb(148, 163, 184))
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StatusToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = value?.ToString() ?? "";
            return status switch
            {
                "Pending" => new SolidColorBrush(Color.FromArgb(30, 245, 158, 11)),
                "Approved" => new SolidColorBrush(Color.FromArgb(30, 16, 185, 129)),
                "Rejected" => new SolidColorBrush(Color.FromArgb(30, 239, 68, 68)),
                "Completed" => new SolidColorBrush(Color.FromArgb(30, 16, 185, 129)),
                "Processing" => new SolidColorBrush(Color.FromArgb(30, 245, 158, 11)),
                "Active" => new SolidColorBrush(Color.FromArgb(30, 16, 185, 129)),
                "Inactive" => new SolidColorBrush(Color.FromArgb(30, 148, 163, 184)),
                _ => new SolidColorBrush(Color.FromArgb(30, 148, 163, 184))
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StringToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is string colorStr && !string.IsNullOrEmpty(colorStr))
                {
                    var color = (Color)ColorConverter.ConvertFromString(colorStr);
                    return new SolidColorBrush(color);
                }
            }
            catch { }
            return new SolidColorBrush(Color.FromRgb(37, 99, 235));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RoleToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is UserRole role)
                return role.ToString();
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class EqualityToBoolConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2) return false;
            return values[0] != null && values[1] != null && values[0].ToString() == values[1].ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
