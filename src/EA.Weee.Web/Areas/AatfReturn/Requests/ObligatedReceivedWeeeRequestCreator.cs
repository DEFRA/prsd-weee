namespace EA.Weee.Web.Areas.AatfReturn.Requests
{
    using EA.Prsd.Core;
    using EA.Weee.Web.Extensions;
    using System;
    using System.Collections.Generic;
    using ViewModels;
    using Web.Requests.Base;
    using Weee.Requests.AatfReturn.Obligated;

    public class ObligatedReceivedWeeeRequestCreator : RequestCreator<ObligatedViewModel, ObligatedBaseRequest>, IObligatedReceivedWeeeRequestCreator
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
                return new EditObligatedReceived()
                {
                    CategoryValues = obligatedRequestValues
                };
            }

            return new AddObligatedReceived()
            {
                AatfId = viewModel.AatfId,
                SchemeId = viewModel.SchemeId,
                OrganisationId = viewModel.OrganisationId,
                ReturnId = viewModel.ReturnId,
                CategoryValues = obligatedRequestValues
            };
        }
    }
}