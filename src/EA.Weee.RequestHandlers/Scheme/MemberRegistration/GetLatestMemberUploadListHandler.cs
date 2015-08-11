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

    public class GetLatestMemberUploadListHandler : IRequestHandler<GetLatestMemberUploadList, LatestMemberUploadList>
    {
        private readonly WeeeContext context;
        private readonly IMap<IEnumerable<MemberUpload>, LatestMemberUploadList> mapper;

        public GetLatestMemberUploadListHandler(WeeeContext context, IMap<IEnumerable<MemberUpload>, LatestMemberUploadList> mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<LatestMemberUploadList> HandleAsync(GetLatestMemberUploadList message)
        {
            var latestMemberUploads = await context.MemberUploads
                .Where(mu => mu.OrganisationId == message.PcsId && mu.IsSubmitted)
                .ToListAsync();

            // Filter to latest uploads in each compliance year
            latestMemberUploads = latestMemberUploads
                .GroupBy(mu => mu.ComplianceYear)
                .Select(g => g.OrderByDescending(mu => BitConverter.ToInt64(mu.RowVersion, 0)).FirstOrDefault())
                .Where(mu => mu != null)
                .ToList();

            return mapper.Map(latestMemberUploads);
        }
    }
}
