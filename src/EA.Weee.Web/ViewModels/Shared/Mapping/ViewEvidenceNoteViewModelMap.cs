﻿namespace EA.Weee.Web.ViewModels.Shared.Mapping
{
    using System.Linq;
    using Core.AatfEvidence;
    using EA.Weee.Web.Extensions;
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

            var organisationAddress = source.EvidenceNoteData.RecipientOrganisationData.HasBusinessAddress
                ? source.EvidenceNoteData.RecipientOrganisationData.BusinessAddress
                : source.EvidenceNoteData.RecipientOrganisationData.NotificationAddress;

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
                SubmittedDate = source.EvidenceNoteData.SubmittedDate.ToDisplayGMTDateTimeString(),
                ApprovedDate = source.EvidenceNoteData.ApprovedDate.ToDisplayGMTDateTimeString(),
                ReturnedDate = source.EvidenceNoteData.ReturnedDate.ToDisplayGMTDateTimeString(),
                Reason = source.EvidenceNoteData.Reason,
                ProtocolValue = source.EvidenceNoteData.Protocol,
                WasteTypeValue = source.EvidenceNoteData.WasteType,
                SubmittedBy = source.EvidenceNoteData.SubmittedDate.HasValue ? source.EvidenceNoteData.AatfData.Name : string.Empty,
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
                    source.EvidenceNoteData.RecipientOrganisationData.OrganisationName,
                    organisationAddress.Address1,
                    organisationAddress.Address2,
                    organisationAddress.TownOrCity,
                    organisationAddress.CountyOrRegion,
                    organisationAddress.Postcode,
                    null),
                SchemeId = source.SchemeId,
                AatfApprovalNumber = source.EvidenceNoteData.AatfData.ApprovalNumber
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

            model.TotalReceivedDisplay = model.ReceivedTotal;

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
                            model.SuccessMessage = $"You have approved the evidence note with reference ID E{note.Reference}";
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