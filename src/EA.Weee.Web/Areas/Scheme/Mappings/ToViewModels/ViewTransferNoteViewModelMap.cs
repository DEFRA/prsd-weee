namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using Core.AatfEvidence;
    using Core.Helpers;
    using CuttingEdge.Conditions;
    using EA.Weee.Web.ViewModels.Returns.Mappings.ToViewModel;
    using EA.Weee.Web.ViewModels.Shared.Utilities;
    using Prsd.Core.Mapper;
    using System.Collections.Generic;
    using System.Linq;
    using ViewModels;

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

            var model = new ViewTransferNoteViewModel
            {
                SelectedComplianceYear = source.SelectedComplianceYear,
                Reference = source.TransferEvidenceNoteData.Reference,
                Type = source.TransferEvidenceNoteData.Type,
                Status = source.TransferEvidenceNoteData.Status,
                SchemeId = source.SchemeId,
                ComplianceYear = source.TransferEvidenceNoteData.ComplianceYear,
                TotalCategoryValues = source.TransferEvidenceNoteData.TransferEvidenceNoteTonnageData.GroupBy(n => n.EvidenceTonnageData.CategoryId)
                .Select(n =>
                    new TotalCategoryValue(n.First().EvidenceTonnageData.CategoryId)
                    {
                        TotalReceived = n.Sum(e => e.EvidenceTonnageData.TransferredReceived).ToString(),
                        TotalReused = n.Sum(e => e.EvidenceTonnageData.TransferredReused).ToString(),
                    }).OrderBy(n => n.CategoryId).ToList(),
                RecipientAddress = addressUtilities.FormattedCompanyPcsAddress(source.TransferEvidenceNoteData.RecipientSchemeData.SchemeName,
                    source.TransferEvidenceNoteData.RecipientOrganisationData.OrganisationName,
                    recipientOrganisationAddress.Address1,
                    recipientOrganisationAddress.Address2,
                    recipientOrganisationAddress.TownOrCity,
                    recipientOrganisationAddress.CountyOrRegion,
                    recipientOrganisationAddress.Postcode,
                    null),
                TransferredByAddress = addressUtilities.FormattedCompanyPcsAddress(source.TransferEvidenceNoteData.TransferredSchemeData.SchemeName,
                    source.TransferEvidenceNoteData.TransferredOrganisationData.Name,
                    transferOrganisationAddress.Address1,
                    transferOrganisationAddress.Address2,
                    transferOrganisationAddress.TownOrCity,
                    transferOrganisationAddress.CountyOrRegion,
                    transferOrganisationAddress.Postcode,
                    null),
                Summary = GenerateNotesModel(source)
            };

            SetSuccessMessage(source.TransferEvidenceNoteData, source.DisplayNotification, model);

            return model;
        }

        private void SetSuccessMessage(TransferEvidenceNoteData note, object displayMessage, ViewTransferNoteViewModel model)
        {
            if (displayMessage is bool display)
            {
                if (display)
                {
                    switch (note.Status)
                    {
                        case NoteStatus.Submitted:
                            model.SuccessMessage =
                            $"You have successfully submitted the evidence note transfer with reference ID {note.Type.ToDisplayString()}{note.Reference}";
                            break;
                        case NoteStatus.Draft:
                            model.SuccessMessage =
                                $"You have successfully saved the evidence note transfer with reference ID {note.Type.ToDisplayString()}{note.Reference} as a draft";
                            break;
                    }
                }
            }
        }

        private IList<ViewTransferEvidenceAatfDataViewModel> GenerateNotesModel(ViewTransferNoteViewModelMapTransfer source)
        {
            return source.TransferEvidenceNoteData.TransferEvidenceNoteTonnageData.OrderBy(n => n.OriginalAatf.Name).ThenBy(n => n.Reference).GroupBy(n => n.OriginalAatf.Name).Select(n => new ViewTransferEvidenceAatfDataViewModel()
            {
                AatfName = n.First().OriginalAatf.Name,
                AatfApprovalNumber = n.First().OriginalAatf.ApprovalNumber,
                Notes = n.GroupBy(nt => nt.Reference).Select(nt => new ViewTransferEvidenceNoteTonnageDataViewModel()
                {
                    ReferenceId = nt.First().Reference,
                    Type = nt.First().Type,
                    CategoryValues = nt.Select(ntt => new EvidenceCategoryValue((Core.DataReturns.WeeeCategory)ntt.EvidenceTonnageData.CategoryId)
                    {
                        Received = tonnageUtilities.CheckIfTonnageIsNull(ntt.EvidenceTonnageData.TransferredReceived),
                        Reused = tonnageUtilities.CheckIfTonnageIsNull(ntt.EvidenceTonnageData.TransferredReused)
                    }).ToList()
                }).ToList()
            }).ToList();
        }
    }
}