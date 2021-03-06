﻿namespace EA.Weee.Web.Areas.AatfReturn.Requests
{
    using EA.Prsd.Core;
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
                var householdValue = ConvertStringToDecimal(categoryValue.B2C);
                var nonHouseholdValue = ConvertStringToDecimal(categoryValue.B2B);

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

        private decimal? ConvertStringToDecimal(string input)
        {
            decimal? value = null;
            if (!string.IsNullOrWhiteSpace(input))
            {
                value = Convert.ToDecimal(input);
            }

            return value;
        }
    }
}