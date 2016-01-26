namespace EA.Weee.Web.Extensions
{
    using System.ComponentModel.DataAnnotations;

    public static class DisplayExtensions
    {
        public static string ToDisplayString<T>(this T value)
        {
            var fieldInfo = typeof(T).GetField(value.ToString());

            var descriptionAttributes = fieldInfo.GetCustomAttributes(
                typeof(DisplayAttribute), false) as DisplayAttribute[];

            if (descriptionAttributes == null)
            {
                return string.Empty;
            }

            return descriptionAttributes.Length > 0 
                ? descriptionAttributes[0].Name 
                : value.ToString();
        }
    }
}