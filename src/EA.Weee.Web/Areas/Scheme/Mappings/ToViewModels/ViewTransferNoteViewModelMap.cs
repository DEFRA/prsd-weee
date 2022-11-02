﻿namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using Core.AatfEvidence;
    using CuttingEdge.Conditions;
    using EA.Weee.Web.Extensions;
    using EA.Weee.Web.ViewModels.Returns.Mappings.ToViewModel;
    using EA.Weee.Web.ViewModels.Shared;
    using Prsd.Core.Mapper;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Helpers;
    using Core.Shared;
    using ViewModels;
    using System.Security.Claims;
    using EA.Weee.Security;
    using System.Security.Principal;

    public class ViewTransferNoteViewModelMap : IMap<ViewTransferNoteViewModelMapTransfer, ViewTransferNoteViewModel>
    {
        private readonly IAddressUtilities addressUtilities;
        private readonly ITonnageUtilities tonnageUtilities;

        public ViewTransferNoteViewModelMap(IAddressUtilities addressUtilities, ITonnageUtilities tonnageUtilities)
        {
            this.addressUtilities = addressUtilities;
            this.tonnageUtilities = tonnageUtilities;
        }

        public ViewTransferNoteViewModel Map(ViewTransferNoteViewModelMapTransfer source)
        {
            Condition.Requires(source).IsNotNull();

            var recipientOrganisationAddress = source.TransferEvidenceNoteData.RecipientOrganisationData.HasBusinessAddress
                ? source.TransferEvidenceNoteData.RecipientOrganisationData.BusinessAddress
                : source.TransferEvidenceNoteData.RecipientOrganisationData.NotificationAddress;

            var transferOrganisationAddress = source.TransferEvidenceNoteData.TransferredOrganisationData.HasBusinessAddress
                ? source.TransferEvidenceNoteData.TransferredOrganisationData.BusinessAddress
                : source.TransferEvidenceNoteData.TransferredOrganisationData.NotificationAddress;

            string transferredByFormattedAddress;

            if (source.TransferEvidenceNoteData.TransferredOrganisationData.IsBalancingScheme)
            {
                transferredByFormattedAddress =
                    source.TransferEvidenceNoteData.TransferredOrganisationData.OrganisationName;
            }
            else
            {
                transferredByFormattedAddress = string.IsNullOrEmpty(source.TransferEvidenceNoteData.ApprovedTransfererDetails)
                    ? addressUtilities.FormattedCompanyPcsAddress(
                    source.TransferEvidenceNoteData.TransferredSchemeData.SchemeName,
                    source.TransferEvidenceNoteData.TransferredOrganisationData.OrganisationName,
                    transferOrganisationAddress.Address1,
                    transferOrganisationAddress.Address2,
                    transferOrganisationAddress.TownOrCity,
                    transferOrganisationAddress.CountyOrRegion,
                    transferOrganisationAddress.Postcode)
                    : source.TransferEvidenceNoteData.ApprovedTransfererDetails;
            }

            var recipientAddress = source.TransferEvidenceNoteData.RecipientOrganisationData.IsBalancingScheme
                ? source.TransferEvidenceNoteData.RecipientOrganisationData.OrganisationName
                : (string.IsNullOrEmpty(source.TransferEvidenceNoteData.ApprovedRecipientDetails)
                    ? addressUtilities.FormattedCompanyPcsAddress(source.TransferEvidenceNoteData.RecipientSchemeData.SchemeName,
                        source.TransferEvidenceNoteData.RecipientOrganisationData.OrganisationName,
                        recipientOrganisationAddress.Address1,
                        recipientOrganisationAddress.Address2,
                        recipientOrganisationAddress.TownOrCity,
                        recipientOrganisationAddress.CountyOrRegion,
                        recipientOrganisationAddress.Postcode,
                        null)
                    : source.TransferEvidenceNoteData.ApprovedRecipientDetails);

            var model = new ViewTransferNoteViewModel
            {
                RedirectTab = source.RedirectTab,
                ReturnToView = source.ReturnToView ?? false,
                EditMode = source.Edit,
                Reference = source.TransferEvidenceNoteData.Reference,
                Type = source.TransferEvidenceNoteData.Type,
                Status = source.TransferEvidenceNoteData.Status,
                SchemeId = source.OrganisationId,
                EvidenceNoteId = source.TransferEvidenceNoteData.Id,
                SubmittedDate = source.TransferEvidenceNoteData.SubmittedDate.ToDisplayGMTDateTimeString(),
                ApprovedDate = source.TransferEvidenceNoteData.ApprovedDate.ToDisplayGMTDateTimeString(),
                RejectedDate = source.TransferEvidenceNoteData.RejectedDate.ToDisplayGMTDateTimeString(),
                ReturnedDate = source.TransferEvidenceNoteData.ReturnedDate.ToDisplayGMTDateTimeString(),
                VoidedDate = source.TransferEvidenceNoteData.VoidedDate.ToDisplayGMTDateTimeString(),
                RejectedReason = source.TransferEvidenceNoteData.RejectedReason,
                ReturnedReason = source.TransferEvidenceNoteData.ReturnedReason,
                VoidedReason = source.TransferEvidenceNoteData.VoidedReason,
                ComplianceYear = source.TransferEvidenceNoteData.ComplianceYear,
                TotalCategoryValues = source.TransferEvidenceNoteData.TransferEvidenceNoteTonnageData.GroupBy(n => n.EvidenceTonnageData.CategoryId)
                .Select(n =>
                    new TotalCategoryValue(n.First().EvidenceTonnageData.CategoryId)
                    {
                        TotalReceived = n.Sum(e => e.EvidenceTonnageData.TransferredReceived).ToString(),
                        TotalReused = n.Sum(e => e.EvidenceTonnageData.TransferredReused).ToString(),
                    }).OrderBy(n => n.CategoryId).ToList(),
                RecipientAddress = recipientAddress,
                TransferredByAddress = transferredByFormattedAddress,
                Summary = GenerateNotesModel(source),
                DisplayEditButton = (source.TransferEvidenceNoteData.Status == NoteStatus.Draft || source.TransferEvidenceNoteData.Status == NoteStatus.Returned)
                                    && source.TransferEvidenceNoteData.TransferredOrganisationData.Id == source.OrganisationId
                                    && (source.TransferEvidenceNoteData.TransferredOrganisationData.IsBalancingScheme || source.TransferEvidenceNoteData.TransferredSchemeData.SchemeStatus != SchemeStatus.Withdrawn)
                                    && WindowHelper.IsDateInComplianceYear(source.TransferEvidenceNoteData.ComplianceYear, source.SystemDateTime),
                CanVoid = HasClaim(source.User, Claims.InternalAdmin),
                Page = source.Page,
                OpenedInNewTab = source.OpenedInNewTab,
                IsPrintable = source.IsPrintable,
                QueryString = source.QueryString
            };

            SetSuccessMessage(source.TransferEvidenceNoteData, source.DisplayNotification, model);

            return model;
        }

        private bool HasClaim(IPrincipal user, string claim)
        {
            if (user == null)
            {
                return false;
            }
            var claimsPrincipal = new ClaimsPrincipal(user);
            return claimsPrincipal.HasClaim(p => p.Value == claim);
        }

        private void SetSuccessMessage(TransferEvidenceNoteData note, object displayMessageStatus, ViewTransferNoteViewModel model)
        {
            if (displayMessageStatus is NoteUpdatedStatusEnum status)
            {
                switch (status)
                {
                    case NoteUpdatedStatusEnum.Submitted:
                        model.SuccessMessage = $"You have successfully submitted the evidence note transfer with reference ID {DisplayExtensions.ToDisplayString(note.Type)}{note.Reference}";
                        break;
                    case NoteUpdatedStatusEnum.Draft:
                        model.SuccessMessage = $"You have successfully saved the evidence note transfer with reference ID {DisplayExtensions.ToDisplayString(note.Type)}{note.Reference} as a draft";
                        break;
                    case NoteUpdatedStatusEnum.Approved:
                        model.SuccessMessage = $"You have approved the evidence note transfer with reference ID {DisplayExtensions.ToDisplayString(note.Type)}{note.Reference}";
                        break;
                    case NoteUpdatedStatusEnum.Rejected:
                        model.SuccessMessage = $"You have rejected the evidence note transfer with reference ID {DisplayExtensions.ToDisplayString(note.Type)}{note.Reference}";
                        break;
                    case NoteUpdatedStatusEnum.Returned:
                        model.SuccessMessage = $"You have returned the evidence note transfer with reference ID {DisplayExtensions.ToDisplayString(note.Type)}{note.Reference}";
                        break;
                    case NoteUpdatedStatusEnum.ReturnedSaved:
                        model.SuccessMessage = $"You have successfully saved the returned evidence note transfer with reference ID {DisplayExtensions.ToDisplayString(note.Type)}{note.Reference}";
                        break;
                    case NoteUpdatedStatusEnum.ReturnedSubmitted:
                        model.SuccessMessage = $"You have successfully submitted the returned evidence note transfer with reference ID {DisplayExtensions.ToDisplayString(note.Type)}{note.Reference}";
                        break;
                    case NoteUpdatedStatusEnum.Void:
                        model.SuccessMessage = $"You have successfully voided the evidence note transfer with reference ID {DisplayExtensions.ToDisplayString(note.Type)}{note.Reference}";
                        break;
                }
            }
        }

        private IList<ViewTransferEvidenceAatfDataViewModel> GenerateNotesModel(ViewTransferNoteViewModelMapTransfer source)
        {
            return source.TransferEvidenceNoteData.TransferEvidenceNoteTonnageData
                .OrderBy(n => n.OriginalAatf.Name)
                .ThenByDescending(n => n.OriginalReference)
                .GroupBy(n => n.OriginalAatf.Name)
                .Select(n => new ViewTransferEvidenceAatfDataViewModel()
            {
                AatfName = n.First().OriginalAatf.Name,
                AatfApprovalNumber = n.First().OriginalAatf.ApprovalNumber,
                Notes = n.GroupBy(nt => nt.OriginalReference).Select(nt => new ViewTransferEvidenceNoteTonnageDataViewModel()
                {
                    ReferenceId = nt.First().OriginalReference,
                    Type = nt.First().Type,
                    Status = source.TransferEvidenceNoteData.Status,
                    CategoryValues = nt.OrderBy(ntt => ntt.EvidenceTonnageData.CategoryId).Select(ntt => new EvidenceCategoryValue((Core.DataReturns.WeeeCategory)ntt.EvidenceTonnageData.CategoryId)
                    {
                        Received = tonnageUtilities.CheckIfTonnageIsNull(ntt.EvidenceTonnageData.TransferredReceived),
                        Reused = tonnageUtilities.CheckIfTonnageIsNull(ntt.EvidenceTonnageData.TransferredReused)
                    }).ToList()
                }).ToList()
            }).ToList();
        }
    }
}