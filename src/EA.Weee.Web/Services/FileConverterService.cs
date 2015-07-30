namespace EA.Weee.Web.Services
{
    using System.IO;
    using System.Text;
    using System.Web;

    public class FileConverterService : IFileConverterService
    {
        public string Convert(HttpPostedFileBase file)
        {
            var reader = new BinaryReader(file.InputStream);
            var data = reader.ReadBytes((int)file.InputStream.Length);

            string result = Encoding.UTF8.GetString(data);

            // If the string is a representation of XML and starts with a UTF-8 byte
            // order mark, we need to remove it otherwise XDocument.Parse will fail.
            //string utf8BOM = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
            //if (result.StartsWith(utf8BOM))
            //{
            //    result = result.Remove(0, utf8BOM.Length);
            //}

            return result;
        }
    }
}