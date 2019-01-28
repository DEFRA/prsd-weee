namespace EA.Weee.RequestHandlers.AatfReturn
{
    using Core.AatfReturn;
    using DataAccess;
    using Domain.AatfReturn;
    using Domain.Organisation;
    using Domain.User;
    using Prsd.Core.Domain;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using RequestHandlers.Mappings;
    using Requests.AatfReturn;
    using Requests.Organisations.Create;
    using Security;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class NonObligatedRequestHandler
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext db;
        private readonly IUserContext userContext;
        public List<NonObligatedWeee> Nonobligatedlist;

        public NonObligatedRequestHandler(IWeeeAuthorization authorization, WeeeContext db, IUserContext userContext)
        {
            this.authorization = authorization;
            this.db = db;
            this.userContext = userContext;
        }

        public async Task<Guid> HandleAsync(NonObligatedCategoryValue message)
        {
            authorization.EnsureCanAccessExternalArea();
            var organisation = Organisation.CreateSoleTrader("TESTINGTRADING");
            db.Organisations.Add(organisation);

            var operatorTest = new Operator(Guid.NewGuid(), organisation.Id);
            db.Operator.Add(operatorTest);

            var returnTest = new Return(Guid.NewGuid(), operatorTest.Id, 1, 1, 1);
            db.Return.Add(returnTest);

            //for (var i = 0; i < message.CategoryValues.Count; i++)
            //{
            //    var nonobligated = new NonObligatedWeee(message.NonObligatedId, returnTest.Id, message.CategoryValues[i].Category, message.CategoryValues[i].Dcf, message.CategoryValues[i].NonObligated);
            //    db.NonObligatedWeee.Add(nonobligated);
            //}
            //db.NonObligatedWeee.Add(request);

            await db.SaveChangesAsync();

            return Guid.NewGuid();
        }
    }
}
