namespace EA.Weee.Email.EventHandlers.SchemeDataReturnSubmission
{
    using System.Threading.Tasks;
    using Domain.Events;
    using Prsd.Core.Domain;

    public class SchemeDataReturnSubmissionEventHandler : IEventHandler<SchemeDataReturnSubmissionEvent>
    {
        private readonly IWeeeEmailService weeeEmailService;
        private readonly ISchemeDataReturnSubmissionEventHandlerDataAccess dataAccess;

        public SchemeDataReturnSubmissionEventHandler(
            IWeeeEmailService weeeEmailService,
            ISchemeDataReturnSubmissionEventHandlerDataAccess dataAccess)
        {
            this.weeeEmailService = weeeEmailService;
            this.dataAccess = dataAccess;
        }

        public async Task HandleAsync(SchemeDataReturnSubmissionEvent @event)
        {
            var dataReturnVersion = @event.DataReturnVersion;

            // The count will include the return version being submitted. A value
            // of one should therefore not be considered as a resubmission.
            int submissionCount = await dataAccess.GetNumberOfDataReturnSubmissionsAsync(
                dataReturnVersion.DataReturn.Scheme,
                dataReturnVersion.DataReturn.Quarter.Year,
                dataReturnVersion.DataReturn.Quarter.Q);

            await weeeEmailService.SendSchemeDataReturnSubmitted(
                   dataReturnVersion.DataReturn.Scheme.CompetentAuthority.Email,
                   dataReturnVersion.DataReturn.Scheme.SchemeName,
                   dataReturnVersion.DataReturn.Quarter.Year,
                   (int)dataReturnVersion.DataReturn.Quarter.Q,
                   submissionCount > 1);
        }
    }
}
