namespace EA.Weee.Security
{
    using System.Threading.Tasks;

    public interface IRoleBasedResponseHandler
    {
        Task<object> HandleAsync(object response);
    }
}
