namespace EA.Weee.Web.ViewModels.Returns.Mappings.ToViewModel
{
    using Core.AatfReturn;
    using Prsd.Core;
    using Prsd.Core.Mapper;

    public class ReturnToReturnsItemViewModelMapper : IMap<ReturnData, ReturnsItemViewModel>
    {
        private readonly IMapper mapper;
        private readonly IMap<ReturnData, ReturnsListRedirectOptions> returnListRedirectMap;
        private readonly IMap<ReturnData, ReturnViewModel> returnMap;

        public ReturnToReturnsItemViewModelMapper(IMapper mapper, 
            IMap<ReturnData, ReturnsListRedirectOptions> returnListRedirectMap,
            IMap<ReturnData, ReturnViewModel> returnMap)
        {
            this.mapper = mapper;
            this.returnListRedirectMap = returnListRedirectMap;
            this.returnMap = returnMap;
        }

        public ReturnsItemViewModel Map(ReturnData source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var model = new ReturnsItemViewModel()
            {
                ReturnViewModel = returnMap.Map(source),
                ReturnsListDisplayOptions = mapper.Map<ReturnsListDisplayOptions>((source.ReturnStatus, source.QuarterWindow)),
                ReturnsListRedirectOptions = returnListRedirectMap.Map(source)
            };

            return model;
        }
    }
}