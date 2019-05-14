namespace EA.Weee.RequestHandlers.AatfReturn.Specification
{
    using System;
    using System.Linq.Expressions;
    using Domain.AatfReturn;

    public class WeeeReusedSiteByWeeeReusedIdSpecification : Specification<WeeeReusedSite>
    {
        public Guid WeeeReusedId { get; private set; }

        public WeeeReusedSiteByWeeeReusedIdSpecification(Guid weeeReusedId)
        {
            WeeeReusedId = weeeReusedId;
        }

        public override Expression<Func<WeeeReusedSite, bool>> ToExpression()
        {
            return @weeeReused => @weeeReused.WeeeReused.Id == WeeeReusedId;
        }
    }
}