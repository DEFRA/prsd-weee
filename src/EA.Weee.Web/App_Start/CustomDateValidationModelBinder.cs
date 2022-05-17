namespace EA.Weee.Web
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Web.Mvc;
    using Filters;

    public class CustomDateValidationModelBinder : DefaultModelBinder
    {
        public override object BindModel
            (ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var displayFormat = bindingContext.ModelMetadata.DisplayFormatString;
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            var t = bindingContext.ModelMetadata.Container.GetType().GetProperty(bindingContext.ModelName);
            var t2 = t.GetCustomAttributes(typeof(WeeeDateFormatAttribute));
            if (t != null && t.GetCustomAttributes(typeof(WeeeDateFormatAttribute)) != null)
            {
                if (!string.IsNullOrEmpty(displayFormat) && value != null)
                {
                    DateTime date;
                    displayFormat = displayFormat.Replace
                        ("{0:", string.Empty).Replace("}", string.Empty);
                    if (DateTime.TryParseExact(value.AttemptedValue, displayFormat,
                            CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                    {
                        return date;
                    }
                    else
                    {
                        bindingContext.ModelState.AddModelError(bindingContext.ModelName, string.Format("{0} is an invalid date format", value.AttemptedValue));
                    }
                }
            }
            
            return base.BindModel(controllerContext, bindingContext);
        }
    }
}