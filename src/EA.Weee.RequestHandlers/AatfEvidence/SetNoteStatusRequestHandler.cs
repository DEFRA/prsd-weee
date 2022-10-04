﻿namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using Core.AatfEvidence;
    using Core.Shared;
    using DataAccess;
    using EA.Weee.Core.Helpers;
    using Prsd.Core.Domain;
    using Prsd.Core.Mediator;
    using Security;
    using System;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;
    using Requests.AatfEvidence;

    public class SetNoteStatusRequestHandler : SaveNoteRequestBase, IRequestHandler<SetNoteStatusRequest, Guid>
    {
        private readonly IAddressUtilities addressUtilities;
        private readonly IEvidenceDataAccess evidenceDataAccess;

        public SetNoteStatusRequestHandler(
            WeeeContext context,
            IUserContext userContext,
            IWeeeAuthorization authorization, 
            ISystemDataDataAccess systemDataDataAccess,
            IAddressUtilities addressUtilities,
            IEvidenceDataAccess evidenceDataAccess) : base(context, userContext, authorization, systemDataDataAccess)
        {
            this.addressUtilities = addressUtilities;
            this.evidenceDataAccess = evidenceDataAccess;
        }

        public async Task<Guid> HandleAsync(SetNoteStatusRequest message)
        {
            Authorization.EnsureCanAccessExternalArea();

            var evidenceNote = await EvidenceNote(message.NoteId);

            Authorization.EnsureOrganisationAccess(message.Status == NoteStatus.Submitted
                ? evidenceNote.Organisation.Id
                : evidenceNote.Recipient.Id);

            evidenceDataAccess.DeleteZeroTonnageFromSubmittedTransferNote(evidenceNote, message.Status.ToDomainEnumeration<Domain.Evidence.NoteStatus>(), evidenceNote.NoteType);

            var currentDate = await SystemDataDataAccess.GetSystemDateTime();

            ValidToSave(evidenceNote.Recipient, evidenceNote.ComplianceYear, currentDate);

            if (message.Status == NoteStatus.Approved && evidenceNote.Recipient.IsBalancingScheme == false)
            {
                var organisationAddress = evidenceNote.Recipient.HasBusinessAddress
                    ? evidenceNote.Recipient.BusinessAddress
                    : evidenceNote.Recipient.NotificationAddress;

                evidenceNote.SetApprovedRecipientAddress(evidenceNote.Recipient.Scheme.SchemeName, addressUtilities.FormattedCompanyPcsAddress(evidenceNote.Recipient.Scheme.SchemeName,
                    evidenceNote.Recipient.OrganisationName,
                    organisationAddress.Address1,
                    organisationAddress.Address2,
                    organisationAddress.TownOrCity,
                    organisationAddress.CountyOrRegion,
                    organisationAddress.Postcode,
                    null));

                if (evidenceNote.NoteType == Domain.Evidence.NoteType.TransferNote)
                {
                    evidenceNote.SetApprovedTransfererAddress(evidenceNote.Organisation.Scheme.SchemeName, addressUtilities.FormattedCompanyPcsAddress(evidenceNote.Organisation.Scheme.SchemeName,
                        evidenceNote.Organisation.OrganisationName,
                        organisationAddress.Address1,
                        organisationAddress.Address2,
                        organisationAddress.TownOrCity,
                        organisationAddress.CountyOrRegion,
                        organisationAddress.Postcode,
                        null));
                }
            }

            return await UpdateNoteStatus(evidenceNote, message.Status, currentDate, message.Reason);
        }
    }
}
