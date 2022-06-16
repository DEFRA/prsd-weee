namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using Core.AatfEvidence;
    using Core.Helpers;
    using CuttingEdge.Conditions;
    using EA.Weee.Web.Services.Caching;
    using EA.Weee.Web.ViewModels.Shared.Utilities;
    using Prsd.Core.Mapper;
    using System.Collections.Generic;
    using System.Linq;
    using ViewModels;

    public class ViewTransferNoteViewModelMap : IMap<ViewTransferNoteViewModelMapTransfer, ViewTransferNoteViewModel>
    {
        private readonly IAddressUtilities addressUtilities;

        public ViewTransferNoteViewModelMap(IAddressUtilities addressUtilities)
        {
            this.addressUtilities = addressUtilities;
        }

        public ViewTransferNoteViewModel Map(ViewTransferNoteViewModelMapTransfer source)
        {
            Condition.Requires(source).IsNotNull();

            var organisationAddress = source.TransferEvidenceNoteData.RecipientOrganisationData.HasBusinessAddress
                ? source.TransferEvidenceNoteData.RecipientOrganisationData.BusinessAddress
                : source.TransferEvidenceNoteData.RecipientOrganisationData.NotificationAddress;

            //var transfer = source.TransferEvidenceNoteData.TransferredOrganisation.HasBusinessAddress
            //    ? source.TransferEvidenceNoteData.TransferredOrganisation.BusinessAddress
            //    : source.TransferEvidenceNoteData.TransferredOrganisation.NotificationAddress;

            var model = new ViewTransferNoteViewModel
            {
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
                    }).ToList(),
                RecipientAddress = addressUtilities.FormattedCompanyPcsAddress(source.TransferEvidenceNoteData.RecipientSchemeData.SchemeName,
                    source.TransferEvidenceNoteData.RecipientOrganisationData.OrganisationName,
                    organisationAddress.Address1,
                    organisationAddress.Address2,
                    organisationAddress.TownOrCity,
                    organisationAddress.CountyOrRegion,
                    organisationAddress.Postcode,
                    null),
                EvidenceNotes = GenerateNotesModel(source),
                //TransferredByAddress = addressUtilities.FormattedCompanyPcsAddress(source.TransferEvidenceNoteData.TransferredOrganisation.Name,
                //    source.TransferEvidenceNoteData.TransferredOrganisation.Name,
                //    transfer.Address1,
                //    transfer.Address2,
                //    transfer.TownOrCity,
                //    transfer.CountyOrRegion,
                //    transfer.Postcode,
                //    null),
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

        private IList<ViewTransferEvidenceNoteTonnageDataViewModel> GenerateNotesModel(ViewTransferNoteViewModelMapTransfer source)
        {
            var sortedSource = source.TransferEvidenceNoteData.TransferEvidenceNoteTonnageData.OrderBy(n => n.OriginalAatf.Name).ThenBy(n => n.EvidenceTonnageData.CategoryId).ToList(); // TO DO change this to Order by Reference + AATF + Category ID
            var modelList = new List<ViewTransferEvidenceNoteTonnageDataViewModel>();

            for (var i = 0; i < sortedSource.Count; i++)
            {
                modelList.Add(new ViewTransferEvidenceNoteTonnageDataViewModel()
                {
                    AatfApprovalNumber = sortedSource[i].OriginalAatf.ApprovalNumber,
                    AatfName = sortedSource[i].OriginalAatf.Name,
                    ReferenceId = sortedSource[i].Reference,
                    Type = sortedSource[i].Type,
                    CategoryValue = new EvidenceCategoryValue()
                    {
                        CategoryId = ((int)sortedSource[i].EvidenceTonnageData.CategoryId),
                        Received = sortedSource[i].EvidenceTonnageData.Received.ToString(),
                        Reused = sortedSource[i].EvidenceTonnageData.Reused.ToString()
                    },
                    DisplayAatfName = (i == 0 || sortedSource[i].OriginalAatf.Name != sortedSource[i - 1].OriginalAatf.Name),
                    DisplayReferenceCell = (i == 0 || sortedSource[i].Reference != sortedSource[i - 1].Reference),
                });
            }

            return modelList;
        }
    }
}