﻿namespace EA.Weee.RequestHandlers.Organisations.DirectRegistrants
{
    using DataAccess;
    using Domain.Producer;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using Mappings;
    using Security;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    internal class 
        ServiceOfNoticeRequestHandler : IRequestHandler<ServiceOfNoticeRequest, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly WeeeContext weeeContext;
        private readonly ISystemDataDataAccess systemDataDataAccess;

        public ServiceOfNoticeRequestHandler(IWeeeAuthorization authorization,
            IGenericDataAccess genericDataAccess, WeeeContext weeeContext, ISystemDataDataAccess systemDataDataAccess)
        {
            this.authorization = authorization;
            this.genericDataAccess = genericDataAccess;
            this.weeeContext = weeeContext;
            this.systemDataDataAccess = systemDataDataAccess;
        }

        public async Task<bool> HandleAsync(ServiceOfNoticeRequest request)
        {
            authorization.EnsureCanAccessExternalArea();

            var directRegistrant = await genericDataAccess.GetById<DirectRegistrant>(request.DirectRegistrantId);

            authorization.EnsureOrganisationAccess(directRegistrant.OrganisationId);

            var systemDate = await systemDataDataAccess.GetSystemDateTime();

            var currentYearSubmission = directRegistrant.DirectProducerSubmissions.First(r => r.ComplianceYear == systemDate.Year);
            
            var country = await weeeContext.Countries.SingleAsync(c => c.Id == request.Address.CountryId);

            request.Address.CountryName = country.Name;

            var address = ValueObjectInitializer.CreateAddress(request.Address, country);

            await genericDataAccess.Add(address);

            currentYearSubmission.CurrentSubmission.AddOrUpdateServiceOfNotice(address);

            await weeeContext.SaveChangesAsync();

            return true;
        }
    }
}
