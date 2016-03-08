namespace EA.Weee.Web.Logging
{
    using Elmah;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public static class ElmahFilter
    {
        private static string[] sensitiveFieldNames = { "Password", "ConfirmPassword" };

        /// <summary>
        /// This method checks the HttpContext associated with the Exception for any sensitive
        /// data submitted in the form that should not be logged in plain text.
        /// If fields are found that match the names in the SensitiveFieldNames list, their values
        /// are replaced with **** before the error is logged.
        /// </summary>
        /// <param name="args"></param>
        internal static void FilterSensitiveData(ExceptionFilterEventArgs args)
        {
            if (args.Context != null)
            {
                HttpContext context = (HttpContext)args.Context;
                if (context.Request != null &&
                    context.Request.Form != null &&
                    context.Request.Form.AllKeys.Intersect(sensitiveFieldNames).Any())
                    {
                        ReplaceSensitiveFormFields(args, context);
                    }
            }
        }

        private static void ReplaceSensitiveFormFields(ExceptionFilterEventArgs args, HttpContext context)
        {
            var replacementError = new Error(args.Exception, (HttpContext)args.Context);

            foreach (var formField in context.Request.Form.AllKeys)
            {
                if (sensitiveFieldNames.Contains(formField))
                {
                    replacementError.Form.Set(formField, "****");
                }
            }

            ErrorLog.GetDefault(HttpContext.Current).Log(replacementError);
            args.Dismiss();
        }
    }
}