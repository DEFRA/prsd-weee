﻿namespace EA.Weee.Web.Areas.AatfReturn.Requests
{
    using EA.Prsd.Core;
    using System;
    using System.Collections.Generic;
    using Extensions;
    using ViewModels;
    using Web.Requests.Base;
    using Weee.Requests.Aatf;
    using Weee.Requests.AatfReturn.Obligated;

    public class ObligatedReusedWeeeRequestCreator : RequestCreator<ObligatedViewModel, ObligatedBaseRequest>, IObligatedReusedWeeeRequestCreator
    {
        public override ObligatedBaseRequest ViewModelToRequest(ObligatedViewModel viewModel)
        {
            Guard.ArgumentNotNull(() => viewModel, viewModel);
            var obligatedRequestValues = new List<TonnageValues>();

            foreach (var categoryValue in viewModel.CategoryValues)
            {
                var householdValue = categoryValue.B2C.ToDecimal();
                var nonHouseholdValue = categoryValue.B2B.ToDecimal();

                obligatedRequestValues.Add(
                    new TonnageValues(categoryValue.Id,
                        categoryValue.CategoryId,
                        householdValue,
                        nonHouseholdValue));
            }

            if (viewModel.Edit)
            {
                return new EditObligatedReused()
                {
                    CategoryValues = obligatedRequestValues
                };
            }

            return new AddObligatedReused()
            {
                AatfId = viewModel.AatfId,
                OrganisationId = viewModel.OrganisationId,
                ReturnId = viewModel.ReturnId,
                CategoryValues = obligatedRequestValues
            };
        }
    }
}