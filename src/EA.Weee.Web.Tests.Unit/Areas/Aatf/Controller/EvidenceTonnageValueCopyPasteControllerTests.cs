namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Controller
{
    using EA.Weee.Core.Shared;
    using EA.Weee.Web.Areas.Aatf.Controllers;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using System.Threading.Tasks;
    using Xunit;

    public class EvidenceTonnageValueCopyPasteControllerTests
    {
        private readonly EvidenceTonnageValueCopyPasteController evidenceTonnageValueCopyPasteController;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly ISessionService sessionService;
        private readonly IPasteProcessor pasteProcessor;

        public EvidenceTonnageValueCopyPasteControllerTests()
        {
            breadcrumb = A.Fake<BreadcrumbService>();
            cache = A.Fake<IWeeeCache>();
            sessionService = A.Fake<ISessionService>();
            pasteProcessor = A.Fake<IPasteProcessor>();

            evidenceTonnageValueCopyPasteController = new EvidenceTonnageValueCopyPasteController(breadcrumb, cache, sessionService, pasteProcessor);
        }

        //[Fact]
        //public void IndexGet_IsDecoratedWith_HttpGetAttribute()
        //{

        //}

        //[Fact]
        //public void IndexPost_IsDecoratedWith_HttpPostAttribute()
        //{

        //}

        //[Fact]
        //public void IndexPost_IsDecoratedWith_AntiForgeryAttribute()
        //{

        //}

        //[Fact]
        //public async Task IndexGet_Calls_SetBreadcrumb()
        //{


        //}

        //[Fact]
        //public async Task IndexGet_RetrievesEvidenceModelSessionObject()
        //{

        //}

        //[Fact]
        //public async Task IndexGet_OnRedirectIsFalse_CreatesModelAndReturnsView()
        //{

        //}

        //[Theory]
        //public async Task IndexGet_OnRedirectIsTrue_RedirectsToCorrectPage()
        //{

        //}

        //[Theory]
        //public void IndexPost_OnRedirectIsTrue_RedirectsToCorrectPage()
        //{

        //}

        //[Fact]
        //public void IndexPost_OnNoGivenCopyPasteValues_DoesNotPopulateSession()
        //{

        //}

        //[Fact]
        //public void IndexPost_OnGivenCopyPasteValues_DoesPopulateSession()
        //{

        //}

        //[Fact]
        //public void IndexPost_OnGivenCopyPasteValues_DoesCallPasteProcessor()
        //{

        //}

        //[Fact]
        //public void IndexPost_Retrieves_EvidenceModelSessionOject()
        //{

        //}
    }
}
