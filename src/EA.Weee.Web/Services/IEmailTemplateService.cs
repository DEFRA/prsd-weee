namespace EA.Weee.Web.Services
{
    public interface IEmailTemplateService
    {
        EmailTemplate TemplateWithDynamicModel(string templateName, object model);
    }
}