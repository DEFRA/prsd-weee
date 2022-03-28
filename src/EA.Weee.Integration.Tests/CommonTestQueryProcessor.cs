namespace EA.Weee.Integration.Tests
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using Autofac;
    using DataAccess;
    using Domain;
    using Domain.Evidence;
    using Domain.Scheme;
    using Prsd.Core.Autofac;

    public class CommonTestQueryProcessor
    {
        private readonly WeeeContext dbContext;

        public CommonTestQueryProcessor()
        {
            dbContext = ServiceLocator.Container.Resolve<WeeeContext>();
        }

        public Country GetCountryById(Guid id)
        {
            return dbContext.Countries.First(c => c.Id.Equals(id));
        }

        public Note GetEvidenceNoteById(Guid id)
        {
            return dbContext.Notes.FirstOrDefault(n => n.Id.Equals(id));
        }

        public Note GetEvidenceNoteByReference(int reference)
        {
            return dbContext.Notes.FirstOrDefault(n => n.Reference == reference);
        }
    }
}
