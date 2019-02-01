namespace EA.Weee.Web.Areas.AatfReturn.Requests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using ViewModels;
    using Web.Requests.Base;
    using Weee.Requests.AatfReturn;
    using Weee.Requests.AatfReturn.NonObligated;

    public class NonObligatedWeeRequestCreator : RequestCreator<NonObligatedValuesViewModel, AddNonObligatedRequest>, INonObligatedWeeRequestCreator
    {
        public override AddNonObligatedRequest ViewModelToRequest(NonObligatedValuesViewModel viewModel)
        {
            var nonObligatedRequestValues = new List<NonObligatedRequestValue>();

            foreach (var nonObligatedCategoryValue in viewModel.CategoryValues)
            {
                decimal? value = null;
                if (!string.IsNullOrWhiteSpace(nonObligatedCategoryValue.Tonnage))
                {
                    value = Convert.ToDecimal(nonObligatedCategoryValue.Tonnage);
                }

                nonObligatedRequestValues.Add(
                    new NonObligatedRequestValue(
                        nonObligatedCategoryValue.CategoryId,
                        value,
                        false));
            }

            return new AddNonObligatedRequest() { CategoryValues = nonObligatedRequestValues, OrganisationId = viewModel.OrganisationId, Dcf = viewModel.Dcf, ReturnId = viewModel.ReturnId };
        }
    }
}