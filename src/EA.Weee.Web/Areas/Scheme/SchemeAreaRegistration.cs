﻿namespace EA.Weee.Web.Areas.Scheme
{
    using Controllers;
    using Infrastructure;
    using System.Web.Mvc;

    public class SchemeAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Scheme";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapLowercaseDashedRoute(
                name: SchemeTransferEvidenceRedirect.ReviewSubmittedTransferEvidenceRouteName,
                url: "Scheme/{pcsId}/transfer-evidence/outgoing-transfers/submitted-transfer/{evidenceNoteId}",
                defaults: new { action = "SubmittedTransfer", controller = "OutgoingTransfers", area = "Scheme" },
                namespaces: new[] { typeof(OutgoingTransfersController).Namespace });

            context.MapLowercaseDashedRoute(
                name: SchemeTransferEvidenceRedirect.ViewSubmittedTransferEvidenceRouteName,
                url: "Scheme/{pcsId}/transfer-evidence/outgoing-transfers/view-submitted-transfer/{evidenceNoteId}",
                defaults: new { action = "TransferredEvidence", controller = "TransferEvidence", area = "Scheme" },
                namespaces: new[] { typeof(ManageEvidenceNotesController).Namespace });

            context.MapLowercaseDashedRoute(
                name: SchemeTransferEvidenceRedirect.ViewApprovedTransferEvidenceRouteName,
                url: "Scheme/{pcsId}/transfer-evidence/outgoing-transfers/view-approved-transfer/{evidenceNoteId}",
                defaults: new { action = "TransferredEvidence", controller = "TransferEvidence", area = "Scheme" },
                namespaces: new[] { typeof(ManageEvidenceNotesController).Namespace });

            context.MapLowercaseDashedRoute(
                name: SchemeTransferEvidenceRedirect.ViewReturnedTransferEvidenceRouteName,
                url: "Scheme/{pcsId}/transfer-evidence/outgoing-transfers/view-returned-transfer/{evidenceNoteId}",
                defaults: new { action = "TransferredEvidence", controller = "TransferEvidence", area = "Scheme" },
                namespaces: new[] { typeof(ManageEvidenceNotesController).Namespace });

            context.MapLowercaseDashedRoute(
                name: SchemeTransferEvidenceRedirect.ViewRejectedTransferEvidenceRouteName,
                url: "Scheme/{pcsId}/transfer-evidence/outgoing-transfers/view-rejected-transfer/{evidenceNoteId}",
                defaults: new { action = "TransferredEvidence", controller = "TransferEvidence", area = "Scheme" },
                namespaces: new[] { typeof(ManageEvidenceNotesController).Namespace });

            context.MapLowercaseDashedRoute(
                name: SchemeTransferEvidenceRedirect.ViewVoidedTransferEvidenceRouteName,
                url: "Scheme/{pcsId}/transfer-evidence/outgoing-transfers/view-voided-transfer/{evidenceNoteId}",
                defaults: new { action = "TransferredEvidence", controller = "TransferEvidence", area = "Scheme" },
                namespaces: new[] { typeof(ManageEvidenceNotesController).Namespace });

            context.MapLowercaseDashedRoute(
                name: SchemeTransferEvidenceRedirect.ViewDraftTransferEvidenceRouteName,
                url: "Scheme/{pcsId}/transfer-evidence/outgoing-transfers/draft-transfer/{evidenceNoteId}",
                defaults: new { action = "TransferredEvidence", controller = "TransferEvidence", area = "Scheme" },
                namespaces: new[] { typeof(ManageEvidenceNotesController).Namespace });

            context.MapLowercaseDashedRoute(
                name: SchemeTransferEvidenceRedirect.ViewCancelledTransferEvidenceNoteRouteName,
                url: "Scheme/{pcsId}/transfer-evidence/outgoing-transfers/view-cancelled-transfer/{evidenceNoteId}",
                defaults: new { action = "TransferredEvidence", controller = "TransferEvidence", area = "Scheme" },
                namespaces: new[] { typeof(ManageEvidenceNotesController).Namespace });

            context.MapLowercaseDashedRoute(
                name: "Scheme_edit_transfer_categories",
                url: "Scheme/{pcsId}/transfer-evidence/outgoing-transfers/edit-categories/{evidenceNoteId}",
                defaults: new { action = "EditCategories", controller = "OutgoingTransfers", area = "Scheme" },
                namespaces: new[] { typeof(OutgoingTransfersController).Namespace });

            context.MapLowercaseDashedRoute(
                name: "Scheme_edit_transfer_tonnages",
                url: "Scheme/{pcsId}/transfer-evidence/outgoing-transfers/edit-tonnages/{evidenceNoteId}",
                defaults: new { action = "EditTonnages", controller = "OutgoingTransfers", area = "Scheme" },
                namespaces: new[] { typeof(OutgoingTransfersController).Namespace });

            context.MapLowercaseDashedRoute(
                name: "Scheme_edit_transfer_notes",
                url: "Scheme/{pcsId}/transfer-evidence/outgoing-transfers/edit-transfer-from/{evidenceNoteId}",
                defaults: new { action = "EditTransferFrom", controller = "OutgoingTransfers", area = "Scheme" },
                namespaces: new[] { typeof(OutgoingTransfersController).Namespace });

            context.MapLowercaseDashedRoute(
                name: SchemeTransferEvidenceRedirect.EditDraftTransferEvidenceRouteName,
                url: "Scheme/{pcsId}/transfer-evidence/outgoing-transfers/edit-draft-transfer/{evidenceNoteId}",
                defaults: new { action = "EditDraftTransfer", controller = "OutgoingTransfers", area = "Scheme" },
                namespaces: new[] { typeof(OutgoingTransfersController).Namespace });

            context.MapLowercaseDashedRoute(
                name: SchemeTransferEvidenceRedirect.ViewApprovedEvidenceNoteRouteName,
                url: "Scheme/{pcsId}/manage-evidence-notes/view-approved-evidence-note/{evidenceNoteId}",
                defaults: new { action = "ViewEvidenceNote", controller = "ManageEvidenceNotes", area = "Scheme" },
                namespaces: new[] { typeof(ManageEvidenceNotesController).Namespace });

            context.MapLowercaseDashedRoute(
                name: SchemeTransferEvidenceRedirect.ViewVoidedEvidenceNoteRouteName,
                url: "Scheme/{pcsId}/manage-evidence-notes/view-voided-evidence-note/{evidenceNoteId}",
                defaults: new { action = "ViewEvidenceNote", controller = "ManageEvidenceNotes", area = "Scheme" },
                namespaces: new[] { typeof(ManageEvidenceNotesController).Namespace });

            context.MapLowercaseDashedRoute(
                name: SchemeTransferEvidenceRedirect.ViewRejectedEvidenceNoteRouteName,
                url: "Scheme/{pcsId}/manage-evidence-notes/view-rejected-evidence-note/{evidenceNoteId}",
                defaults: new { action = "ViewEvidenceNote", controller = "ManageEvidenceNotes", area = "Scheme" },
                namespaces: new[] { typeof(ManageEvidenceNotesController).Namespace });

            context.MapLowercaseDashedRoute(
                name: SchemeTransferEvidenceRedirect.ViewReturnedEvidenceNoteRouteName,
                url: "Scheme/{pcsId}/manage-evidence-notes/view-returned-evidence-note/{evidenceNoteId}",
                defaults: new { action = "ViewEvidenceNote", controller = "ManageEvidenceNotes", area = "Scheme" },
                namespaces: new[] { typeof(ManageEvidenceNotesController).Namespace });

            context.MapLowercaseDashedRoute(
                name: SchemeTransferEvidenceRedirect.ViewSubmittedEvidenceNoteRouteName,
                url: "Scheme/{pcsId}/manage-evidence-notes/view-submitted-evidence-note/{evidenceNoteId}",
                defaults: new { action = "ViewEvidenceNote", controller = "ManageEvidenceNotes", area = "Scheme" },
                namespaces: new[] { typeof(ManageEvidenceNotesController).Namespace });

            context.MapLowercaseDashedRoute(
                name: "Scheme_evidence_default",
                url: "Scheme/{pcsId}/{controller}/{action}/{evidenceNoteId}",
                defaults: new { action = "Index", controller = "Home", area = "Scheme" },
                namespaces: new[] { typeof(ManageEvidenceNotesController).Namespace });

            context.MapLowercaseDashedRoute(
                name: "Scheme_default",
                url: "Scheme/{pcsId}/{controller}/{action}/{entityId}",
                defaults: new { action = "Index", controller = "Home", entityId = UrlParameter.Optional, area = "Scheme" },
                namespaces: new[] { typeof(HomeController).Namespace });

            context.MapLowercaseDashedRoute(
                name: "DownloadTransferEvidenceNote",
                url: "Scheme/{organisationId}/{controller}/{pcsId}/{action}/{transferEvidenceNoteId}",
                defaults: new { action = "DownloadTransferEvidenceNote", controller = "TransferEvidence" },
                namespaces: new[] { typeof(TransferEvidenceController).Namespace });
        }
    }
}