namespace EA.Weee.Web.Areas.AatfReturn.Requests
{
    using System;
    using System.Collections.Generic;
    using EA.Prsd.Core;
    using ViewModels;
    using Web.Requests.Base;
    using Weee.Requests.AatfReturn.ObligatedReceived;

    public class ObligatedReceivedWeeeRequestCreator : RequestCreator<ObligatedReceivedViewModel, AddObligatedReceived>, IObligatedReceivedWeeeRequestCreator
    {
        public override AddObligatedReceived ViewModelToRequest(ObligatedReceivedViewModel viewModel)
        {
            Guard.ArgumentNotNull(() => viewModel, viewModel);
            var obligatedRequestValues = new List<ObligatedReceivedValue>();

            foreach (var categoryValue in viewModel.CategoryValues)
            {
                var householdValue = ConvertStringToDecimal(categoryValue.B2C);
                var nonHouseholdValue = ConvertStringToDecimal(categoryValue.B2B);

                obligatedRequestValues.Add(
                    new ObligatedReceivedValue(
                        categoryValue.CategoryId,
                        householdValue,
                        nonHouseholdValue));
            }

            return new AddObligatedReceived() { OrganisationId = viewModel.OrganisationId, ReturnId = viewModel.ReturnId, CategoryValues = obligatedRequestValues };
        }

        public decimal? ConvertStringToDecimal(string input)
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