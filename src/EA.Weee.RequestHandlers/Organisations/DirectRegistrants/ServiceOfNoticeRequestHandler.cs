namespace EA.Weee.RequestHandlers.Organisations.DirectRegistrants
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
        ServiceOfNoticeRequestHandler : SubmissionRequestHandlerBase, IRequestHandler<ServiceOfNoticeRequest, bool>
    {
        private readonly IGenericDataAccess genericDataAccess;
        private readonly WeeeContext weeeContext;

        public ServiceOfNoticeRequestHandler(IWeeeAuthorization authorization,
            IGenericDataAccess genericDataAccess, WeeeContext weeeContext, ISystemDataDataAccess systemDataDataAccess, ISmallProducerDataAccess smallProducerDataAccess) : base(authorization, genericDataAccess, systemDataDataAccess, smallProducerDataAccess)
        {
            this.genericDataAccess = genericDataAccess;
            this.weeeContext = weeeContext;
        }

        public async Task<bool> HandleAsync(ServiceOfNoticeRequest request)
        {
            var currentYearSubmission = await Get(request.DirectRegistrantId);

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
