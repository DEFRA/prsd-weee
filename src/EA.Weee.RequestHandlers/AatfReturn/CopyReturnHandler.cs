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

    internal class CopyReturnHandler : IRequestHandler<CopyReturn, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IReturnDataAccess returnDataAccess;
        private readonly IUserContext userContext;
        private readonly WeeeContext context;

        public CopyReturnHandler(IWeeeAuthorization authorization,
            WeeeContext context,
            IReturnDataAccess returnDataAccess,
            IUserContext userContext)
        {
            this.authorization = authorization;
            this.returnDataAccess = returnDataAccess;
            this.userContext = userContext;
            this.context = context;
        }

        public async Task<bool> HandleAsync(CopyReturn message)
        {
            authorization.EnsureCanAccessExternalArea();

            var @return = await returnDataAccess.GetById(message.ReturnId);

            if (@return == null)
            {
                throw new ArgumentException($"No return was found with id {message.ReturnId}.");
            }

            var returnCopy = await context.Returns.FirstAsync(o => o.Id == message.ReturnId);

            var reportsOn = await context.ReturnReportOns.Where(r => r.ReturnId == message.ReturnId).ToListAsync();
            reportsOn.ForEach(s => s.UpdateReturn(@returnCopy));

            var schemes = await context.ReturnScheme.Where(r => r.ReturnId == message.ReturnId).ToListAsync();
            schemes.ForEach(s => s.UpdateReturn(@returnCopy));

            var nonObligated = await context.NonObligatedWeee.Where(n => n.ReturnId == message.ReturnId).ToListAsync();

            nonObligated.ForEach(n => n.UpdateReturn(@returnCopy));

            var received = context.WeeeReceived.Where(w => w.ReturnId == message.ReturnId)
                .Include(w => w.WeeeReceivedAmounts)
                .AsNoTracking()
                .ToList();

            received.ForEach(r => r.UpdateReturn(@returnCopy));

            var sentOn = context.WeeeSentOn.Where(w => w.ReturnId == message.ReturnId)
                .Include(w => w.WeeeSentOnAmounts)
                .Include(w => w.OperatorAddress)
                .Include(w => w.Aatf)
                .Include(w => w.SiteAddress)
                .AsNoTracking()
                .ToList();

            sentOn.ForEach(s => s.UpdateReturn(@returnCopy));
            sentOn.ForEach(s => context.Entry(s.Aatf).State = EntityState.Unchanged);

            var reused = context.WeeeReused.Where(w => w.ReturnId == message.ReturnId)
                .Include(w => w.WeeeReusedSites.Select(s => s.Address))
                .Include(w => w.WeeeReusedAmounts)
                .Include(w => w.Aatf)
                .AsNoTracking()
                .ToList();

            reused.ForEach(s => s.UpdateReturn(@returnCopy));
            reused.ForEach(r => context.Entry(r.Aatf).State = EntityState.Unchanged);

            returnCopy.ResetSubmitted(userContext.UserId.ToString(), message.ReturnId);

            context.ReturnReportOns.AddRange(reportsOn);
            context.ReturnScheme.AddRange(schemes);
            context.WeeeReused.AddRange(reused);
            context.WeeeSentOn.AddRange(sentOn);
            context.WeeeReceived.AddRange(received);
            context.Returns.Add(@returnCopy);

            await context.SaveChangesAsync();

            return true;
        }
    }
}