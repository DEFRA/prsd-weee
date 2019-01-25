namespace EA.Weee.RequestHandlers.AatfReturn
{
    using DataAccess;
    using Domain.AatfReturn;
    using Domain.Organisation;
    using Domain.User;
    using Prsd.Core.Domain;
    using Prsd.Core.Mediator;
    using Requests.AatfReturn;
    using Requests.Organisations.Create;
    using Security;
    using System;
    using System.Threading.Tasks;

    public class CreateNonObligatedRequestHandler
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext db;
        private readonly IUserContext userContext;

        public CreateNonObligatedRequestHandler(IWeeeAuthorization authorization, WeeeContext db, IUserContext userContext)
        {
            this.authorization = authorization;
            this.db = db;
            this.userContext = userContext;
        }

        public async Task<Guid> HandleAsync(NonObligatedRequest message)
        {
            authorization.EnsureCanAccessExternalArea();
            var organisation = Organisation.CreateSoleTrader("TESTINGTRADING");
            db.Organisations.Add(organisation);

            var operatorTest = new Operator(Guid.NewGuid(), organisation.Id);
            db.Operator.Add(operatorTest);

            var returnTest = new Return(Guid.NewGuid(), operatorTest.Id, 1, 1, 1);
            db.Return.Add(returnTest);

            var nonobligated = new NonObligatedWeee(message.NonObligatedId, returnTest.Id, message.CategoryId, message.Dcf, message.Tonnage);

            db.NonObligatedWeee.Add(nonobligated);

            await db.SaveChangesAsync();

            return nonobligated.Id;
        }
    }
}
