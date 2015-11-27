namespace EA.Weee.Api.Client
{
    using Actions;
    using Prsd.Core.Mediator;
    using System;
    using System.Threading.Tasks;

    public interface IWeeeClient : IDisposable
    {
        IUnauthenticatedUser User { get; }

        Task<TResult> SendAsync<TResult>(IRequest<TResult> request);
        Task<TResult> SendAsync<TResult>(string accessToken, IRequest<TResult> request);
    }
}