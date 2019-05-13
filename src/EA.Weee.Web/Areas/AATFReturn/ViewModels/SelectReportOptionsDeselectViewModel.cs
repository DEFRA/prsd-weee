namespace EA.Weee.Web.Areas.AatfReturn.Views.SelectReportOptionsDeselect
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.ViewModels.Shared;

    public class SelectReportOptionsDeselectViewModel : RadioButtonStringCollectionViewModel
    {
        public SelectReportOptionsDeselectViewModel() : base(new List<string> { "Yes", "No" })
        {
        }

        public Guid ReturnId { get; set; }

        public Guid OrganisationId { get; set; }

        public IList<ReportOnQuestion> ReportOnQuestions { get; set; }

        public IList<int> SelectedOptions { get;set; }

        public string DcfSelectedValue { get; set; }

        public IList<int> DeselectedOptions { get; set; }

        [Required(ErrorMessage = "You must tell us if you want to remove these reporting options")]
        public override string SelectedValue { get; set; }
    }
}