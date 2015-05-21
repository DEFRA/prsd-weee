namespace EA.Prsd.Core.Mediator
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    ///     Defines a mediator to encapsulate request/response and publishing interaction patterns
    /// </summary>
    public interface IMediator
    {
        /// <summary>
        /// Asynchronously send a request to a single handler
        /// </summary>
        /// <typeparam name="TResponse">Response type</typeparam>
        /// <param name="request">Request object</param>
        /// <returns>A task that represents the send operation. The task result contains the handler response</returns>
        Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request);

        Task<object> SendAsync(object request, Type responseType);
    }
}