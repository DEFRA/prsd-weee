namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using System.Collections.Generic;
    using System.Linq;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Services.Caching;

    public class ReturnToNonObligatedValuesViewModelMap : IMap<ReturnToNonObligatedValuesViewModelMapTransfer, NonObligatedValuesViewModel>
    {
        private readonly IWeeeCache cache;
        private readonly IMap<NonObligatedDataToNonObligatedValueMapTransfer, IList<NonObligatedCategoryValue>> nonObligatedMap;
        private readonly ICategoryValueTotalCalculator calculator;

        public ReturnToNonObligatedValuesViewModelMap(IWeeeCache cache,
            IMap<NonObligatedDataToNonObligatedValueMapTransfer,
            IList<NonObligatedCategoryValue>> nonObligatedMap,
            ICategoryValueTotalCalculator calculator)
        {
            this.cache = cache;
            this.nonObligatedMap = nonObligatedMap;
            this.calculator = calculator;
        }

        public NonObligatedValuesViewModel Map(ReturnToNonObligatedValuesViewModelMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var viewModel = new NonObligatedValuesViewModel(calculator)
            {
                ReturnId = source.ReturnId,
                OrganisationId = source.OrganisationId,
                Dcf = source.Dcf
            };

            viewModel.CategoryValues = nonObligatedMap.Map(new NonObligatedDataToNonObligatedValueMapTransfer() { NonObligatedDataValues = source.ReturnData.NonObligatedData.ToList(), NonObligatedCategoryValues = viewModel.CategoryValues, Dcf = source.Dcf });

            return viewModel;
        }
    }
}