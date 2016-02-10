namespace EA.Weee.Security
{
    using System.Threading.Tasks;

    public interface IRoleRequestHandler
    {
        Task<object> HandleAsync(object response);
    }
}
