namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using EA.Prsd.Core;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.DataReturns;
    using FluentValidation.Attributes;
    using Validation;

    public abstract class SelectReportOptionsModelBase
    {
        public SelectReportOptionsModelBase()
        {
        }

        public Guid OrganisationId { get; set; }

        public Guid ReturnId { get; set; }

        public IList<ReportOnQuestion> ReportOnQuestions { get; set; }

        public bool HasSelectedOptions => SelectedOptions != null && SelectedOptions.Count != 0;

        public List<int> SelectedOptions { get; set; }

        public List<int> DeselectedOptions { get; set; }

        public string DcfSelectedValue { get; set; }
    }
}