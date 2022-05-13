namespace EA.Weee.RequestHandlers.AatfReturn.Specification
{
    using EA.Weee.Domain.AatfReturn;
    using System;
    using System.Linq.Expressions;
    using DataAccess.Specification;

    public class WeeeReusedByAddressIdSpecification : Specification<WeeeReusedSite>
    {
        public Guid SiteId { get; private set; }

        public WeeeReusedByAddressIdSpecification(Guid siteId)
        {
            SiteId = siteId;
        }

        public override Expression<Func<WeeeReusedSite, bool>> ToExpression()
        {
            return @weeeReusedSite => @weeeReusedSite.Address.Id == SiteId;
        }
    }
}
