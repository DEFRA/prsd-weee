namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class AatfTaskListViewModel
    {
        public Guid OrganisationId { get; set; }

        public string OrganisationName { get; set; }

        public Guid ReturnId { get; set; }

        [Display(Name = "Quarter")]
        public string Quarter { get; set; }

        [Display(Name = "Reporting period")]
        public string Period { get; set; }

        [Display(Name = "Compliance year")]
        public string Year { get; set; }

        public List<string> Aatfs { get; set; }
    }
}