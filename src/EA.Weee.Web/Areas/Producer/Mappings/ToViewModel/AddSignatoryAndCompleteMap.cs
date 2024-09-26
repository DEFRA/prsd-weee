namespace EA.Weee.Web.Areas.Producer.Mappings.ToViewModel
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Web.Areas.Producer.ViewModels;
    using EA.Weee.Web.ViewModels.Shared;

    public class AddSignatoryAndCompleteMap : IMap<SmallProducerSubmissionData, AppropriateSignatoryViewModel>
    {
        public AppropriateSignatoryViewModel Map(SmallProducerSubmissionData source)
        {
            var viewModel = new AppropriateSignatoryViewModel
            {
                DirectRegistrantId = source.DirectRegistrantId,
                OrganisationId = source.OrganisationData.Id,
                HasAuthorisedRepresentitive = source.HasAuthorisedRepresentitive
            };

            return viewModel;
        }
    }
}