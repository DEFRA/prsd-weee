namespace EA.Weee.Tests.Core.Model
{
    using System;
    using System.Collections.Generic;
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
                .Where(e => e.State == System.Data.Entity.EntityState.Added)
                .Select(e => e.Entity)
                .ToList();

            if (newRegisteredProducers.Count > 0)
            {
                Dictionary<Guid, Guid?> currentSubmissionIds =
                    newRegisteredProducers.ToDictionary(rp => rp.Id, rp => rp.CurrentSubmissionId);

                foreach (RegisteredProducer newRegisteredProducer in newRegisteredProducers)
                {
                    newRegisteredProducer.CurrentSubmissionId = null;
                }

                base.SaveChanges();

                foreach (RegisteredProducer newRegisteredProducer in newRegisteredProducers)
                {
                    newRegisteredProducer.CurrentSubmissionId = currentSubmissionIds[newRegisteredProducer.Id];
                }
            }

            return base.SaveChanges();
        }
    }
}
