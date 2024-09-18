namespace EA.Weee.Web.Areas.Producer.ViewModels
{
    using System;

    public interface IProducerSubmissionViewModel
    {
        Guid DirectRegistrantId { get; set; }
        Guid OrganisationId { get; set; }

        bool HasAuthorisedRepresentitive { get; set; }

        bool? RedirectToCheckAnswers { get; set; }
    }
}