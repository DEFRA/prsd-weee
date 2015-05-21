namespace EA.Prsd.Core.Domain
{
    using System.Threading.Tasks;

    public interface IDeferredEventDispatcher
    {
        /// <summary>
        /// Add an event to the dispatcher.
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="e"></param>
        /// <returns></returns>
        Task Dispatch<TEvent>(TEvent e) where TEvent : IEvent;

        /// <summary>
        /// Resolve all events in the dispatcher.
        /// </summary>
        /// <returns></returns>
        Task Resolve();
    }
}