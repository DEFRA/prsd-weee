namespace EA.Weee.Web.Areas.PCS.ViewModels
{
    using System.ComponentModel.DataAnnotations;
    using System.Web;
    using Validators;

    public class ManageMembersViewModel
    {
        [Required]
        [FileType("text/xml")]
        public HttpPostedFileBase File { get; set; }
    }
}