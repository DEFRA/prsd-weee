namespace EA.Weee.Web.Infrastructure
{
    using System.IO;
    using System.Web;
    using Prsd.Email;

    public class MvcRazorTemplateLoader : ITemplateLoader
    {
        public string LoadTemplate(string path)
        {
            string template = System.Text.Encoding.UTF8.GetString(System.IO.File.ReadAllBytes(path));

            return template;
        }
    }
}