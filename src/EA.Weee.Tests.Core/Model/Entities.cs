namespace EA.Weee.Tests.Core.Model
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public partial class Entities
    {
        public override int SaveChanges()
        {
            /* 
             * Entity Framework is unable to handle the circular reference between
             * the registered producer table and the producer submission table.
             * By overriding the SaveChanges() method, we can perform the update
             * in two parts. Normally this would require overriding EF's unit of
             * work approach to ensure atomicity, but our database tests are already
             * wrapped in a transaction which we know will abort anyway.
             */

            var newRegisteredProducers = ChangeTracker.Entries<RegisteredProducer>()
                .Where(e => e.State == EntityState.Added)
                .Select(e => e.Entity)
                .ToList();

            Dictionary<Guid, Guid?> currentSubmissionIds =
                newRegisteredProducers.ToDictionary(rp => rp.Id, rp => rp.CurrentSubmissionId);

            var newDataReturns = ChangeTracker.Entries<DataReturn>()
                .Where(e => e.State == EntityState.Added)
                .Select(e => e.Entity)
                .ToList();

            Dictionary<Guid, Guid?> currentDataReturnVersionIds
                = newDataReturns.ToDictionary(dr => dr.Id, dr => dr.CurrentDataReturnVersionId);

            foreach (RegisteredProducer newRegisteredProducer in newRegisteredProducers)
            {
                newRegisteredProducer.CurrentSubmissionId = null;
            }

            foreach (var newDataReturn in newDataReturns)
            {
                newDataReturn.CurrentDataReturnVersionId = null;
            }

            base.SaveChanges();

            foreach (RegisteredProducer newRegisteredProducer in newRegisteredProducers)
            {
                newRegisteredProducer.CurrentSubmissionId = currentSubmissionIds[newRegisteredProducer.Id];
            }

            foreach (var newDataReturn in newDataReturns)
            {
                newDataReturn.CurrentDataReturnVersionId = currentDataReturnVersionIds[newDataReturn.Id];
            }

            return base.SaveChanges();
        }
    }
}
