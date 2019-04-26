namespace EA.Weee.Web.Areas.AatfReturn.ViewModels.Validation
{
    using Api.Client;
    using Core.AatfReturn;
    using FluentValidation;
    using FluentValidation.Results;
    using FluentValidation.Validators;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Weee.Requests.AatfReturn;

    public class SelectReportOptionsViewModelValidator : AbstractValidator<SelectReportOptionsViewModel>
    {
        public SelectReportOptionsViewModelValidator()
        {
        }
    }
}