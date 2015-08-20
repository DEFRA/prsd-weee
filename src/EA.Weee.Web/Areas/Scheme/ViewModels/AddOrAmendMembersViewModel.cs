namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using System.ComponentModel.DataAnnotations;
    using System.Web;
    using Prsd.Core.Validation;

    public class AddOrAmendMembersViewModel
    {
        [Required(ErrorMessage = "You must choose a file")]
        [HttpPostedFileType("text/xml")]
        public HttpPostedFileBase File { get; set; }
    }
}