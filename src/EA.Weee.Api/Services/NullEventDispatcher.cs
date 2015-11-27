namespace EA.Weee.Api.Services
{
    using Prsd.Core.Domain;
    using System.Threading.Tasks;

    internal class NullEventDispatcher : IEventDispatcher
    {
        public Task Dispatch(IEvent @event)
        {
            return Task.FromResult(0);
        }
    }
}