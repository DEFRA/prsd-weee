namespace EA.Weee.RequestHandlers.AatfReturn
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Domain.DataReturns;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;

    internal class GetPreviousQuarterSchemesHandler : IRequestHandler<GetPreviousQuarterSchemes, PreviousQuarterReturnResult>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess dataAccess;
        
        public GetPreviousQuarterSchemesHandler(IWeeeAuthorization authorization, IGenericDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
        }

        public async Task<PreviousQuarterReturnResult> HandleAsync(GetPreviousQuarterSchemes message)
        {
            authorization.EnsureCanAccessExternalArea();

            IEnumerable<Return> allReturns = await dataAccess.GetAll<Return>();
            var currentReturn = allReturns.First(r => r.Id == message.ReturnId);
            
            if (allReturns.Any(r => r.Quarter == currentReturn.Quarter 
                                    && r.Id != currentReturn.Id 
                                    && r.Organisation.Id == currentReturn.Organisation.Id
                                    && r.FacilityType.Value == currentReturn.FacilityType.Value))
            {
                return new PreviousQuarterReturnResult();
            }

            // Filter the returns by the organisationId. Also ignore the return we are creating. Finally make sure they are submitted.
            // Then order them by the Quarter descending so that we can get the latest returns
            allReturns = allReturns.Where(p => p.Organisation.Id == message.OrganisationId 
                                               && p.Id != message.ReturnId 
                                               && p.ReturnStatus == Domain.AatfReturn.ReturnStatus.Submitted
                                               && p.FacilityType.Value == currentReturn.FacilityType.Value
                                               && p.Quarter.Year <= currentReturn.Quarter.Year)
                .OrderByDescending(p => p.Quarter.Year)
                .ThenByDescending(p => (int)p.Quarter.Q)
                .ThenByDescending(p => p.SubmittedDate).ToList();

            // This is their first return
            if (!allReturns.Any())
            {
                return new PreviousQuarterReturnResult();
            }

            // Find the last quarter submitted
            Quarter lastQ = allReturns.FirstOrDefault().Quarter;
            
            // Grab the last submitted return
            Return lastReturn = allReturns.FirstOrDefault(p => p.Quarter == lastQ);

            if (lastReturn == null)
            {
                return new PreviousQuarterReturnResult();
            }

            IEnumerable<ReturnScheme> returnSchemes = await dataAccess.GetAll<ReturnScheme>();

            if (returnSchemes == null)
            {
                return new PreviousQuarterReturnResult();
            }

            List<ReturnScheme> lastSchemes = new List<ReturnScheme>();

            lastSchemes.AddRange(returnSchemes.Where(p => p.ReturnId == lastReturn.Id));

            PreviousQuarterReturnResult result = new PreviousQuarterReturnResult()
            {
                PreviousQuarter = new Core.DataReturns.Quarter(lastQ.Year, (Core.DataReturns.QuarterType)lastQ.Q),
                PreviousSchemes = lastSchemes.Select(p => p.SchemeId).ToList()
            };

            return result;
        }
    }
}
