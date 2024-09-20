namespace EA.Weee.Core.Organisations
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