namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using EA.Weee.Core.AatfReturn;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using System.Web.Mvc;

    public class SentOnSiteSummaryListViewModel
    {
        public Guid OrganisationId { get; set; }

        public Guid ReturnId { get; set; }

        public Guid AatfId { get; set; }

        public String AatfName { get; set; }

        public List<WeeeSentOnSummaryListData> Sites { get; set; }

        public ObligatedCategoryValue Tonnages { get; set; }

        public bool IsChkCopyPreviousQuarterVisible { get; set; }

        [Required]
        [Range(typeof(bool), "true", "true", ErrorMessage = "Please select copy selection from previous quarter")]
        public bool IsCopyChecked { get; set; }
    }
}