namespace EA.Weee.Web.Areas.Scheme.ViewModels.DataReturns
{
    using Core.DataReturns;
    using System;

    public class SuccessfulSubmissionViewModel
    {
        public Guid PcsId { get; set; }

        public int ComplianceYear { get; set; }

        public QuarterType Quarter { get; set; }
    }
}