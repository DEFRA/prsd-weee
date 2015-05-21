namespace EA.Prsd.Core.Domain
{
    using System.Threading.Tasks;

    public static class DomainEvents
    {
        static DomainEvents()
        {
            Dispatcher = new EmptyDispatcher();
        }

        public static IDeferredEventDispatcher Dispatcher { get; set; }

        public static void Raise<TEvent>(TEvent e) where TEvent : IEvent
        {
            if (e != null)
            {
                Dispatcher.Dispatch(e);
            }
        }

        private class EmptyDispatcher : IDeferredEventDispatcher
        {
            public Task Dispatch<TEvent>(TEvent e) where TEvent : IEvent
            {
                // Do nothing
                return Task.FromResult(0);
            }

            public Task Resolve()
            {
                // Do nothing
                return Task.FromResult(0);
            }
        }
    }
}