namespace EA.Weee.Web.Areas.Admin.ViewModels.Obligations
{
    using EA.Weee.Core.Shared;

    public class UploadObligationsViewModel
    {
        public CompetentAuthority Authority { get; set; }

        public UploadObligationsViewModel(CompetentAuthority authority)
        {
            Authority = authority;
        }

        public UploadObligationsViewModel()
        {
        }
    }
}