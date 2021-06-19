﻿namespace EA.Weee.Web.Areas.AatfReturn.Requests
{
    using System;
    using System.Collections.Generic;
    using ViewModels;
    using Web.Requests.Base;
    using Weee.Requests.AatfReturn.NonObligated;

    public class NonObligatedWeeeRequestCreator : RequestCreator<NonObligatedValuesViewModel, NonObligated>, INonObligatedWeeeRequestCreator
    {
        public override NonObligated ViewModelToRequest(NonObligatedValuesViewModel viewModel)
        {
            var nonObligatedRequestValues = new List<NonObligatedValue>();

            foreach (var nonObligatedCategoryValue in viewModel.CategoryValues)
            {
                decimal? value = null;
                if (!string.IsNullOrWhiteSpace(nonObligatedCategoryValue.Tonnage))
                {
                    value = Convert.ToDecimal(nonObligatedCategoryValue.Tonnage);
                }

                nonObligatedRequestValues.Add(
                    new NonObligatedValue(
                        nonObligatedCategoryValue.CategoryId,
                        value,
                        false,
                        nonObligatedCategoryValue.Id));
            }

            if (viewModel.Edit)
            {
                return new EditNonObligated()
                {
                    CategoryValues = nonObligatedRequestValues,
                    ReturnId = viewModel.ReturnId
                };
            }

            return new AddNonObligated()
            {
                CategoryValues = nonObligatedRequestValues,
                OrganisationId = viewModel.OrganisationId,
                Dcf = viewModel.Dcf,
                ReturnId = viewModel.ReturnId
            };
        }
    }
}