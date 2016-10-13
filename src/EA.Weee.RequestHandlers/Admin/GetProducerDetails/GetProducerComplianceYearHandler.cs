namespace EA.Weee.RequestHandlers.Admin.GetProducerDetails
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Requests.Admin;
    using Security;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class GetProducerComplianceYearHandler : IRequestHandler<GetProducerComplianceYear, List<int>>
    {
        private readonly IGetProducerComplianceYearDataAccess dataAccess;
        private readonly IWeeeAuthorization authorization;

        public GetProducerComplianceYearHandler(IGetProducerComplianceYearDataAccess dataAccess, IWeeeAuthorization authorization)
        {
            this.dataAccess = dataAccess;
            this.authorization = authorization;
        }

        public async Task<List<int>> HandleAsync(GetProducerComplianceYear request)
        {
            authorization.EnsureCanAccessInternalArea();

            var complianceYears = await dataAccess.GetComplianceYears(request.RegistrationNumber);

            if (complianceYears.Count == 0)
            {
                string message = string.Format(
                    "No producer has been registered with the registration number \"{0}\".",
                    request.RegistrationNumber);

                throw new ArgumentException(message);
            }

            return complianceYears;
        }
    }
}