namespace EA.Weee.Web.Services
{
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web;

    public class FileConverterService : IFileConverterService
    {
        public byte[] Convert(HttpPostedFileBase file)
        {
            var reader = new BinaryReader(file.InputStream);
            return reader.ReadBytes((int)file.InputStream.Length);
        }
    }
}