using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace BookManagement.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public bool Invert { get; set; }
        public bool UseHidden { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool val = value is bool b && b;
            if (Invert) val = !val;

            return val ? Visibility.Visible : (UseHidden ? Visibility.Hidden : Visibility.Collapsed);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is Visibility vis && vis == Visibility.Visible;
        }
    }

    public class NullToVisibilityConverter : IValueConverter
    {
        public bool Invert { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isNull = value == null;
            bool visible = Invert ? isNull : !isNull;
            return visible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StringNullOrEmptyToVisibilityConverter : IValueConverter
    {
        public bool Invert { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isEmpty = string.IsNullOrEmpty(value as string);
            bool visible = Invert ? isEmpty : !isEmpty;
            return visible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StatusToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = value as string;
            if (string.IsNullOrEmpty(status)) return Brushes.Gray;

            switch (status.ToLower())
            {
                case "approved":
                case "active":
                case "success":
                    return Application.Current.TryFindResource("SuccessBrush") ?? Brushes.Green;
                case "pending":
                case "warning":
                    return Application.Current.TryFindResource("WarningBrush") ?? Brushes.Orange;
                case "need revision":
                case "revision":
                case "need_revision":
                    return Application.Current.TryFindResource("PrimaryPurpleBrush") ?? Brushes.Purple;
                case "rejected":
                case "inactive":
                case "danger":
                    return Application.Current.TryFindResource("DangerBrush") ?? Brushes.Red;
                case "draft":
                default:
                    return Application.Current.TryFindResource("TextSecondaryBrush") ?? Brushes.SlateGray;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StatusToBackgroundBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = value as string;
            if (string.IsNullOrEmpty(status)) return Brushes.LightGray;

            switch (status.ToLower())
            {
                case "approved":
                case "active":
                case "success":
                    return Application.Current.TryFindResource("SuccessLightBrush") ?? Brushes.LightGreen;
                case "pending":
                case "warning":
                    return Application.Current.TryFindResource("WarningLightBrush") ?? Brushes.LightGoldenrodYellow;
                case "need revision":
                case "revision":
                case "need_revision":
                    return Application.Current.TryFindResource("PrimaryPurpleLightBrush") ?? Brushes.Lavender;
                case "rejected":
                case "inactive":
                case "danger":
                    return Application.Current.TryFindResource("DangerLightBrush") ?? Brushes.MistyRose;
                case "draft":
                default:
                    return new SolidColorBrush(Color.FromRgb(243, 244, 246)); // light gray #F3F4F6
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ResourceKeyToGeometryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string key = value as string;
            if (string.IsNullOrEmpty(key)) return null;
            return Application.Current.TryFindResource(key) as Geometry;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StatusToIconGeometryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = value as string;
            if (string.IsNullOrEmpty(status)) return Application.Current.TryFindResource("IconBell") as Geometry;

            string key = "IconBell";
            switch (status.ToLower())
            {
                case "approved":
                case "active":
                case "success":
                    key = "IconCheck";
                    break;
                case "pending":
                case "warning":
                case "need revision":
                case "revision":
                case "need_revision":
                    key = "IconShield";
                    break;
                case "rejected":
                case "inactive":
                case "danger":
                    key = "IconClose";
                    break;
            }
            return Application.Current.TryFindResource(key) as Geometry;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
