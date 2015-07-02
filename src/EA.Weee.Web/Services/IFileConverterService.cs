namespace EA.Weee.Web.Services
{
    using System.Web;

    public interface IFileConverterService
    {
        string Convert(HttpPostedFileBase file);
    }
}
