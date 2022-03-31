namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using Prsd.Core;
    using Prsd.Core.Mapper;
    using ViewModels;

    public class ViewEvidenceNoteViewModelMap : IMap<ViewEvidenceNoteMapTransfer, ViewEvidenceNoteViewModel>
    {
        public ViewEvidenceNoteViewModel Map(ViewEvidenceNoteMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);

            //var singleAatf = source.Aatfs.Where(a => a.FacilityType.Equals(FacilityType.Aatf) && ((int)a.ComplianceYear).Equals(SystemTime.Now.Year));

            var model = new ViewEvidenceNoteViewModel
            {
                OrganisationId = source.OrganisationId,
                AatfId = source.AatfId,
                Reference = source.EvidenceNoteData.Reference
            };

            return model;
        }
    }
}