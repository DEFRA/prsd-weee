namespace EA.Weee.Web.RazorHelpers
{
    using System;

    public class ProgressiveDisclosure<TModel> : IDisposable
    {
        private readonly WeeeGds<TModel> gdsHelper;

        public ProgressiveDisclosure(WeeeGds<TModel> gdsHelper, string linkText)
        {
            this.gdsHelper = gdsHelper;

            var html = string.Format(
                "<details><summary><span class=\"summary\">{0}</span></summary><div class=\"panel-indent\">", linkText);

            gdsHelper.HtmlHelper.ViewContext.Writer.Write(html);
        }

        public void Dispose()
        {
            gdsHelper.HtmlHelper.ViewContext.Writer.Write("</div></details>");
        }
    }
}