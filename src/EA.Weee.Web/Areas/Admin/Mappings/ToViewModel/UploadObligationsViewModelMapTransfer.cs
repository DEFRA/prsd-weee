namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using Core.Admin.Obligation;
    using Core.Shared;

    public class UploadObligationsViewModelMapTransfer
    {
        public CompetentAuthority CompetentAuthority { get; set; }

        public SchemeObligationUploadData UploadData { get; set; }
    }
}