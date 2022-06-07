namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using Core.AatfEvidence;
    using Core.Helpers;
    using CuttingEdge.Conditions;
    using Prsd.Core.Mapper;
    using ViewModels;

    public class ViewTransferNoteViewModelMap : IMap<ViewTransferNoteViewModelMapTransfer, ViewTransferNoteViewModel>
    {
        public ViewTransferNoteViewModel Map(ViewTransferNoteViewModelMapTransfer source)
        {
            Condition.Requires(source).IsNotNull();

            var model = new ViewTransferNoteViewModel
            {
                Reference = source.TransferEvidenceNoteData.Reference,
                Type = source.TransferEvidenceNoteData.Type,
                Status = source.TransferEvidenceNoteData.Status,
                SchemeId = source.SchemeId
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
    }
}