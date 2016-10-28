namespace EA.Weee.Api.Client
{
    using System;
    using System.Threading.Tasks;
    using Actions;
    using Prsd.Core.Mediator;

    public interface IWeeeClient : IDisposable
    {
        IUnauthenticatedUser User { get; }

        ISmokeTest SmokeTest { get; }

        Task<TResult> SendAsync<TResult>(IRequest<TResult> request);
        Task<TResult> SendAsync<TResult>(string accessToken, IRequest<TResult> request);
    }
}