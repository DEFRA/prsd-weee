namespace EA.Weee.Web.Areas.Producer.Mappings.ToViewModel
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Web.Areas.Producer.ViewModels;

    public class EditOrganisationDetailsMap : IMap<SmallProducerSubmissionData, EditOrganisationDetailsViewModel>
    {
        public EditOrganisationDetailsViewModel Map(SmallProducerSubmissionData source)
        {
            return new EditOrganisationDetailsViewModel();
        }
    }
}