namespace EA.Weee.Web.Areas.Producer.Mappings.ToRequest
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Core.Shared;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using EA.Weee.Web.Areas.Producer.ViewModels;
    using EA.Weee.Web.Requests.Base;
    using System.Collections.Generic;

    public class ToEditEeeDataRequestCreator : IRequestCreator<EditEeeDataViewModel, EditEeeDataRequest>
    {
        private readonly IMapper mapper;

        public ToEditEeeDataRequestCreator(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public EditEeeDataRequest ViewModelToRequest(EditEeeDataViewModel viewModel)
        {
            var eeeList = new List<Eee>();

            foreach (var categoryValue in viewModel.CategoryValues)
            {
                if (decimal.TryParse(categoryValue.HouseHold, out var householdTonnage))
                {
                    eeeList.Add(new Eee(tonnage: householdTonnage,
                        category: (WeeeCategory)categoryValue.CategoryId,
                        obligationType: ObligationType.B2C));
                }

                if (decimal.TryParse(categoryValue.NonHouseHold, out var nonHouseholdTonnage))
                {
                    eeeList.Add(new Eee(tonnage: nonHouseholdTonnage,
                        category: (WeeeCategory)categoryValue.CategoryId,
                        obligationType: ObligationType.B2B));
                }
            }

            return new EditEeeDataRequest(viewModel.DirectRegistrantId, eeeList,
                viewModel.SellingTechnique.ToSellingTechniqueType());
        }
    }
}