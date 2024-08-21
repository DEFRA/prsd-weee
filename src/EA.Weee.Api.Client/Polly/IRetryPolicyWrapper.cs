namespace EA.Weee.Api.Client
{
    using System;
    using System.Threading.Tasks;

    public interface IRetryPolicyWrapper
    {
        Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> action);
    }
}
