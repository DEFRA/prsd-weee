namespace EA.Prsd.Email
{
    using RazorEngine;
    using RazorEngine.Templating;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Uses the Razor templating engine to merge an email template with content.
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
            bool isTemplated = Engine.Razor.IsTemplateCached(name, null);
            string result = null;

            if (isTemplated)
            {
                result = Engine.Razor.Run(name, null, model);
            }
            else
            {
                string template = templateLoader.LoadTemplate(name);
                result = Engine.Razor.RunCompile(template, name, null, model);
            }

            return result;
        }
    }
}
