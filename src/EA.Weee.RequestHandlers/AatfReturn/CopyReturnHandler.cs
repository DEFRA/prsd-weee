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

            var returnCopy = await context.Returns.Include(r => r.Operator.Organisation).FirstOrDefaultAsync(r => r.Id == message.ReturnId);

            if (returnCopy == null)
            {
                throw new ArgumentException($"No return was found with id {message.ReturnId}.");
            }

            authorization.EnsureOrganisationAccess(returnCopy.Operator.Organisation.Id);

            context.Entry(returnCopy.Operator).State = EntityState.Unchanged;

            var reportsOn = await context.ReturnReportOns.Where(r => r.ReturnId == message.ReturnId).ToListAsync();
            reportsOn.ForEach(s => s.UpdateReturn(returnCopy));

            context.ReturnReportOns.AddRange(reportsOn);

            var schemes = await context.ReturnScheme.Where(r => r.ReturnId == message.ReturnId).ToListAsync();
            schemes.ForEach(s => s.UpdateReturn(returnCopy));

            context.ReturnScheme.AddRange(schemes);

            var nonObligated = await context.NonObligatedWeee.Where(n => n.ReturnId == message.ReturnId).ToListAsync();
            nonObligated.ForEach(n => n.UpdateReturn(returnCopy));

            context.NonObligatedWeee.AddRange(nonObligated);

            var received = context.WeeeReceived.Where(w => w.ReturnId == message.ReturnId)
                .Include(w => w.WeeeReceivedAmounts)
                .ToList();

            received.ForEach(r => r.UpdateReturn(returnCopy));
            received.ForEach(r => r.WeeeReceivedAmounts.ToList().ForEach(w => context.Entry(w).State = EntityState.Added));
            received.ForEach(s => context.Entry(s.Aatf).State = EntityState.Unchanged);
            received.ForEach(s => context.Entry(s.Scheme).State = EntityState.Unchanged);

            context.WeeeReceived.AddRange(received);

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

            returnCopy.ResetSubmitted(userContext.UserId.ToString(), message.ReturnId);

            context.Returns.Add(returnCopy);

            await context.SaveChangesAsync();

            return @returnCopy.Id;
        }
    }
}