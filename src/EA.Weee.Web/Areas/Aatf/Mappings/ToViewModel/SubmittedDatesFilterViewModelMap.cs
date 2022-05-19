﻿namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Web.Areas.Aatf.ViewModels;

    public class SubmittedDatesFilterViewModelMap : IMap<SubmittedDateFilterBase, SubmittedDatesFilterViewModel>
    {
        public SubmittedDatesFilterViewModel Map(SubmittedDateFilterBase source)
        {
            return new SubmittedDatesFilterViewModel
            {
                StartDate = source.StartDate,
                EndDate = source.EndDate
            };
        }
    }
}
