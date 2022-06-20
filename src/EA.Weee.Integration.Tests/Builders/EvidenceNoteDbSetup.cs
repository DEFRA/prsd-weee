namespace EA.Weee.Integration.Tests.Builders
{
    using Autofac;
    using AutoFixture;
    using Base;
    using Core.Helpers;
    using Domain.Evidence;
    using Prsd.Core.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class EvidenceNoteDbSetup : DbTestDataBuilder<Note, EvidenceNoteDbSetup>
    {
        protected override Note Instantiate()
        {
            DefaultNote(new List<NoteTonnage>());
            return instance;
        }

        private void DefaultNote(List<NoteTonnage> tonnages)
        {
            var organisation = DbContext.Organisations.First(o => o.Name.Equals(TestingConstants.TestCompanyName));
            var scheme = DbContext.Schemes.First(s => s.SchemeName.Equals(TestingConstants.TestCompanyName));
            var aatf = DbContext.Aatfs.First(s => s.Name.Equals(TestingConstants.TestCompanyName));
            var user = Container.Resolve<IUserContext>();

            instance = new Note(organisation,
            scheme,
            DateTime.Now,
            DateTime.Now.AddDays(10),
            Fixture.Create<WasteType>(),
            Fixture.Create<Protocol>(),
            aatf,
            user.UserId.ToString(),
            tonnages);
        }

        public EvidenceNoteDbSetup WithRecipient(Guid schemeId)
        {
            instance.UpdateScheme(schemeId);
            return this;
        }

        public EvidenceNoteDbSetup WithOrganisation(Guid organisationId)
        {
            instance.UpdateOrganisation(organisationId);
            return this;
        }

        public EvidenceNoteDbSetup WithAatf(Guid aatfId)
        {
            instance.UpdateAatf(aatfId);
            return this;
        }

        public EvidenceNoteDbSetup WithStatus(NoteStatus statusToUpdate, string user)
        {
            instance.UpdateStatus(statusToUpdate, user);
            return this;
        }

        public EvidenceNoteDbSetup WithTonnages(List<NoteTonnage> tonnages)
        {
            instance.NoteTonnage.Clear();
            instance.NoteTonnage.AddRange(tonnages);
            return this;
        }

        public EvidenceNoteDbSetup WithComplianceYear(int complianceYear)
        {
            instance.ComplianceYear = complianceYear;
            return this;
        }
    }
}
