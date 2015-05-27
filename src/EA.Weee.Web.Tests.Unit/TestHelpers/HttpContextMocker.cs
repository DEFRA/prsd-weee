namespace EA.Weee.Web.Tests.Unit.TestHelpers
{
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using FakeItEasy;

    internal class HttpContextMocker
    {
        public HttpContextBase HttpContextBase { get; set; }
        public HttpRequestBase HttpRequestBase { get; set; }
        public HttpResponseBase HttpResponseBase { get; set; }
        public HttpSessionStateBase HttpSessionStateBase { get; set; }

        public RouteData RouteData { get; set; }

        public HttpContextMocker()
        {
            HttpContextBase = A.Fake<HttpContextBase>();
        }

        public virtual void AttachToController(Controller controller)
        {
            UseControllerDefaultsForMocksIfNull(controller);

            A.CallTo(() => HttpContextBase.Request).Returns(HttpRequestBase);
            A.CallTo(() => HttpContextBase.Response).Returns(HttpResponseBase);
            A.CallTo(() => HttpContextBase.Session).Returns(HttpSessionStateBase);

            RequestContext rc = new RequestContext(HttpContextBase, RouteData);

            controller.ControllerContext = new ControllerContext(rc, controller);
        }

        protected virtual void UseControllerDefaultsForMocksIfNull(Controller controller)
        {
            if (HttpRequestBase == null)
            {
                HttpRequestBase = controller.Request;
            }

            if (HttpResponseBase == null)
            {
                HttpResponseBase = controller.Response;
            }

            if (HttpSessionStateBase == null)
            {
                if (controller.Session == null)
                {
                    HttpSessionStateBase sessionStateBase = A.Fake<HttpSessionStateBase>();

                    A.CallTo(() => sessionStateBase[A<string>.Ignored]).Returns(null);

                    HttpSessionStateBase = sessionStateBase;
                }
                else
                {
                    HttpSessionStateBase = controller.Session;
                }
            }

            if (RouteData == null)
            {
                RouteData = controller.RouteData ?? new RouteData();
            }
        }

        public virtual void AddMockSession(Dictionary<string, object> sessionObjects)
        {
            var mockSession = A.Fake<HttpSessionStateBase>();

            foreach (var sessionObject in sessionObjects)
            {
                // Required for proper treatment in the closure.
                var key = sessionObject.Key;
                var value = sessionObject.Value;

                A.CallTo(() => mockSession[key]).Returns(value);
            }

            HttpSessionStateBase = mockSession;
        }
    }
}
