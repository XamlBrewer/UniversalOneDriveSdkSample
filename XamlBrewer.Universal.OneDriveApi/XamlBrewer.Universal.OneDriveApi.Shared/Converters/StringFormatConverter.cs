namespace XamlBrewer.Universal.OneDriveApi
{
    using System;
    using Windows.UI.Xaml.Data;

    public class StringFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter == null)
            {
                return value;
            }

            return String.Format((String)parameter, value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
