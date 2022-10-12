namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using System.Web;

    public interface ITransferEvidenceNotesViewModelValidator
    {
        void Validate(TransferEvidenceNotesViewModel model, HttpSessionStateBase sessionWrapper);
    }
}