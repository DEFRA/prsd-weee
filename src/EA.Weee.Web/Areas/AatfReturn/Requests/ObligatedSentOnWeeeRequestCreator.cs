namespace EA.Weee.Web.Areas.AatfReturn.Requests
{
    using EA.Prsd.Core;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Requests.Base;
    using System;
    using System.Collections.Generic;
    using Extensions;

    public class ObligatedSentOnWeeeRequestCreator : RequestCreator<ObligatedViewModel, ObligatedBaseRequest>, IObligatedSentOnWeeeRequestCreator
    {
        public override ObligatedBaseRequest ViewModelToRequest(ObligatedViewModel viewModel)
        {
            Guard.ArgumentNotNull(() => viewModel, viewModel);
            var obligatedRequestValues = new List<ObligatedValue>();

            foreach (var categoryValue in viewModel.CategoryValues)
            {
                var householdValue = categoryValue.B2C.ToDecimal();
                var nonHouseholdValue = categoryValue.B2B.ToDecimal();

                obligatedRequestValues.Add(
                    new ObligatedValue(categoryValue.Id,
                        categoryValue.CategoryId,
                        householdValue,
                        nonHouseholdValue));
            }

            if (viewModel.Edit)
            {
                return new EditObligatedSentOn()
                {
                    CategoryValues = obligatedRequestValues
                };
            }

            return new AddObligatedSentOn()
            {
                AatfId = viewModel.AatfId,
                OrganisationId = viewModel.OrganisationId,
                ReturnId = viewModel.ReturnId,
                CategoryValues = obligatedRequestValues,
                SiteAddressId = viewModel.SiteAddressId,
                WeeeSentOnId = viewModel.WeeeSentOnId
            };
        }
    }
}