namespace EA.Weee.XmlValidation.BusinessValidation.QuerySets
{
    using System;
    using System.Linq;
    using DataAccess;
    using Domain.Scheme;

    public class SchemeQuerySet : ISchemeQuerySet
    {
        private readonly WeeeContext context;

        public SchemeQuerySet(WeeeContext context)
        {
            this.context = context;
        }

        public Scheme GetScheme(Guid schemeId)
        {
            return context.Schemes.FirstOrDefault(s => s.OrganisationId == schemeId);
        }
    }
}
