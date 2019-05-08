namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.AatfReturn;
    using Prsd.Core.Mapper;
    using ViewModels;

    public class ReturnsToReturnsViewModelMap : IMap<List<ReturnData>, ReturnsViewModel>
    {
        private readonly IMapper mapper;
        private readonly IReturnsOrdering ordering;

        public ReturnsToReturnsViewModelMap(IMapper mapper, IReturnsOrdering ordering)
        {
            this.mapper = mapper;
            this.ordering = ordering;
        }

        public ReturnsViewModel Map(List<ReturnData> source)
        {
            var model = new ReturnsViewModel();

            var orderedItems = ordering.Order(source);
            foreach (var @return in orderedItems)
            {
                model.Returns.Add(mapper.Map<ReturnViewModel>(@return));
            }

            return model;
        }
    }
}