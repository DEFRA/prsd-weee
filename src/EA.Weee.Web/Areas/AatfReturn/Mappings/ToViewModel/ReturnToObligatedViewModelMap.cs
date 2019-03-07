namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.AatfReturn;
    using Core.Helpers;
    using Prsd.Core;
    using Prsd.Core.Mapper;
    using Services.Caching;
    using ViewModels;

    public class ReturnToObligatedViewModelMap : IMap<ReturnToObligatedViewModelTransfer, ObligatedViewModel>
    {
        private readonly IWeeeCache cache;
        private readonly IMap<ObligatedDataToObligatedValueMapTransfer, IList<ObligatedCategoryValue>> obligatedMap;

        public ReturnToObligatedViewModelMap(IWeeeCache cache,
            IMap<ObligatedDataToObligatedValueMapTransfer, IList<ObligatedCategoryValue>> obligatedMap)
        {
            this.cache = cache;
            this.obligatedMap = obligatedMap;
        }

        public ObligatedViewModel Map(ReturnToObligatedViewModelTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var model = new ObligatedViewModel(new ObligatedCategoryValues())
            {
                SchemeName = cache.FetchSchemePublicInfo(source.OrganisationId).Result.Name,
                AatfName = cache.FetchAatfData(source.OrganisationId, source.AatfId).Result.Name,
                AatfId = source.AatfId,
                OrganisationId = source.OrganisationId,
                ReturnId = source.ReturnId,
                SchemeId = source.SchemeId
            };

            model.CategoryValues = obligatedMap.Map(new ObligatedDataToObligatedValueMapTransfer() { WeeeDataValues = source.ReturnData.ObligatedWeeeReceivedData.Where(w => w.Aatf.Id == source.AatfId && w.Scheme.Id == source.SchemeId).ToList(), ObligatedCategoryValues = model.CategoryValues });

            return model;
        }
    }
}