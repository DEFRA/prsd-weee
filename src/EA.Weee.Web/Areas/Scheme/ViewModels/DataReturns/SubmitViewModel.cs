namespace EA.Weee.Web.Areas.Scheme.ViewModels.DataReturns
{
    using Core.DataReturns;
    using Prsd.Core.Validation;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class SubmitViewModel
    {
        public DataReturnForSubmission DataReturn { get; set; }

        [MustBeTrue(ErrorMessage = "Please confirm that you have read the privacy policy")]
        public bool PrivacyPolicy { get; set; }
    }
}