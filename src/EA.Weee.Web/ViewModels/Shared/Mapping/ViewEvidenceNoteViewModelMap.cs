namespace EA.Weee.Web.ViewModels.Shared.Mapping
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Security.Principal;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Security;
    using EA.Weee.Web.Areas.Scheme.ViewModels;
    using EA.Weee.Web.Extensions;
    using EA.Weee.Web.ViewModels.Returns.Mappings.ToViewModel;
    using EA.Weee.Web.ViewModels.Shared.Utilities;

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
                RecipientAddress = source.EvidenceNoteData.RecipientOrganisationData.IsBalancingScheme ? source.EvidenceNoteData.RecipientOrganisationData.OrganisationName :
                        addressUtilities.FormattedCompanyPcsAddress(source.EvidenceNoteData.RecipientSchemeData.SchemeName,
                    source.EvidenceNoteData.RecipientOrganisationData.OrganisationName,
                    organisationAddress.Address1,
                    organisationAddress.Address2,
                    organisationAddress.TownOrCity,
                    organisationAddress.CountyOrRegion,
                    organisationAddress.Postcode,
                    null),
                SchemeId = source.SchemeId, 
                AatfApprovalNumber = source.EvidenceNoteData.AatfData.ApprovalNumber,
                DisplayEditButton = (source.EvidenceNoteData.Status == NoteStatus.Draft || source.EvidenceNoteData.Status == NoteStatus.Returned) && source.EvidenceNoteData.AatfData.CanCreateEditEvidence,
                RedirectTab = source.RedirectTab,
                EvidenceNoteHistoryData = mapper.Map<IList<EvidenceNoteHistoryViewModel>>(source.EvidenceNoteData.EvidenceNoteHistoryData),
                CanVoid = !source.PrintableVersion &&
                          InternalAdmin(source.User) && 
                          source.EvidenceNoteData.Status == NoteStatus.Approved && 
                          source.EvidenceNoteData.EvidenceNoteHistoryData.All(e => allowVoidStatus.Contains(e.Status)),
                CanDisplayNotesMessage = source.EvidenceNoteData.EvidenceNoteHistoryData.Any(e => !allowVoidStatus.Contains(e.Status)),
                IsPrintable = source.PrintableVersion
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

                    if ((transferTonnage == null || transferTonnage.Count == 0 || originalTonnage != null) && !source.IncludeAllCategories)
                    {
                        model.RemainingTransferCategoryValues.RemoveAt(i);
                    }
                    else if (transferTonnage != null && originalTonnage != null)
                    {
                        var transferReceived = originalTonnage.Received - transferTonnage.Sum(x => x.Received);
                        var transferReused = originalTonnage.Reused - transferTonnage.Sum(x => x.Reused);
                        category.Received = tonnageUtilities.CheckIfTonnageIsNull(transferReceived == 0 ? null : transferReceived);
                        category.Reused = tonnageUtilities.CheckIfTonnageIsNull(transferReused == 0 ? null : transferReused);
                    }
                    else
                    {
                        category.Received = tonnageUtilities.CheckIfTonnageIsNull(null);
                        category.Reused = tonnageUtilities.CheckIfTonnageIsNull(null);
                    }
                }
            }

            model.TotalReceivedDisplay = model.ReceivedTotal;

            SetSuccessMessage(source.EvidenceNoteData, source.NoteStatus, model);

            return model;
        }

        private bool InternalAdmin(IPrincipal user)
        {
            if (user == null)
            {
                return false;
            }
            var claimsPrincipal = new ClaimsPrincipal(user);
            return claimsPrincipal.HasClaim(p => p.Value == Claims.InternalAdmin);
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