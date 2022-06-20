namespace EA.Weee.Web.App_Start
{
    using System;
    using System.Web.Mvc;

    public class UKDateTimeModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var vpr = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (vpr == null)
            {
                return null;
            }

            var date = vpr.AttemptedValue;

            if (string.IsNullOrEmpty(date))
            {
                return null;
            }
            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, bindingContext.ValueProvider.GetValue(bindingContext.ModelName));

            try
            {
                var realDate = DateTime.Parse(date, System.Globalization.CultureInfo.GetCultureInfoByIetfLanguageTag("en-GB"));

                bindingContext.ModelState.SetModelValue(bindingContext.ModelName, new ValueProviderResult(date, realDate.ToString("dd/MM/yyyy"), System.Globalization.CultureInfo.GetCultureInfoByIetfLanguageTag("en-GB")));

                return realDate;
            }
            catch (Exception)
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, string.Format("\"{0}\" is invalid.", bindingContext.ModelName));
                return null;
            }
        }
    }
}
