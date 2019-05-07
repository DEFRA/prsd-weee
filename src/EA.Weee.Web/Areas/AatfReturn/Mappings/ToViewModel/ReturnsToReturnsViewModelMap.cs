namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using System.Collections.Generic;
    using Core.AatfReturn;
    using Prsd.Core.Mapper;
    using ViewModels;

    public class ReturnsToReturnsViewModelMap : IMap<IList<ReturnData>, ReturnsViewModel>
    {
        private readonly IMapper mapper;

        public ReturnsToReturnsViewModelMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public ReturnsViewModel Map(IList<ReturnData> source)
        {
            var model = new ReturnsViewModel();

            foreach (var @return in source)
            {
                model.Returns.Add(mapper.Map<ReturnViewModel>(@return));
            }

            return model;
        }
    }
}