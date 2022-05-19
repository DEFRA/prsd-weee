namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using System.Linq;
    using Core.AatfReturn;
    using Prsd.Core;
    using Prsd.Core.Mapper;
    using ViewModels;

    public class ManageEvidenceNoteViewModelMap : IMap<ManageEvidenceNoteTransfer, ManageEvidenceNoteViewModel>
    {
        public ManageEvidenceNoteViewModel Map(ManageEvidenceNoteTransfer source)
        {
            var singleAatf = source.Aatfs.Where(a =>
                a.FacilityType.Equals(FacilityType.Aatf) && ((int)a.ComplianceYear).Equals(SystemTime.Now.Year));

            var model = new ManageEvidenceNoteViewModel()
            {
                OrganisationId = source.OrganisationId, 
                AatfId = source.AatfId, 
                AatfName = source.AatfData.Name, 
                SingleAatf = singleAatf.Count().Equals(1)
            };

            if (source.FilterViewModel != null)
            {
                model.FilterViewModel = source.FilterViewModel;
            }

            if (source.RecipientWasteStatusFilterViewModel != null)
            {
                model.RecipientWasteStatusFilterViewModel = source.RecipientWasteStatusFilterViewModel;
            }

            if (source.SubmittedDatesFilterViewModel != null)
            {
                model.SubmittedDatesFilterViewModel = source.SubmittedDatesFilterViewModel;
            }

            return model;
        }
    }
}