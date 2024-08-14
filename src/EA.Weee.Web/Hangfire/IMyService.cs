namespace EA.Weee.Web.Hangfire
{
    using System.Threading.Tasks;

    public interface IMyService
    {
        Task PerformTask();
    }
}