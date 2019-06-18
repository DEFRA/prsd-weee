namespace EA.Weee.Web.ViewModels.Returns.Mappings.ToViewModel
{
    using Core.AatfReturn;
    using Prsd.Core;
    using Prsd.Core.Mapper;
    using Returns;

    public class ReturnToSubmittedReturnViewModelMap : IMap<ReturnData, SubmittedReturnViewModel>
    {
        public SubmittedReturnViewModel Map(ReturnData source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var model = new SubmittedReturnViewModel(source)
            {
                OrganisationId = source.OrganisationData.Id
            };

            return model;
        }
    }
}