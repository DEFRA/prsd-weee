namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Scheme;
    using DataAccess;
    using Domain.Scheme;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.Scheme.MemberRegistration;
    using Security;

    public class GetComplianceYearsHandler : IRequestHandler<GetComplianceYears, List<int>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext context;

        public GetComplianceYearsHandler(IWeeeAuthorization authorization, WeeeContext context)
        {
            this.authorization = authorization;
            this.context = context;
        }

        public async Task<List<int>> HandleAsync(GetComplianceYears request)
        {
            authorization.EnsureInternalOrOrganisationAccess(request.PcsId);

            var organisation = await context.Organisations.FindAsync(request.PcsId);

            if (organisation == null)
            {
                string message = string.Format("An organisation could not be found with ID \"{0}\".", request.PcsId);
                throw new ArgumentException(message);
            }

            return await context.MemberUploads
                .Where(mu => mu.OrganisationId == request.PcsId)
                .Where(mu => mu.IsSubmitted)
                .Where(mu => mu.ComplianceYear.HasValue)
                .GroupBy(mu => (int)mu.ComplianceYear)
                .Select(group => group.Key)
                .OrderBy(year => year)
                .ToListAsync();
        }
    }
}
