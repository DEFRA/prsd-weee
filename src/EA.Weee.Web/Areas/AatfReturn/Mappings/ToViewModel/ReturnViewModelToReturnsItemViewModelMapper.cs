namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using Core.AatfReturn;
    using Prsd.Core;
    using Prsd.Core.Mapper;
    using ViewModels;

    public class ReturnViewModelToReturnsItemViewModelMapper : IMap<ReturnViewModel, ReturnsItemViewModel>
    {
        private readonly IMapper mapper;
        private readonly IMap<ReturnViewModel, ReturnsListRedirectOptions> returnListRedirectMap;

        public ReturnViewModelToReturnsItemViewModelMapper(IMapper mapper, 
            IMap<ReturnViewModel, ReturnsListRedirectOptions> returnListRedirectMap)
        {
            this.mapper = mapper;
            this.returnListRedirectMap = returnListRedirectMap;
        }

        public ReturnsItemViewModel Map(ReturnViewModel source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var model = new ReturnsItemViewModel()
            {
                ReturnViewModel = source,
                ReturnsListDisplayOptions = mapper.Map<ReturnsListDisplayOptions>(source.ReturnStatus),
                ReturnsListRedirectOptions = returnListRedirectMap.Map(source)
            };

            return model;
        }
    }
}