namespace EA.Weee.Web.Areas.Admin.ViewModels.Obligations
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;
    using EA.Weee.Core.Shared;

    public class UploadObligationsViewModel
    {
        public CompetentAuthority Authority { get; set; }

        [DisplayName("Choose file")]
        [Required(ErrorMessage = "You must select a file before you can upload it")]
        public HttpPostedFileBase File { get; set; }

        public UploadObligationsViewModel(CompetentAuthority authority)
        {
            Authority = authority;
            SchemeObligations = new List<SchemeObligationViewModel>();
        }

        public UploadObligationsViewModel()
        {
            SchemeObligations = new List<SchemeObligationViewModel>();
        }

        public bool DisplayDataError { get; set; }

        public int NumberOfDataErrors { get; set; }

        public bool DisplayFormatError { get; set; }

        public bool DisplaySelectFileError { get; set; }

        public bool AnyError => DisplayDataError || DisplayFormatError || DisplaySelectFileError;

        public bool DisplaySuccessMessage { get; set; }

        public List<SchemeObligationViewModel> SchemeObligations { get; set; }

        public bool AnyObligation => SchemeObligations.Any();
    }
}