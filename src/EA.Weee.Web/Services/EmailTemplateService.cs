namespace EA.Weee.Web.Services
{
    using System;
    using System.IO;
    using RazorEngine;
    using RazorEngine.Templating;

    public class EmailTemplateService : IEmailTemplateService
    {
        private static readonly string HtmlTemplateExtension = ".cshtml";
        private static readonly string PlainTextTemplateExtension = ".txt";

        private static string GetEmailTemplateFileDirectory()
        {
            return Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Views"), "Emails");
        }

        private static string GetEmailTemplateAsString(string templateName, string extension)
        {
            return File.ReadAllText(Path.Combine(GetEmailTemplateFileDirectory(), templateName + extension));
        }

        public EmailTemplate TemplateWithDynamicModel(string templateName, object model)
        {
            string htmlTemplateKey = templateName;
            string plainTextTemplateKey = templateName + "PlainText";

            string htmlResult = RunTemplateWithDynamicModel(htmlTemplateKey, templateName, model, HtmlTemplateExtension);
            string plainTextResult = RunTemplateWithDynamicModel(plainTextTemplateKey, templateName, model, PlainTextTemplateExtension);
            
            return new EmailTemplate { HtmlText = htmlResult, PlainText = plainTextResult };
        }

        private static string RunTemplateWithDynamicModel(string templateKey, string templateName, object model, string fileExtension)
        {
            bool isTemplated = Engine.Razor.IsTemplateCached(templateKey, null);
            string result = null;

            if (isTemplated)
            {
                result = Engine.Razor.Run(templateKey, null, model);
            }
            else
            {
                result = Engine.Razor.RunCompile(GetEmailTemplateAsString(templateName, fileExtension), templateKey,
                    null, model);
            }

            return result;
        }
    }
}