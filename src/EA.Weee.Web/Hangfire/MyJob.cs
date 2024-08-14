namespace EA.Weee.Web.Hangfire
{
    using System.Threading.Tasks;

    public class MyJob
    {
        private readonly IMyService myService;

        public MyJob(IMyService myService)
        {
            this.myService = myService;
        }

        public async Task Execute()
        {
            await myService.PerformTask();
        }
    }
}