namespace EA.Weee.Web.Areas.Scheme.ViewModels.DataReturns
{
    using System;
    using Core.DataReturns;

    public class SuccessfulSubmissionViewModel
    {
        public Guid PcsId { get; set; }

        public int ComplianceYear { get; set; }

        public string QuarterText { get; set; }
    }
}