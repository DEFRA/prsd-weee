namespace EA.Weee.Web.RazorHelpers
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;
    using System.Web;

    public static class AttributesHelper
    {
        public static string AttributesHtml(object htmlAttributes)
        {
            if (htmlAttributes == null)
            {
                return string.Empty;
            }

            var attributeBuilder = new StringBuilder();
            foreach (var prop in htmlAttributes.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                attributeBuilder.Append(string.Format(@"{0}=""{1}"" ",
                    HttpUtility.HtmlEncode(prop.Name),
                    HttpUtility.HtmlEncode(prop.GetValue(htmlAttributes, null))));
            }

            return attributeBuilder.ToString();
        }

        public static string AttributesHtml(IDictionary<string, object> htmlAttributes)
        {
            if (htmlAttributes == null)
            {
                return string.Empty;
            }

            var attributeBuilder = new StringBuilder();
            foreach (var attribute in htmlAttributes)
            {
                attributeBuilder.AppendFormat(@"{0}=""{1}"" ",
                    HttpUtility.HtmlEncode(attribute.Key),
                    HttpUtility.HtmlEncode(attribute.Value));
            }

            return attributeBuilder.ToString();
        }
    }
}