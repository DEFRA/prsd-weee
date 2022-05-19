namespace EA.Weee.Web.Areas.Aatf.Attributes
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Api.Client;
    using Autofac.Integration.Mvc;
    using Services;

    public class EvidenceDateValidationBase : ValidationAttribute
    {
        private Func<IWeeeClient> client;
        public Func<IWeeeClient> Client
        {
            get
            {
                if (client == null)
                {
                    return () => DependencyResolver.Current.GetService<IWeeeClient>();
                }

                return client;
            }
                
            set => client = value;
        }

        private IHttpContextService httpContextService;
        public IHttpContextService HttpContextService
        {
            get
            {
                if (httpContextService == null)
                {
                    var resolver = DependencyResolver.Current.GetService<AutofacDependencyResolver>();
                    return resolver.GetService<IHttpContextService>();
                }

                return httpContextService;
            }

            set => httpContextService = value;
        }
    }
}