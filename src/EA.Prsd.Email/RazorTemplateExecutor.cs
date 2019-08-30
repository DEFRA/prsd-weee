namespace EA.Prsd.Email
{
    using RazorEngine;
    using RazorEngine.Templating;

    /// <summary>
    ///     Uses the Razor templating engine to merge an email template with content.
    /// </summary>
    public class RazorTemplateExecutor : ITemplateExecutor
    {
        private readonly ITemplateLoader templateLoader;

        public RazorTemplateExecutor(ITemplateLoader templateLoader)
        {
            this.templateLoader = templateLoader;
        }

        public string Execute(string name, object model)
        {
            var isTemplated = Engine.Razor.IsTemplateCached(name, null);
            string result = null;

            if (isTemplated)
            {
                result = Engine.Razor.Run(name, null, model);
            }
            else
            {
                var template = templateLoader.LoadTemplate(name);
                result = Engine.Razor.RunCompile(template, name, null, model);
            }

            return result;
        }
    }
}