﻿namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using System.ComponentModel.DataAnnotations;
    using System.Web;

    public class AddOrAmendMembersViewModel
    {
        [Required(ErrorMessage = "You must choose a file")]
        [Display(Name = "Choose a file")]
        public HttpPostedFileBase File { get; set; }
    }
}