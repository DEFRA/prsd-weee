namespace EA.Weee.RequestHandlers.AatfReturn
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using AatfTaskList;
    using CheckYourReturn;
    using Core.AatfReturn;
    using Core.Helpers;
    using DataAccess;
    using Domain.AatfReturn;
    using NonObligated;
    using Prsd.Core.Domain;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.AatfReturn;
    using Security;
    using ReturnStatus = Core.AatfReturn.ReturnStatus;

    internal class CopyReturnHandler : IRequestHandler<CopyReturn, Guid>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IUserContext userContext;
        private readonly WeeeContext context;

        public CopyReturnHandler(IWeeeAuthorization authorization,
            WeeeContext context,
            IUserContext userContext)
        {
            this.authorization = authorization;
            this.userContext = userContext;
            this.context = context;
        }

        public async Task<Guid> HandleAsync(CopyReturn message)
        {
            authorization.EnsureCanAccessExternalArea();

            var returnCopy = await context.Returns.Include(r => r.Organisation).FirstOrDefaultAsync(r => r.Id == message.ReturnId);

            if (returnCopy == null)
            {
                throw new ArgumentException($"No return was found with id {message.ReturnId}.");
            }

            authorization.EnsureOrganisationAccess(returnCopy.Organisation.Id);

            await CopyReturnReportsOn(message, returnCopy);

            await CopyReturnSchemes(message, returnCopy);

            await CopyReturnNonObligated(message, returnCopy);

            CopyReturnReceived(message, returnCopy);

            CopyReturnSentOn(message, returnCopy);

            CopyReturnReused(message, returnCopy);

            CopyReturn(message, returnCopy);

            await context.SaveChangesAsync();

            return @returnCopy.Id;
        }

        private void CopyReturn(CopyReturn message, Return returnCopy)
        {
            returnCopy.ResetSubmitted(userContext.UserId.ToString(), message.ReturnId);
            context.Entry(returnCopy.Organisation).State = EntityState.Unchanged;

            context.Returns.Add(returnCopy);
        }

        private void CopyReturnReused(CopyReturn message, Return returnCopy)
        {
            var reused = context.WeeeReused.Where(w => w.ReturnId == message.ReturnId)
                .Include(w => w.WeeeReusedSites.Select(s => s.Address))
                .Include(w => w.WeeeReusedAmounts)
                .Include(w => w.Aatf)
                .ToList();

            reused.ForEach(s => s.UpdateReturn(returnCopy));
            reused.ForEach(r => context.Entry(r.Aatf).State = EntityState.Unchanged);
            reused.ForEach(r => r.WeeeReusedSites.ToList().ForEach(w => context.Entry(w.Address).State = EntityState.Added));
            reused.ForEach(r => r.WeeeReusedSites.ToList().ForEach(w => context.Entry(w).State = EntityState.Added));
            reused.ForEach(r => r.WeeeReusedAmounts.ToList().ForEach(w => context.Entry(w).State = EntityState.Added));

            context.WeeeReused.AddRange(reused);
        }

        private void CopyReturnSentOn(CopyReturn message, Return returnCopy)
        {
            var sentOn = context.WeeeSentOn.Where(w => w.ReturnId == message.ReturnId)
                .Include(w => w.WeeeSentOnAmounts)
                .Include(w => w.OperatorAddress)
                .Include(w => w.Aatf)
                .Include(w => w.SiteAddress)
                .ToList();

            sentOn.ForEach(s => s.UpdateReturn(returnCopy));
            sentOn.ForEach(s => s.WeeeSentOnAmounts.ToList().ForEach(w => context.Entry(w).State = EntityState.Added));
            sentOn.ForEach(s => context.Entry(s.OperatorAddress).State = EntityState.Added);
            sentOn.ForEach(s => context.Entry(s.SiteAddress).State = EntityState.Added);
            sentOn.ForEach(s => context.Entry(s.Aatf).State = EntityState.Unchanged);

            context.WeeeSentOn.AddRange(sentOn);
        }

        private void CopyReturnReceived(CopyReturn message, Return returnCopy)
        {
            var received = context.WeeeReceived.Where(w => w.ReturnId == message.ReturnId)
                .Include(w => w.WeeeReceivedAmounts)
                .ToList();

            received.ForEach(r => r.UpdateReturn(returnCopy));
            received.ForEach(r => r.WeeeReceivedAmounts.ToList().ForEach(w => context.Entry(w).State = EntityState.Added));
            received.ForEach(s => context.Entry(s.Aatf).State = EntityState.Unchanged);
            received.ForEach(s => context.Entry(s.Scheme).State = EntityState.Unchanged);

            context.WeeeReceived.AddRange(received);
        }

        private async Task CopyReturnNonObligated(CopyReturn message, Return returnCopy)
        {
            var nonObligated = await context.NonObligatedWeee.Where(n => n.ReturnId == message.ReturnId).ToListAsync();
            nonObligated.ForEach(n => n.UpdateReturn(returnCopy));

            context.NonObligatedWeee.AddRange(nonObligated);
        }

        private async Task CopyReturnSchemes(CopyReturn message, Return returnCopy)
        {
            var schemes = await context.ReturnScheme.Where(r => r.ReturnId == message.ReturnId).ToListAsync();
            schemes.ForEach(s => s.UpdateReturn(returnCopy));

            context.ReturnScheme.AddRange(schemes);
        }

        private async Task CopyReturnReportsOn(CopyReturn message, Return returnCopy)
        {
            var reportsOn = await context.ReturnReportOns.Where(r => r.ReturnId == message.ReturnId).ToListAsync();
            reportsOn.ForEach(s => s.UpdateReturn(returnCopy));

            context.ReturnReportOns.AddRange(reportsOn);
        }
    }
}