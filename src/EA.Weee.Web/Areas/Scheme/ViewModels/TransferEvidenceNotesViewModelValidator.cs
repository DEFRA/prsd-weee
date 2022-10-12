namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using System.Web;
    using System.Web.Providers.Entities;
    using Constant;
    using FluentValidation;
    using FluentValidation.Results;
    using Services;
    using Weee.Requests.Scheme;

    public class TransferEvidenceNotesViewModelValidator : ITransferEvidenceNotesViewModelValidator
    {
        private readonly ISessionService sessionService;

        public TransferEvidenceNotesViewModelValidator(ISessionService sessionService)
        {
            this.sessionService = sessionService;
        }

        public void Validate(TransferEvidenceNotesViewModel model, HttpSessionStateBase sessionWrapper)
        {
            var requestProperty = model.IsEdit
                ? SessionKeyConstant.OutgoingTransferKey
                : SessionKeyConstant.TransferNoteKey;

            var request = sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(requestProperty);
        }
    }
}