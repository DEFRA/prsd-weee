namespace EA.Prsd.Core.Security
{
    using System.Linq;
    using System.Threading.Tasks;

    public class ResourceAuthorizationManager : IResourceAuthorizationManager
    {
        public Task<bool> CheckAccessAsync(ResourceAuthorizationContext context)
        {
            // Resource is what is being accessed - e.g. Notification, Exporter
            // Action is what is being performed - e.g. Add, Update, Delete
            // Can switch on resource / action and perform different checks
            // Can inject DbContext to look at data other than claims
            return Eval(context.Principal.HasClaim(p => p.Type == context.Action.First().Value && p.Value == context.Resource.First().Value));
        }

        private Task<bool> Ok()
        {
            return Task.FromResult(true);
        }

        private Task<bool> Nok()
        {
            return Task.FromResult(false);
        }

        private Task<bool> Eval(bool val)
        {
            return Task.FromResult(val);
        }
    }
}