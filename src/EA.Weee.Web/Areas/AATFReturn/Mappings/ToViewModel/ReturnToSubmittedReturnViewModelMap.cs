namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Core.AatfReturn;
    using Prsd.Core;
    using Prsd.Core.Mapper;
    using ViewModels;

    public class ReturnToSubmittedReturnViewModelMap : IMap<ReturnData, SubmittedReturnViewModel>
    {
        public SubmittedReturnViewModel Map(ReturnData source)
        {
            Guard.ArgumentNotNull(() => source, source);

            return new SubmittedReturnViewModel(source.Quarter, source.QuarterWindow, source.Quarter.Year);
        }
    }
}