namespace EA.Weee.DataAccess.EventHandlers
{
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.Events;
    using Domain.Producer;
    using Prsd.Core.Domain;

    public class MemberUploadSubmittedEventHandler : IEventHandler<MemberUploadSubmittedEvent>
    {
        private readonly WeeeContext context;

        public MemberUploadSubmittedEventHandler(WeeeContext context)
        {
            this.context = context;
        }

        public async Task HandleAsync(MemberUploadSubmittedEvent @event)
        {
            /* This query relies on producers being indexed by scheme to avoid
             * taking shared read locks on producers outside of this scheme.
             * By using this index, deadlocks will be avoided provided that multiple
             * uploads for the same scheme are not being submitted concurrently.
             */
            var currentProducers = await context.Producers
                .Where(p => p.SchemeId == @event.MemberUpload.SchemeId)
                .Where(p => p.MemberUpload.ComplianceYear == @event.MemberUpload.ComplianceYear)
                .Where(p => p.IsCurrentForComplianceYear)
                .ToListAsync();

            foreach (Producer newVersion in @event.MemberUpload.Producers)
            {
                Producer previousVersion = currentProducers
                    .SingleOrDefault(p => p.RegistrationNumber == newVersion.RegistrationNumber);

                if (previousVersion != null)
                {
                    if (previousVersion.UpdatedDate < newVersion.UpdatedDate)
                    {
                        previousVersion.IsCurrentForComplianceYear = false;
                        newVersion.IsCurrentForComplianceYear = true;
                    }
                }
                else
                {
                    newVersion.IsCurrentForComplianceYear = true;
                }
            }
            await context.SaveChangesAsync();
        }
    }
}
