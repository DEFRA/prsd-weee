namespace EA.Prsd.Core.Web.Mvc.Tests.Helpers
{
    using System.IO;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using FakeItEasy;

    public class HtmlHelperFactory
    {
        public static HtmlHelper<T> CreateHtmlHelper<T>() where T : new()
        {
            var viewDataDictionary = new ViewDataDictionary(new T());
            
            var controllerContext = new ControllerContext(A.Fake<HttpContextBase>(), 
                new RouteData(), 
                A.Fake<ControllerBase>());
            
            var viewContext = new ViewContext(controllerContext, A.Fake<IView>(), viewDataDictionary, new TempDataDictionary(), A.Fake<TextWriter>());

            var viewDataContainer = A.Fake<IViewDataContainer>();

            A.CallTo(() => viewDataContainer.ViewData).Returns(viewDataDictionary);

            return new HtmlHelper<T>(viewContext, viewDataContainer);
        } 
    }
}
