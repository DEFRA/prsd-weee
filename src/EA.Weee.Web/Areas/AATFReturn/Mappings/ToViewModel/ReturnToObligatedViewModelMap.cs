namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
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

        public ReturnToObligatedViewModelMap(IWeeeCache cache)
        {
            this.cache = cache;
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

            var obligatedReceived = source.ReturnData.ObligatedWeeeReceivedData;

            foreach (var weeeObligatedData in obligatedReceived.Where(w => w.Aatf.Id == source.AatfId && w.Scheme.Id == source.SchemeId))
            {
                // CHANGE THIS TO A MAPPER
                var category = model.CategoryValues.First(c => c.CategoryId == weeeObligatedData.CategoryId);
                category.B2B = weeeObligatedData.B2B.ToTonnageDisplay();
                category.B2C = weeeObligatedData.B2C.ToTonnageDisplay();
                category.Id = weeeObligatedData.Id;
            }
            return model;
        }
    }
}