namespace EA.Weee.RequestHandlers.AatfReturn.NonObligated
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.AatfReturn;
    using Domain.Organisation;
    using Prsd.Core.Domain;
    using Prsd.Core.Mediator;
    using Requests.AatfReturn;
    using Security;

    internal class AddNonObligatedRequestHandler : IRequestHandler<AddNonObligatedRequest, Guid>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IUserContext userContext;
        private readonly IAddNonObligatedDataAccess nonObligatedDataAccess;
        private readonly WeeeContext context;
        public List<NonObligatedWeee> Nonobligatedlist;

        public AddNonObligatedRequestHandler(IWeeeAuthorization authorization, WeeeContext db, IUserContext userContext, IAddNonObligatedDataAccess nonObligatedDataAccess, WeeeContext context)
        {
            this.authorization = authorization;
            this.userContext = userContext;
            this.nonObligatedDataAccess = nonObligatedDataAccess;
            this.context = context;
        }

        public async Task<Guid> HandleAsync(AddNonObligatedRequest message)
        {
            authorization.EnsureCanAccessExternalArea();

            var operatorTest = new Operator(Guid.NewGuid(), message.OrganisationId);

            var aatfReturn = new Return(Guid.NewGuid(), 1, 1, 1, operatorTest);

            var nonObligatedWee = new List<NonObligatedWeee>();

            for (var i = 0; i < message.CategoryValues.Count; i++)
            {
                nonObligatedWee.Add(new NonObligatedWeee(message.NonObligatedId, aatfReturn, message.CategoryValues[i].Category, message.CategoryValues[i].Dcf, message.CategoryValues[i].Tonnage));
            }

            await nonObligatedDataAccess.Submit(nonObligatedWee);
            
            return Guid.NewGuid();
        }
    }
}
