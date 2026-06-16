namespace EA.Weee.RequestHandlers.AatfReturn
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class AddReturnSchemeHandler : IRequestHandler<AddReturnScheme, List<Guid>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IReturnSchemeDataAccess returnSchemeDataAccess;
        private readonly IReturnDataAccess returnDataAccess;
        private readonly ISchemeDataAccess schemeDataAccess;

        public AddReturnSchemeHandler(IWeeeAuthorization authorization, IReturnSchemeDataAccess returnSchemeDataAccess, IReturnDataAccess returnDataAccess, ISchemeDataAccess schemeDataAccess)
        {
            this.authorization = authorization;
            this.returnSchemeDataAccess = returnSchemeDataAccess;
            this.returnDataAccess = returnDataAccess;
            this.schemeDataAccess = schemeDataAccess;
        }

        public async Task<List<Guid>> HandleAsync(AddReturnScheme message)
        {
            authorization.EnsureCanAccessExternalArea();

            var @return = await returnDataAccess.GetById(message.ReturnId);

            // Make the handler idempotent: duplicate or concurrent submissions
            // (double-click, browser retry, second tab) must not create
            // duplicate ReturnScheme rows for the same (ReturnId, SchemeId).
            var existingSchemes = await returnSchemeDataAccess.GetSelectedSchemesByReturnId(message.ReturnId);
            var existingSchemeIds = new HashSet<Guid>(existingSchemes.Select(rs => rs.SchemeId));

            var returnSchemes = new List<ReturnScheme>();
            foreach (var schemeId in message.SchemeIds.Distinct())
            {
                if (existingSchemeIds.Contains(schemeId))
                {
                    continue;
                }

                var scheme = await schemeDataAccess.GetSchemeOrDefault(schemeId);
                returnSchemes.Add(new ReturnScheme(scheme, @return));
            }

            if (returnSchemes.Count == 0)
            {
                return new List<Guid>();
            }

            return await returnSchemeDataAccess.Submit(returnSchemes);
        }
    }
}
