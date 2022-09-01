namespace EA.Weee.Web.Infrastructure
{
    using System.IO;
    using System.Text;
    using System.Web.Mvc;

    public class MvcTemplateExecutor : IMvcTemplateExecutor
    {
        public string RenderRazorView(ControllerContext context, string viewName, object model)
        {
            var viewEngineResult = ViewEngines.Engines.FindView(context, viewName, null).View;
            var sb = new StringBuilder();

            using (TextWriter tr = new StringWriter(sb))
            {
                context.Controller.ViewData.Model = model;
                var viewContext = new ViewContext(context, viewEngineResult, context.Controller.ViewData,
                    context.Controller.TempData, tr);
                viewEngineResult.Render(viewContext, tr);
            }
            return sb.ToString();
        }
    }
}