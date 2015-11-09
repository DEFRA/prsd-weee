namespace EA.Weee.RequestHandlers.Shared
{
    using System;
    using System.Threading.Tasks;
    using Core.Shared;
    using DataAccess;
    using Domain;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.Shared;

    internal class GetUKCompetentAuthorityByIdHandler :
        IRequestHandler<GetUKCompetentAuthorityById, UKCompetentAuthorityData>
    {
        private readonly WeeeContext context;
        private readonly IMap<UKCompetentAuthority, UKCompetentAuthorityData> mapper;

        public GetUKCompetentAuthorityByIdHandler(WeeeContext context, IMap<UKCompetentAuthority, UKCompetentAuthorityData> mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<UKCompetentAuthorityData> HandleAsync(GetUKCompetentAuthorityById request)
        {
            var ukCompetentAuthority =
                await context.UKCompetentAuthorities.FindAsync(request.Id);

            if (ukCompetentAuthority == null)
            {
                string message = string.Format("No authorised authority was found with id \"{0}\".", request.Id);
                throw new ArgumentException(message);
            }
            return mapper.Map(ukCompetentAuthority);
        }
    }
}