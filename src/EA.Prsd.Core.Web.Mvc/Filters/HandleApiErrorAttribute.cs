namespace EA.Prsd.Core.Web.Mvc.Filters
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using ApiClient;

    public class HandleApiErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            var apiException = GetApiException(filterContext);

            if (!filterContext.ExceptionHandled && apiException != null)
            {
                filterContext.ExceptionHandled = true;
                throw new HttpException((int)apiException.StatusCode, apiException.ErrorData.ExceptionMessage,
                    apiException);
            }

            base.OnException(filterContext);
        }

        private static ApiException GetApiException(ExceptionContext filterContext)
        {
            var apiException = filterContext.Exception as ApiException;

            if (apiException == null)
            {
                var aggregateException = filterContext.Exception as AggregateException;
                if (aggregateException != null)
                {
                    apiException = aggregateException.InnerException as ApiException;
                }
            }
            return apiException;
        }
    }
}