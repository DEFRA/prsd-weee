namespace EA.Weee.Integration.Tests.Builders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Autofac;
    using Base;
    using Core.Helpers;
    using Domain.Evidence;
    using Prsd.Core.Domain;

    public class TransferEvidenceNoteDbSetup : DbTestDataBuilder<Note, TransferEvidenceNoteDbSetup>
    {
        protected override Note Instantiate()
        {
            DefaultNote(new List<NoteTransferTonnage>(), new List<NoteTransferCategory>());
            return instance;
        }

        private void DefaultNote(IList<NoteTransferTonnage> tonnages, IList<NoteTransferCategory> categories)
        {
            var organisation = DbContext.Organisations.First(o => o.Name.Equals(TestingConstants.TestCompanyName));
            var scheme = DbContext.Schemes.First(s => s.SchemeName.Equals(TestingConstants.TestCompanyName));
            var user = Container.Resolve<IUserContext>();

            instance = new Note(organisation,
            scheme,
            user.UserId.ToString(),
            tonnages,
            categories,
            DateTime.Now.Year,
            WasteType.HouseHold);
        }

        public TransferEvidenceNoteDbSetup WithStatus(NoteStatus statusToUpdate, string user, string reason = null)
        {
            instance.UpdateStatus(statusToUpdate, user, reason);
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
