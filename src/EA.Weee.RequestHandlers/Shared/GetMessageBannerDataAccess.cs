namespace EA.Weee.RequestHandlers.Shared
{
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.Lookup;
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    public class GetMessageBannerDataAccess : IGetMessageBannerDataAccess
    {
        private readonly WeeeContext context;

        public GetMessageBannerDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<MessageBanner> GetMessageBannerData()
        {
            var currentDate = DateTime.Now;

            return await context.MessageBanners
                                .Where(x => x.StartTime <= currentDate && x.EndTime >= currentDate)
                                .SingleOrDefaultAsync();
        }
    }
}
