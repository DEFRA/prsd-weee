//namespace EA.Weee.RequestHandlers.Organisations
//{
//    using System.Data.Entity;
//    using System.Threading.Tasks;
//    using Core.Organisations;
//    using DataAccess;
//    using Domain.Organisation; //CHECK
//    using Domain.Scheme;
//    using Prsd.Core.Mapper;
//    using Prsd.Core.Mediator;
//    using Requests.Organisations;
//    using Security;

//    internal class GetContactPersonByOrganisationIdHandler : IRequestHandler<GetContactPersonByOrganisationId, ContactData>
//    {
//        private readonly IWeeeAuthorization authorization;
//        private readonly WeeeContext context;
//        private readonly IMap<Scheme, ContactData> mapper;

//        public GetContactPersonByOrganisationIdHandler(IWeeeAuthorization authorization, WeeeContext context, IMap<Scheme, ContactData> mapper)
//        {
//            this.authorization = authorization;
//            this.context = context;
//            this.mapper = mapper;
//        }

//        public async Task<ContactData> HandleAsync(GetContactPersonByOrganisationId message)
//        {
//            authorization.EnsureOrganisationAccess(message.OrganisationsId);

//            var scheme = await context.Schemes.SingleOrDefaultAsync(n => n.Id == message.OrganisationsId);

//            if (scheme != null)
//            {
//                return mapper.Map(scheme);
//            }
            
//            return new ContactData() { OrganisationId = message.OrganisationsId };
//        }
//    }
//}
