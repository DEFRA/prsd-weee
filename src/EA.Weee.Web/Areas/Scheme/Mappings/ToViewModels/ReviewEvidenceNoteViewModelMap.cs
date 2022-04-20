namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModel
{
    using Core.AatfEvidence;
    using EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using Prsd.Core;
    using Prsd.Core.Mapper;

    public class ReviewEvidenceNoteViewModelMap : IMap<ViewEvidenceNoteMapTransfer, ReviewEvidenceNoteViewModel>
    {
        private readonly IMapper mapper;

        public ReviewEvidenceNoteViewModelMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public ReviewEvidenceNoteViewModel Map(ViewEvidenceNoteMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var model = new ReviewEvidenceNoteViewModel()
            {
                ViewEvidenceNoteViewModel = mapper.Map<ViewEvidenceNoteViewModel>(source),
                EvidenceNoteApprovalOptionsViewModel = new EvidenceNoteApprovalOptionsViewModel(), 
                SubmittedDate = source.EvidenceNoteData.SubmittedDate.HasValue ? source.EvidenceNoteData.SubmittedDate.Value.ToString() + " (GMT)" : string.Empty
            };

            //var model = new ReviewEvidenceNoteViewModel
            //{
            //    Id = source.EvidenceNoteData.Id,
            //    OrganisationId = source.EvidenceNoteData.OrganisationData.Id,
            //    AatfId = source.EvidenceNoteData.AatfData.Id,
            //    Reference = source.EvidenceNoteData.Reference,
            //    Status = source.EvidenceNoteData.Status,
            //    Type = source.EvidenceNoteData.Type,
            //    StartDate = source.EvidenceNoteData.StartDate,
            //    EndDate = source.EvidenceNoteData.EndDate,
            //    ProtocolValue = source.EvidenceNoteData.Protocol,
            //    WasteTypeValue = source.EvidenceNoteData.WasteType,
            //    OperatorAddress = addressUtilities.FormattedAddress(source.EvidenceNoteData.OrganisationData.OrganisationName,
            //        source.EvidenceNoteData.OrganisationData.BusinessAddress.Address1,
            //        source.EvidenceNoteData.OrganisationData.BusinessAddress.Address2,
            //        source.EvidenceNoteData.OrganisationData.BusinessAddress.TownOrCity,
            //        source.EvidenceNoteData.OrganisationData.BusinessAddress.CountyOrRegion,
            //        source.EvidenceNoteData.OrganisationData.BusinessAddress.Postcode),
            //    SiteAddress = addressUtilities.FormattedAddress(source.EvidenceNoteData.AatfData.SiteAddress.Name,
            //        source.EvidenceNoteData.AatfData.SiteAddress.Address1,
            //        source.EvidenceNoteData.AatfData.SiteAddress.Address2,
            //        source.EvidenceNoteData.AatfData.SiteAddress.TownOrCity,
            //        source.EvidenceNoteData.AatfData.SiteAddress.CountyOrRegion,
            //        source.EvidenceNoteData.AatfData.SiteAddress.Postcode,
            //        source.EvidenceNoteData.AatfData.ApprovalNumber),
            //    RecipientAddress = addressUtilities.FormattedAddress(source.EvidenceNoteData.SchemeData.SchemeName,
            //        source.EvidenceNoteData.SchemeData.Address.Address1,
            //        source.EvidenceNoteData.SchemeData.Address.Address2,
            //        source.EvidenceNoteData.SchemeData.Address.TownOrCity,
            //        source.EvidenceNoteData.SchemeData.Address.CountyOrRegion,
            //        source.EvidenceNoteData.SchemeData.Address.Postcode)
            //};

            //foreach (var tonnageData in source.EvidenceNoteData.EvidenceTonnageData)
            //{
            //    var category = model.CategoryValues.FirstOrDefault(c => c.CategoryId == (int)tonnageData.CategoryId);

            //    if (category != null)
            //    {
            //        category.Received = tonnageUtilities.CheckIfTonnageIsNull(tonnageData.Received);
            //        category.Reused = tonnageUtilities.CheckIfTonnageIsNull(tonnageData.Reused);
            //        category.Id = tonnageData.Id;
            //    }
            //}

            //SetSuccessMessage(source.EvidenceNoteData, source.NoteStatus, model);

            return model;
        }

        private void SetSuccessMessage(EvidenceNoteData note, object noteStatus, ViewEvidenceNoteViewModel model)
        {
            if (noteStatus != null)
            {
                if (noteStatus is NoteStatus status)
                {
                    model.SuccessMessage = (status == NoteStatus.Approved ?
                        $"You have successfully approved the evidence note with reference ID {note.Reference}" : 
                        $"You have successfully saved the evidence note with reference ID {note.Reference} as a draft");

                    model.Status = status;
                }
                //else
                //{
                //    model.Status = NoteStatus.Draft;
                //}
            }
        }
    }
}