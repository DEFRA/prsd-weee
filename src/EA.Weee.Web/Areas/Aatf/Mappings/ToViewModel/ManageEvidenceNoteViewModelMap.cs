namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using System.Linq;
    using Core.AatfReturn;
    using CuttingEdge.Conditions;
    using EA.Weee.Web.Extensions;
    using EA.Weee.Web.ViewModels.Shared.Mapping;
    using Prsd.Core;
    using Prsd.Core.Mapper;
    using ViewModels;

    public class ManageEvidenceNoteViewModelMap : IMap<ManageEvidenceNoteTransfer, ManageEvidenceNoteViewModel>
    {
        public ManageEvidenceNoteViewModel Map(ManageEvidenceNoteTransfer source)
        {
            Condition.Requires(source).IsNotNull();

            var singleAatf = source.Aatfs.Where(a =>
                a.FacilityType.Equals(FacilityType.Aatf) && 
                ((int)a.ComplianceYear).Equals(source.SelectedComplianceYear));  //TODO: check this 

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

            model.ComplianceYearList = source.ComplianceYearList;
            model.SelectedComplianceYear = source.SelectedComplianceYear;

            return model;
        }
    }
}