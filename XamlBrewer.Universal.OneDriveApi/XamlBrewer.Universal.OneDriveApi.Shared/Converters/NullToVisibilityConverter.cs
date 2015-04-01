namespace XamlBrewer.Universal.OneDriveApi
{
    using System;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Data;

    /// <summary>
    /// Converts null value to a visibility.
    /// </summary>
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            Visibility? v = value as Visibility?;
            return ((v.HasValue) || (v.Value == Visibility.Collapsed)) ? null : "";
        }
    }
}
