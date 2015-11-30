namespace EA.Weee.Web.Infrastructure
{
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class WeeeRequiredAttributeAdapter : RequiredAttributeAdapter
    {
        public WeeeRequiredAttributeAdapter(ModelMetadata metadata, ControllerContext context, RequiredAttribute attribute)
        : base(metadata, context, attribute)
        {
            if (string.IsNullOrEmpty(attribute.ErrorMessage) &&
                !string.IsNullOrEmpty(metadata.DisplayName))
            {
                attribute.ErrorMessage = string.Format("Enter {0}", metadata.DisplayName.ToLower());
            }
        }
    }
}