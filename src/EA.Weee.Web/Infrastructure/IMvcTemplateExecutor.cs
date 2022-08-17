namespace EA.Weee.Web.Infrastructure
{
    using System.Web.Mvc;

    public interface IMvcTemplateExecutor
    {
        string RenderRazorView(ControllerContext context, string viewName, object model);
    }
}