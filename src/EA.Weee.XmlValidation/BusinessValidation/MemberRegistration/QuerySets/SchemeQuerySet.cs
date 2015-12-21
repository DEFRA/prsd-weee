namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.QuerySets
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

        public string GetSchemeApprovalNumberByOrganisationId(Guid organisationId)
        {
            return context.Schemes
                .Where(s => s.OrganisationId == organisationId)
                .Select(s => s.ApprovalNumber)
                .SingleOrDefault();
        }
    }
}
