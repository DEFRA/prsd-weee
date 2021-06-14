namespace EA.Weee.RequestHandlers.Mappings
{
    using System.Collections.Generic;
    using System.Linq;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Requests.AatfReturn.NonObligated;

    public class NonObligatedObjectMapper : IMapWithParameter<IEnumerable<NonObligatedValue>, Return, IEnumerable<NonObligatedWeee>>
    {
        public IEnumerable<NonObligatedWeee> Map(IEnumerable<NonObligatedValue> categoryValues, Return aatfReturn)
        {
            return categoryValues
                .GroupBy(n => n.CategoryId)
                .Select(g => g.FirstOrDefault())
                .Where(n => n != null)
                .Select(n => new NonObligatedWeee(aatfReturn, n.CategoryId, n.Dcf, n.Tonnage));
        }
    }
}
