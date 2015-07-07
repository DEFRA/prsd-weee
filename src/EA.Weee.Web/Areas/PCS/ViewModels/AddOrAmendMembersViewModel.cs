namespace EA.Weee.Web.Areas.PCS.ViewModels
{
    using System.ComponentModel.DataAnnotations;
    using System.Web;
    using Prsd.Core.Validation;

    public class AddOrAmendMembersViewModel
    {
        [Required]
        [HttpPostedFileType("text/xml")]
        public HttpPostedFileBase File { get; set; }
    }
}