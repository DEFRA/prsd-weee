﻿namespace EA.Weee.Web.ViewModels.Shared.Mapping
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Security.Principal;
    using Core.Shared;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Security;
    using EA.Weee.Web.Extensions;
    using EA.Weee.Web.ViewModels.Returns.Mappings.ToViewModel;

    public class ViewEvidenceNoteViewModelMap : IMap<ViewEvidenceNoteMapTransfer, ViewEvidenceNoteViewModel>
    {
        private readonly ITonnageUtilities tonnageUtilities;
        private readonly IAddressUtilities addressUtilities;
        private readonly IMapper mapper;

        public ViewEvidenceNoteViewModelMap(ITonnageUtilities tonnageUtilities,
            IAddressUtilities addressUtilities,
            IMapper mapper)
        {
            this.tonnageUtilities = tonnageUtilities;
            this.addressUtilities = addressUtilities;
            this.mapper = mapper;
        }

        public ViewEvidenceNoteViewModel Map(ViewEvidenceNoteMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var organisationAddress = source.EvidenceNoteData.RecipientOrganisationData.HasBusinessAddress
                ? source.EvidenceNoteData.RecipientOrganisationData.BusinessAddress
                : source.EvidenceNoteData.RecipientOrganisationData.NotificationAddress;

            var allowVoidStatus = new List<NoteStatus>() { NoteStatus.Void, NoteStatus.Rejected };

            var recipientAddress = source.EvidenceNoteData.RecipientOrganisationData.IsBalancingScheme
                ? source.EvidenceNoteData.RecipientOrganisationData.OrganisationName
                : (string.IsNullOrEmpty(source.EvidenceNoteData.ApprovedRecipientDetails)
                    ? addressUtilities.FormattedCompanyPcsAddress(source.EvidenceNoteData.RecipientSchemeData.SchemeName,
                        source.EvidenceNoteData.RecipientOrganisationData.OrganisationName,
                        organisationAddress.Address1,
                        organisationAddress.Address2,
                        organisationAddress.TownOrCity,
                        organisationAddress.CountyOrRegion,
                        organisationAddress.Postcode,
                        null)
                    : source.EvidenceNoteData.ApprovedRecipientDetails);

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
                RejectedDate = source.EvidenceNoteData.RejectedDate.ToDisplayGMTDateTimeString(),
                VoidedDate = source.EvidenceNoteData.VoidedDate.ToDisplayGMTDateTimeString(),
                RejectedReason = source.EvidenceNoteData.RejectedReason,
                ReturnedReason = source.EvidenceNoteData.ReturnedReason,
                VoidedReason = source.EvidenceNoteData.VoidedReason,
                CancelledReason = source.EvidenceNoteData.CancelledReason,
                ProtocolValue = source.EvidenceNoteData.Protocol,
                WasteTypeValue = source.EvidenceNoteData.WasteType,
                SubmittedBy = source.EvidenceNoteData.SubmittedDate.HasValue ? source.EvidenceNoteData.AatfData.Name : string.Empty,
                ComplianceYear = source.EvidenceNoteData.ComplianceYear,
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
                RecipientAddress = recipientAddress,
                SchemeId = source.SchemeId,
                AatfApprovalNumber = source.EvidenceNoteData.AatfData.ApprovalNumber,
                DisplayEditButton = (source.EvidenceNoteData.Status == NoteStatus.Draft) && source.EvidenceNoteData.AatfData.CanCreateEditEvidence,
                RedirectTab = source.RedirectTab,
                EvidenceNoteHistoryData = mapper.Map<IList<EvidenceNoteRowViewModel>>(source.EvidenceNoteData.EvidenceNoteHistoryData),
                CanVoid = !source.PrintableVersion &&
                          HasClaim(source.User, Claims.InternalAdmin) &&
                          source.EvidenceNoteData.Status == NoteStatus.Approved &&
                          source.EvidenceNoteData.EvidenceNoteHistoryData.All(e => allowVoidStatus.Contains(e.Status)),
                CanDisplayNotesMessage = source.EvidenceNoteData.EvidenceNoteHistoryData.Any(e => !allowVoidStatus.Contains(e.Status)),
                IsPrintable = source.PrintableVersion,
                IsInternalUser = HasClaim(source.User, Claims.CanAccessInternalArea),
                TotalAvailable = source.EvidenceNoteData.TotalReceivedAvailable.ToTonnageDisplay(),
                OpenedInNewTab = source.OpenedInNewTab,
                QueryString = source.QueryString,
                ReturnToView = source.ReturnToView,
                DisplayCancelButton = (source.EvidenceNoteData.Status == NoteStatus.Returned)
            };

            for (var i = model.CategoryValues.Count - 1; i >= 0; i--)
            {
                var category = model.CategoryValues.ElementAt(i);

                var noteTonnage = source.EvidenceNoteData.EvidenceTonnageData.FirstOrDefault(t =>
                    t.CategoryId.ToInt().Equals(category.CategoryId.ToInt()));

                if (noteTonnage == null && !source.IncludeAllCategories)
                {
                    model.CategoryValues.RemoveAt(i);
                }
                else if (noteTonnage != null)
                {
                    category.Received = tonnageUtilities.CheckIfTonnageIsNull(noteTonnage.Received);
                    category.Reused = tonnageUtilities.CheckIfTonnageIsNull(noteTonnage.Reused);
                    category.Id = noteTonnage.Id;
                }
            }

            if (TransferHistoryHasApprovedTransferNotes(source.EvidenceNoteData.EvidenceNoteHistoryData))
            {
                model.DisplayTransferEvidenceColumns = true;

                for (var i = model.RemainingTransferCategoryValues.Count - 1; i >= 0; i--)
                {
                    var category = model.RemainingTransferCategoryValues.ElementAt(i);

                    var transferTonnage = source.EvidenceNoteData.EvidenceNoteHistoryData.SelectMany(x => x.TransferEvidenceTonnageData).Where(y => y.CategoryId == (WeeeCategory)category.CategoryId).Distinct().ToList();
                    var originalTonnage = source.EvidenceNoteData.EvidenceTonnageData.FirstOrDefault(t =>
                        t.CategoryId.ToInt().Equals(category.CategoryId.ToInt()));

                    if ((transferTonnage == null || transferTonnage.Count == 0) && originalTonnage == null)
                    {
                        // If original category was deleted, number should be zeroed
                        category.Received = tonnageUtilities.CheckIfTonnageIsNull(null);
                        category.Reused = tonnageUtilities.CheckIfTonnageIsNull(null);
                    }
                    else if (originalTonnage != null)
                    {
                        var transferReceived = originalTonnage.Received - transferTonnage.Sum(x => x.Received);
                        var transferReused = originalTonnage.Reused - transferTonnage.Sum(x => x.Reused);

                        if (transferReceived == 0)
                        {
                            category.Received = "0.000";
                        }
                        else
                        {
                            category.Received = tonnageUtilities.CheckIfTonnageIsNull(transferReceived == 0 ? null : transferReceived);
                        }
                        if (transferReused == 0)
                        {
                            category.Reused = "0.000";
                        }
                        else
                        {
                            category.Reused = tonnageUtilities.CheckIfTonnageIsNull(transferReused == 0 ? null : transferReused);
                        }
                    }
                    else
                    {
                        category.Received = tonnageUtilities.CheckIfTonnageIsNull(null);
                        category.Reused = tonnageUtilities.CheckIfTonnageIsNull(null);
                    }
                }
            }

            SetSuccessMessage(source.EvidenceNoteData, source.NoteStatus, model);

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

        private void SetSuccessMessage(EvidenceNoteData note, object noteStatus, ViewEvidenceNoteViewModel model)
        {
            if (noteStatus != null)
            {
                if (noteStatus is NoteUpdatedStatusEnum status)
                {
                    switch (status)
                    {
                        case NoteUpdatedStatusEnum.Submitted:
                            model.SuccessMessage = $"You have successfully submitted the evidence note with reference ID E{note.Reference}";
                            break;
                        case NoteUpdatedStatusEnum.Draft:
                            model.SuccessMessage = $"You have successfully saved the evidence note with reference ID E{note.Reference} as a draft";
                            break;
                        case NoteUpdatedStatusEnum.Approved:
                            model.SuccessMessage = $"You have approved the evidence note with reference ID E{note.Reference}";
                            break;
                        case NoteUpdatedStatusEnum.Rejected:
                            model.SuccessMessage = $"You have rejected the evidence note with reference ID E{note.Reference}";
                            break;
                        case NoteUpdatedStatusEnum.Returned:
                            model.SuccessMessage = $"You have returned the evidence note with reference ID E{note.Reference}";
                            break;
                        case NoteUpdatedStatusEnum.ReturnedSaved:
                            model.SuccessMessage = $"You have successfully saved the returned evidence note with reference ID E{note.Reference}";
                            break;
                        case NoteUpdatedStatusEnum.ReturnedSubmitted:
                            model.SuccessMessage = $"You have successfully submitted the returned evidence note with reference ID E{note.Reference}";
                            break;
                        case NoteUpdatedStatusEnum.Void:
                            model.SuccessMessage = $"You have successfully voided the evidence note with reference ID E{note.Reference}";
                            break;
                        case NoteUpdatedStatusEnum.Cancelled:
                            model.SuccessMessage = $"You have successfully cancelled the evidence note with reference ID E{note.Reference}";
                            break;
                    }
                }
            }
        }

        private bool TransferHistoryHasApprovedTransferNotes(List<EvidenceNoteHistoryData> historyData)
        {
            return (historyData != null && historyData.Count >= 1 && historyData.Where(x => x.Status.Equals(NoteStatus.Approved)).Any(y => y.TransferEvidenceTonnageData != null && y.TransferEvidenceTonnageData.Count >= 1)) ? true : false;
        }
    }
}