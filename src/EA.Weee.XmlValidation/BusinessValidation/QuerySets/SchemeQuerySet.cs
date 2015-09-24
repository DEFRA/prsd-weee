namespace EA.Weee.XmlValidation.BusinessValidation.QuerySets
{
    using System;
    using System.Linq;
    using DataAccess;

    public class SchemeQuerySet : ISchemeQuerySet
    {
        private readonly WeeeContext context;

        public SchemeQuerySet(WeeeContext context)
        {
            this.context = context;
        }

        public string GetSchemeApprovalNumber(Guid schemeId)
        {
            return context.Schemes
                .Where(s => s.OrganisationId == schemeId)
                .Select(s => s.ApprovalNumber)
                .SingleOrDefault();
        }
    }
}
