namespace EA.Weee.Web.Areas.Scheme.ViewModels.DataReturns
{
    using System;
    using Core.DataReturns;

    public class SubmitViewModel
    {
        public Guid PcsId { get; set; }

        public DataReturnForSubmission DataReturn { get; set; }
    }
}