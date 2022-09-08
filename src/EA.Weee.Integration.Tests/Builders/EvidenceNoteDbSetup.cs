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
            var date = SystemTime.UtcNow;

            instance.UpdateStatus(statusToUpdate, user, new DateTime(instance.ComplianceYear, date.Month, date.Day, date.Hour, date.Minute, date.Second));

            return this;
        }

        public EvidenceNoteDbSetup WithStatusUpdate(NoteStatus statusToUpdate)
        {
            var date = SystemTime.UtcNow;
            var updateDate = new DateTime(instance.ComplianceYear, date.Month, date.Day, date.Hour, date.Minute, date.Second);

            if (statusToUpdate == NoteStatus.Draft)
            {
                return this;
            }
            else
            {
                if (statusToUpdate == NoteStatus.Submitted)
                {
                    instance.UpdateStatus(NoteStatus.Submitted, DbContext.GetCurrentUser(), updateDate);
                }
                else if (statusToUpdate == NoteStatus.Approved)
                {
                    instance.UpdateStatus(NoteStatus.Submitted, DbContext.GetCurrentUser(), updateDate);
                    instance.UpdateStatus(NoteStatus.Approved, DbContext.GetCurrentUser(), updateDate);
                }
                else if (statusToUpdate == NoteStatus.Rejected)
                {
                    instance.UpdateStatus(NoteStatus.Submitted, DbContext.GetCurrentUser(), updateDate);
                    instance.UpdateStatus(NoteStatus.Rejected, DbContext.GetCurrentUser(), updateDate);
                }
                else if (statusToUpdate == NoteStatus.Returned)
                {
                    instance.UpdateStatus(NoteStatus.Submitted, DbContext.GetCurrentUser(), updateDate);
                    instance.UpdateStatus(NoteStatus.Returned, DbContext.GetCurrentUser(), updateDate);
                }
                else if (statusToUpdate == NoteStatus.Void)
                {
                    instance.UpdateStatus(NoteStatus.Submitted, DbContext.GetCurrentUser(), updateDate);
                    instance.UpdateStatus(NoteStatus.Approved, DbContext.GetCurrentUser(), updateDate);
                    instance.UpdateStatus(NoteStatus.Void, DbContext.GetCurrentUser(), updateDate);
                }
            }
            
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
            
            var r = new Random();
            var rand62Bit = (((long)r.Next()) << 31) + r.Next();
            var startDate = new DateTime(complianceYear, 1, 1);
            var endDate = new DateTime(complianceYear, 12, 31);

            var newStartDate = startDate + new TimeSpan(rand62Bit % (endDate - startDate).Ticks);
            var newEndDate = newStartDate + new TimeSpan(rand62Bit % (endDate - newStartDate).Ticks);

            ObjectInstantiator<Note>.SetProperty(o => o.StartDate, newStartDate, instance);
            ObjectInstantiator<Note>.SetProperty(o => o.EndDate, newEndDate, instance);

            return this;
        }

        public EvidenceNoteDbSetup WithWasteType(WasteType wasteType)
        {
            ObjectInstantiator<Note>.SetProperty(o => o.WasteType, wasteType, instance);

            return this;
        }

        public EvidenceNoteDbSetup WithProtocol(Protocol protocol)
        {
            ObjectInstantiator<Note>.SetProperty(o => o.Protocol, protocol, instance);

            return this;
        }
    }
}
