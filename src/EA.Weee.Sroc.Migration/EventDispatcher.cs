namespace EA.Weee.Sroc.Migration
{
    using System;
    using System.Threading.Tasks;
    using Prsd.Core.Domain;

    public class EventDispatcher : IEventDispatcher
    {
        public Task Dispatch(IEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}
