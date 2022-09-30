namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using DataAccess;
    using Prsd.Core.Domain;
    using Prsd.Core.Mediator;
    using Security;
    using System;
    using System.Threading.Tasks;
    using Core.AatfEvidence;
    using Core.Shared;
    using DataAccess.DataAccess;
    using Requests.AatfEvidence;

    public class SetNoteStatusRequestHandler : SaveNoteRequestBase, IRequestHandler<SetNoteStatusRequest, Guid>
    {
        private readonly IAddressUtilities addressUtilities;

        public SetNoteStatusRequestHandler(
            WeeeContext context,
            IUserContext userContext,
            IWeeeAuthorization authorization, 
            ISystemDataDataAccess systemDataDataAccess,
            IAddressUtilities addressUtilities) : base(context, userContext, authorization, systemDataDataAccess)
        {
            this.addressUtilities = addressUtilities;
        }

        public async Task<Guid> HandleAsync(SetNoteStatusRequest message)
        {
            Authorization.EnsureCanAccessExternalArea();

            var evidenceNote = await EvidenceNote(message.NoteId);

            Authorization.EnsureOrganisationAccess(message.Status == NoteStatus.Submitted
                ? evidenceNote.Organisation.Id
                : evidenceNote.Recipient.Id);

            var currentDate = await SystemDataDataAccess.GetSystemDateTime();

            ValidToSave(evidenceNote.Recipient, evidenceNote.ComplianceYear, currentDate);

            if (message.Status == NoteStatus.Approved && evidenceNote.Recipient.IsBalancingScheme == false)
            {
                var organisationAddress = evidenceNote.Recipient.HasBusinessAddress
                    ? evidenceNote.Recipient.BusinessAddress
                    : evidenceNote.Recipient.NotificationAddress;

                evidenceNote.SetApprovedRecipientDetails(evidenceNote.Recipient.Scheme.SchemeName, addressUtilities.FormattedCompanyPcsAddress(evidenceNote.Recipient.Scheme.SchemeName,
                    evidenceNote.Recipient.OrganisationName,
                    organisationAddress.Address1,
                    organisationAddress.Address2,
                    organisationAddress.TownOrCity,
                    organisationAddress.CountyOrRegion,
                    organisationAddress.Postcode,
                    null), evidenceNote.Recipient.Scheme.ApprovalNumber);
            }

            return await UpdateNoteStatus(evidenceNote, message.Status, currentDate, message.Reason);
        }
    }
}
