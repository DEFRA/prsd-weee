namespace EA.Weee.Web.Tests.Integration.RazorHelpers
{
    using System.Web.Mvc;

    public class CustomWebViewPage<T> : WebViewPage<T>
    {
        public override void Execute()
        {
        }
    }
}