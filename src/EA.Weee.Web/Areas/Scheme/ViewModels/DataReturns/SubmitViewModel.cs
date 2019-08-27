namespace EA.Weee.Web.Areas.Scheme.ViewModels.DataReturns
{
    using Core.DataReturns;
    using System;

    public class SubmitViewModel
    {
        public Guid PcsId { get; set; }

        public DataReturnForSubmission DataReturn { get; set; }
    }
}