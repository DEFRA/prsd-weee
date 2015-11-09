namespace EA.Weee.Web.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    public class WeeeRequiredAttributeAdapter : RequiredAttributeAdapter
    {
        public WeeeRequiredAttributeAdapter(ModelMetadata metadata, ControllerContext context, RequiredAttribute attribute)
        : base(metadata, context, attribute)
        {
            if (string.IsNullOrEmpty(attribute.ErrorMessage) &&
                !string.IsNullOrEmpty(metadata.DisplayName))
            {
                string lower = metadata.DisplayName.ToLower();

                attribute.ErrorMessage = string.Format("Enter {0} {1}.", IsVowel(lower[0]) ? "an" : "a", lower);
            }
        }
        private static bool IsVowel(char character)
        {
            return new[] { 'a', 'e', 'i', 'o', 'u' }.Contains(char.ToLower(character));
        }
    }
}