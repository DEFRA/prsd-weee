namespace EA.Weee.Integration.Tests.Builders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Autofac;
    using Base;
    using Core.Helpers;
    using Domain.Evidence;
    using Prsd.Core;
    using Prsd.Core.Domain;

    public class TransferEvidenceNoteDbSetup : DbTestDataBuilder<Note, TransferEvidenceNoteDbSetup>
    {
        protected override Note Instantiate()
        {
            DefaultNote(new List<NoteTransferTonnage>());
            return instance;
        }

        private void DefaultNote(IList<NoteTransferTonnage> tonnages)
        {
            var organisation = DbContext.Organisations.First(o => o.Name.Equals(TestingConstants.TestCompanyName));
            var recipientOrganisation = DbContext.Organisations.First(o => o.Name.Equals(TestingConstants.TestCompanyName));
            var user = Container.Resolve<IUserContext>();

            instance = new Note(organisation,
                recipientOrganisation,
            user.UserId.ToString(),
            tonnages,
            DateTime.Now.Year);
        }

        public TransferEvidenceNoteDbSetup WithStatus(NoteStatus statusToUpdate, string user, string reason = null)
        {
            instance.UpdateStatus(statusToUpdate, user, SystemTime.UtcNow, reason);
            return this;
        }

        public TransferEvidenceNoteDbSetup WithTonnages(List<NoteTransferTonnage> tonnages)
        {
            instance.NoteTransferTonnage.Clear();
            instance.NoteTransferTonnage.AddRange(tonnages);
            return this;
        }

        public TransferEvidenceNoteDbSetup WithOrganisation(Guid organisationId)
        {
            instance.UpdateOrganisation(organisationId);
            return this;
        }
    }
}
