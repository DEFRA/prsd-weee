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
    using Prsd.Core;
    using Weee.Tests.Core;

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
            var recipientOrganisation = DbContext.Organisations.First(o => o.Name.Equals(TestingConstants.TestCompanyName));
            var aatf = DbContext.Aatfs.First(s => s.Name.Equals(TestingConstants.TestCompanyName));
            var user = Container.Resolve<IUserContext>();

            instance = new Note(organisation,
                recipientOrganisation,
            DateTime.Now,
            DateTime.Now.AddDays(10),
            WasteType.HouseHold,
            Fixture.Create<Protocol>(),
            aatf,
            user.UserId.ToString(),
            tonnages);
        }

        public EvidenceNoteDbSetup WithRecipient(Guid recipientId)
        {
            ObjectInstantiator<Note>.SetProperty(n => n.Recipient, null, instance);
            ObjectInstantiator<Note>.SetProperty(n => n.RecipientId, recipientId, instance);
            return this;
        }

        public EvidenceNoteDbSetup WithOrganisation(Guid organisationId)
        {
            ObjectInstantiator<Note>.SetProperty(o => o.Organisation, null, instance);
            ObjectInstantiator<Note>.SetProperty(o => o.OrganisationId, organisationId, instance);
            return this;
        }

        public EvidenceNoteDbSetup WithAatf(Guid aatfId)
        {
            instance.UpdateAatf(aatfId);
            return this;
        }

        public EvidenceNoteDbSetup WithStatus(NoteStatus statusToUpdate, string user)
        {
            instance.UpdateStatus(statusToUpdate, user, SystemTime.UtcNow);
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

        public EvidenceNoteDbSetup WithWasteType(WasteType wasteType)
        {
            ObjectInstantiator<Note>.SetProperty(o => o.WasteType, wasteType, instance);
            
            return this;
        }
    }
}
