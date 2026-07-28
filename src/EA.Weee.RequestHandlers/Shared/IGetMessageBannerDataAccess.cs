namespace EA.Weee.RequestHandlers.Shared
{
    using EA.Weee.Domain.Lookup;
    using System.Threading.Tasks;

    public interface IGetMessageBannerDataAccess
    {
        Task<MessageBanner> GetMessageBannerData();
    }
}
