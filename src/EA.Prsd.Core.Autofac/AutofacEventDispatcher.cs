namespace EA.Prsd.Core.Autofac
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using global::Autofac;
    using Domain;

    public class AutofacEventDispatcher : IEventDispatcher
    {
        private readonly IComponentContext context;

        public AutofacEventDispatcher(IComponentContext context)
        {
            this.context = context;
        }

        public async Task Dispatch(IEvent @event)
        {
            var handlerType = typeof(IEventHandler<>).MakeGenericType(@event.GetType());
            var collectionType = typeof(IEnumerable<>).MakeGenericType(handlerType);
            var handlers = ((IEnumerable<object>)context.Resolve(collectionType)).ToList();

            foreach (var handler in handlers)
            {
                var handleMethod = handlerType.GetMethod("HandleAsync");

                var resultTask = (Task)handleMethod.Invoke(handler, new object[] { @event });

                await resultTask;
            }
        }
    }
}