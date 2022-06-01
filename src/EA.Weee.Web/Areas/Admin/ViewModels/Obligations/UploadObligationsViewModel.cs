namespace EA.Weee.Web.Areas.Admin.ViewModels.Obligations
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Web;
    using EA.Weee.Core.Shared;

    public class UploadObligationsViewModel
    {
        public CompetentAuthority Authority { get; set; }

        [DisplayName("Choose file")]
        [Required(ErrorMessage = "You must select a file before the system can check for errors")]
        public HttpPostedFileBase File { get; set; }

        public UploadObligationsViewModel(CompetentAuthority authority)
        {
            Authority = authority;
        }

        public UploadObligationsViewModel()
        {
        }

        public bool DisplayDataError { get; set; }

        public int NumberOfDataErrors { get; set; }

        public bool DisplayFormatError { get; set; }

        public bool DisplaySelectFileError { get; set; }

        public bool AnyError => DisplayDataError || DisplayFormatError || DisplaySelectFileError;

        public bool DisplaySuccessMessage { get; set; }
    }
}