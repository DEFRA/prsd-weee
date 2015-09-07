namespace EA.Weee.Api.Services
{
    using System.Threading.Tasks;
    using Prsd.Core.Domain;

    internal class NullEventDispatcher : IEventDispatcher
    {
        public Task Dispatch(IEvent @event)
        {
            return Task.FromResult(0);
        }
    }
}