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

            return Encoding.UTF8.GetString(data);
        }
    }
}