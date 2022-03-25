namespace EA.Weee.Integration.Tests.Builders
{
    using System;
    using System.Linq;
    using Api.Identity;
    using Autofac;
    using Base;
    using Domain;
    using Domain.AatfReturn;
    using Domain.Evidence;
    using Domain.Organisation;
    using Prsd.Core.Domain;

    public class EvidenceNoteDbSetup : DbTestDataBuilder<Note, EvidenceNoteDbSetup>
    {
        protected override Note Instantiate()
        {
            var organisation = DbContext.Organisations.First(o => o.Name.Equals(TestingConstants.TestCompanyName));
            var scheme = DbContext.Schemes.First(s => s.SchemeName.Equals(TestingConstants.TestCompanyName));
            var aatf = DbContext.Aatfs.First(s => s.Name.Equals(TestingConstants.TestCompanyName));
            var user = Container.Resolve<IUserContext>();

            instance = new Note(organisation,
                scheme,
                DateTime.Now,
                DateTime.Now.AddDays(10),
                WasteType.HouseHold,
                Protocol.Actual,
                aatf,
                NoteType.EvidenceNote,
                user.UserId.ToString());

            return instance;
        }

        protected EvidenceNoteDbSetup WithOrganisation(Organisation organisation)
        {
            instance.UpdateOrganisation(organisation);

            return this;
        }
    }
}
