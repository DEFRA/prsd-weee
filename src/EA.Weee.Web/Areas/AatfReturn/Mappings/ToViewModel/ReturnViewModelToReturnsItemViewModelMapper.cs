namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using Core.AatfReturn;
    using Prsd.Core;
    using Prsd.Core.Mapper;
    using ViewModels;

    public class ReturnViewModelToReturnsItemViewModelMapper : IMap<ReturnViewModel, ReturnsItemViewModel>
    {
        private readonly IMapper mapper;

        public ReturnViewModelToReturnsItemViewModelMapper(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public ReturnsItemViewModel Map(ReturnViewModel source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var model = new ReturnsItemViewModel()
            {
                ReturnViewModel = source,
                ReturnsListDisplayOptions = mapper.Map<ReturnsListDisplayOptions>(source.ReturnStatus)
            };

            return model;
        }
    }
}