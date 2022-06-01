namespace EA.Weee.Web.Services
{
    using System.Web;

    public interface IFileConverterService
    {
        byte[] Convert(HttpPostedFileBase file);

        byte[] ConvertCsv(HttpPostedFileBase file);
    }
}
