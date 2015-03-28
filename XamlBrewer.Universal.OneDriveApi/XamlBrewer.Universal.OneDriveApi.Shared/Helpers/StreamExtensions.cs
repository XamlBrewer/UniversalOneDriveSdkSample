namespace XamlBrewer.Universal.OneDriveApi
{
    using System.IO;
    using System.Text;

    public static class StreamExtensions
    {
        public static Stream AsStream(this string text)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(text);
            return new MemoryStream(byteArray);
        }
    }
}
