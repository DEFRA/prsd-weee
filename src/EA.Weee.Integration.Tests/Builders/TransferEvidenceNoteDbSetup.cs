namespace EA.Weee.Integration.Tests.Builders
{
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
            DefaultNote(new List<NoteTransferTonnage>());
            return instance;
        }

        private void DefaultNote(IList<NoteTransferTonnage> tonnages)
        {
            var organisation = DbContext.Organisations.First(o => o.Name.Equals(TestingConstants.TestCompanyName));
            var scheme = DbContext.Schemes.First(s => s.SchemeName.Equals(TestingConstants.TestCompanyName));
            var user = Container.Resolve<IUserContext>();

            instance = new Note(organisation,
            scheme,
            user.UserId.ToString(),
            tonnages);
        }

        public TransferEvidenceNoteDbSetup WithStatus(NoteStatus statusToUpdate, string user)
        {
            instance.UpdateStatus(statusToUpdate, user);
            return this;
        }

        public TransferEvidenceNoteDbSetup WithTonnages(List<NoteTransferTonnage> tonnages)
        {
            instance.NoteTransferTonnage.Clear();
            instance.NoteTransferTonnage.AddRange(tonnages);
            return this;
        }
    }
}
