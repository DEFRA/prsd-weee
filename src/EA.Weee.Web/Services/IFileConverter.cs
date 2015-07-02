namespace EA.Weee.Web.Services
{
    using System.Web;

    public interface IFileConverter
    {
        string Convert(HttpPostedFileBase file);
    }
}
