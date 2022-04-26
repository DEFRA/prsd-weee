﻿namespace EA.Weee.Web.ViewModels.Shared.Mapping
{
    using System.Linq;
    using Core.AatfEvidence;
    using Prsd.Core;
    using Prsd.Core.Mapper;
    using Returns.Mappings.ToViewModel;
    using Utilities;

    public class ViewEvidenceNoteViewModelMap : IMap<ViewEvidenceNoteMapTransfer, ViewEvidenceNoteViewModel>
    {
        private readonly ITonnageUtilities tonnageUtilities;
        private readonly IAddressUtilities addressUtilities;
        
        public ViewEvidenceNoteViewModelMap(ITonnageUtilities tonnageUtilities, 
            IAddressUtilities addressUtilities)
        {
            this.tonnageUtilities = tonnageUtilities;
            this.addressUtilities = addressUtilities;
        }

        public ViewEvidenceNoteViewModel Map(ViewEvidenceNoteMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var model = new ViewEvidenceNoteViewModel
            {
                Id = source.EvidenceNoteData.Id,
                OrganisationId = source.EvidenceNoteData.OrganisationData.Id,
                AatfId = source.EvidenceNoteData.AatfData.Id,
                Reference = source.EvidenceNoteData.Reference,
                Status = source.EvidenceNoteData.Status,
                Type = source.EvidenceNoteData.Type,
                StartDate = source.EvidenceNoteData.StartDate,
                EndDate = source.EvidenceNoteData.EndDate,
                SubmittedDate = source.EvidenceNoteData.SubmittedDate.HasValue ? source.EvidenceNoteData.SubmittedDate.Value.ToString("dd/MM/yyyy HH:mm:ss \"(GMT)\"") : string.Empty,
                ProtocolValue = source.EvidenceNoteData.Protocol,
                WasteTypeValue = source.EvidenceNoteData.WasteType,
                OperatorAddress = addressUtilities.FormattedAddress(source.EvidenceNoteData.OrganisationData.OrganisationName,
                    source.EvidenceNoteData.OrganisationData.BusinessAddress.Address1,
                    source.EvidenceNoteData.OrganisationData.BusinessAddress.Address2,
                    source.EvidenceNoteData.OrganisationData.BusinessAddress.TownOrCity,
                    source.EvidenceNoteData.OrganisationData.BusinessAddress.CountyOrRegion,
                    source.EvidenceNoteData.OrganisationData.BusinessAddress.Postcode),
                SiteAddress = addressUtilities.FormattedAddress(source.EvidenceNoteData.AatfData.SiteAddress.Name,
                    source.EvidenceNoteData.AatfData.SiteAddress.Address1,
                    source.EvidenceNoteData.AatfData.SiteAddress.Address2,
                    source.EvidenceNoteData.AatfData.SiteAddress.TownOrCity,
                    source.EvidenceNoteData.AatfData.SiteAddress.CountyOrRegion,
                    source.EvidenceNoteData.AatfData.SiteAddress.Postcode,
                    source.EvidenceNoteData.AatfData.ApprovalNumber),
                RecipientAddress = addressUtilities.FormattedCompanyPcsAddress(source.EvidenceNoteData.SchemeData.SchemeName,
                    source.EvidenceNoteData.SchemeData.Name,
                    source.EvidenceNoteData.OrganisationData.BusinessAddress.Address1,
                    source.EvidenceNoteData.OrganisationData.BusinessAddress.Address2,
                    source.EvidenceNoteData.OrganisationData.BusinessAddress.TownOrCity,
                    source.EvidenceNoteData.OrganisationData.BusinessAddress.CountyOrRegion,
                    source.EvidenceNoteData.OrganisationData.BusinessAddress.Postcode,
                    null),
            };

            foreach (var tonnageData in source.EvidenceNoteData.EvidenceTonnageData)
            {
                var category = model.CategoryValues.FirstOrDefault(c => c.CategoryId == (int)tonnageData.CategoryId);

                if (category != null)
                {
                    category.Received = tonnageUtilities.CheckIfTonnageIsNull(tonnageData.Received);
                    category.Reused = tonnageUtilities.CheckIfTonnageIsNull(tonnageData.Reused);
                    category.Id = tonnageData.Id;
                }
            }
            SetSuccessMessage(source.EvidenceNoteData, source.NoteStatus, model);

            return model;
        }

        private void SetSuccessMessage(EvidenceNoteData note, object noteStatus, ViewEvidenceNoteViewModel model)
        {
            if (noteStatus != null)
            {
                if (noteStatus is NoteStatus status)
                {
                    switch (status)
                    {
                        case NoteStatus.Submitted:
                            model.SuccessMessage =
                                $"You have successfully submitted the evidence note with reference ID E{note.Reference}";
                            break;
                        case NoteStatus.Draft:
                            model.SuccessMessage =
                                $"You have successfully saved the evidence note with reference ID E{note.Reference} as a draft";
                            break;
                        case NoteStatus.Approved:
                            model.SuccessMessage = "success message TODO";
                            break;
                    }

                    model.Status = status;
                }
                else
                {
                    model.Status = NoteStatus.Draft;
                }
            }
        }
    }
}