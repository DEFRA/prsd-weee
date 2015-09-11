namespace EA.Weee.DataAccess.EventHandlers
{
    using EA.Prsd.Core.Domain;
    using EA.Weee.Domain.Events;
    using EA.Weee.Domain.Producer;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class MemberUploadSubmittedEventHandler : IEventHandler<MemberUploadSubmittedEvent>
    {
        private readonly WeeeContext context;

        public MemberUploadSubmittedEventHandler(WeeeContext context)
        {
            this.context = context;
        }

        public async Task HandleAsync(MemberUploadSubmittedEvent @event)
        {
            var currentProducers = await context.Producers
                .Where(p => p.SchemeId == @event.MemberUpload.SchemeId)
                .Where(p => p.MemberUpload.ComplianceYear == @event.MemberUpload.ComplianceYear)
                .Where(p => p.IsCurrentForComplianceYear)
                .ToListAsync();

            foreach (Producer newVersion in @event.MemberUpload.Producers)
            {
                Producer previousVersion = currentProducers
                    .Where(p => p.RegistrationNumber == newVersion.RegistrationNumber)
                    .SingleOrDefault();

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
