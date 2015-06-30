namespace EA.Weee.RequestHandlers.MemberRegistration
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess;
    using EA.Weee.Requests.MemberRegistration;

    internal class GetMemberUploadDataHandler : IRequestHandler<GetMemberUploadData, List<KeyValuePair<string, string>>>
    {
        private readonly WeeeContext context;

        public GetMemberUploadDataHandler(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<List<KeyValuePair<string, string>>> HandleAsync(GetMemberUploadData message)
        {
            var memberUpload = await context.MemberUploads.FirstOrDefaultAsync(m => m.Id == message.MemberUploadId);

            return
                memberUpload.Errors.Select(
                    e => new KeyValuePair<string, string>(e.ErrorLevel.DisplayName, e.Description)).ToList();
        }
    }
}
