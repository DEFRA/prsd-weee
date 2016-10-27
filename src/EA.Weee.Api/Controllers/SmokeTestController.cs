namespace EA.Weee.Api.Controllers
{
    using System.Threading.Tasks;
    using System.Web.Http;
    using DataAccess;

    [RoutePrefix("api/SmokeTest")]
    public class SmokeTestController : ApiController
    {
        private readonly WeeeContext context;

        public SmokeTestController(WeeeContext context)
        {
            this.context = context;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("PerformTest")]
        public Task<bool> PerformTest()
        {
            return Task.FromResult(context.Database.Exists());
        }
    }
}