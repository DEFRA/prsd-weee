namespace EA.Weee.RequestHandlers.Organisations
{
    using System;
    using System.Threading.Tasks;
    using AatfReturn;
    using Core.Organisations;
    using Core.Shared;
    using DataAccess;
    using Domain.Organisation;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.Organisations;
    using Security;

    public class GetContactHandler : IRequestHandler<GetContact, ContactData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess dataAccess;
        private readonly IMap<Contact, ContactData> mapper;
        private readonly WeeeContext context;
        
        public GetContactHandler(IWeeeAuthorization authorization, WeeeContext context, IGenericDataAccess dataAccess, IMap<Contact, ContactData> mapper)
        {
            this.authorization = authorization;
            this.context = context;
            this.dataAccess = dataAccess;
            this.mapper = mapper;
        }

        public async Task<ContactData> HandleAsync(GetContact message)
        {
            authorization.EnsureOrganisationAccess(message.OrganisationId);

            var contact = await dataAccess.GetById<Contact>(message.ContactId);

            if (contact == null)
            {
                throw new ArgumentException($"Could not find a contact with Id {message.ContactId}");
            }

            return mapper.Map(contact);
        }
    }
}
