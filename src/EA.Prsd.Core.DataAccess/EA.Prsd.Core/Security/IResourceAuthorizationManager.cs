namespace EA.Prsd.Core.Security
{
    using System.Threading.Tasks;

    public interface IResourceAuthorizationManager
    {
        Task<bool> CheckAccessAsync(ResourceAuthorizationContext context);
    }
}