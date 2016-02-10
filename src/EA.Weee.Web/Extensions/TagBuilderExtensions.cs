namespace EA.Weee.Web.Extensions
{
    using System.Reflection;
    using System.Web;
    using System.Web.Mvc;
    using Prsd.Core;

    public static class TagBuilderExtensions
    {
        public static void AddHtmlAttributes(this TagBuilder tagBuilder, object htmlAttributes)
        {
            if (htmlAttributes == null)
            {
                return;
            }

            foreach (var prop in htmlAttributes.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var key = HttpUtility.HtmlEncode(prop.Name);
                var value = HttpUtility.HtmlEncode(prop.GetValue(htmlAttributes, null));

                tagBuilder.Attributes.Add(key, value);
            }
        }
    }
}