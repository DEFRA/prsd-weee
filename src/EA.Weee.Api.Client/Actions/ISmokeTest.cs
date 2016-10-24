namespace EA.Weee.Api.Client.Actions
{
    using System.Threading.Tasks;

    public interface ISmokeTest
    {
        Task<bool> PerformTest();
    }
}
