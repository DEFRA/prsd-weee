namespace EA.Weee.RequestHandlers.Aatf
{
    using Core.Aatf;
    using Core.AatfEvidence;
    using Prsd.Core.Mediator;
    using Requests.AatfReturn;
    using Security;
    using System.Threading.Tasks;

    internal class GetViewEvidenceNoteHandler : IRequestHandler<GetEvidenceNoteRequest, ViewEvidenceNoteViewModel>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetViewEvidenceNote getViewEvidenceNote;

        public GetViewEvidenceNoteHandler(IWeeeAuthorization authorization,
            IGetViewEvidenceNote getViewEvidenceNote)
        {
            this.authorization = authorization;
            this.getViewEvidenceNote = getViewEvidenceNote;
        }

        public async Task<ReturnData> HandleAsync(GetReturn message)
        {
            authorization.EnsureCanAccessExternalArea();

            var returnData = await getViewEvidenceNote.GetReturnData(message.ReturnId, message.ForSummary);

            return returnData;
        }
    }
}