namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web.Mvc;
    using Api.Client;
    using Constant;
    using Services;
    using Web.ViewModels.Shared;
    using Weee.Requests.Scheme;

    [Serializable]
    public class TransferEvidenceNotesViewModel : TransferEvidenceViewModelBase, IValidatableObject
    {
        [Display(Name = "Search by reference ID")]
        public string SearchRef { get; set; }

        [Display(Name = "Submitted by")]
        public Guid? SubmittedBy { get; set; }

        public IEnumerable<SelectListItem> SubmittedByList { get; set; }

        public bool SearchPerformed => !string.IsNullOrWhiteSpace(SearchRef) || SubmittedBy.HasValue;

        public ActionEnum Action { get; set; }

        public int PageNumber { get; set; }

        public bool IsEdit { get; set; }

        /// <summary>
        /// Given time we shouldn't do the validation like this. A FluentValidation validator should be created that
        /// the service can be injected into.
        /// </summary>
        private ISessionService sessionService;
        public ISessionService SessionService
        {
            get => sessionService ?? DependencyResolver.Current.GetService<ISessionService>();
            set => sessionService = value;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var requestProperty = IsEdit
                ? SessionKeyConstant.OutgoingTransferKey
                : SessionKeyConstant.TransferNoteKey;

            var request = SessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(requestProperty);

            if (!request.EvidenceNoteIds.Any())
            {
                yield return new ValidationResult("Select at least one evidence note to transfer from", new[] { ValidationKeyConstants.TransferEvidenceNotesSelectedNotesError });
            }
            else
            {
                if (request.EvidenceNoteIds.Count > 5)
                {
                    yield return new ValidationResult("You cannot select more than 5 notes",
                        new[] { ValidationKeyConstants.TransferEvidenceNotesSelectedNotesError });
                }
            }
        }
    }
}