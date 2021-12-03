namespace EA.Weee.RequestHandlers.Mappings
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Requests.AatfReturn.NonObligated;

    public class NonObligatedWeeeMap :
        IMapWithParameter<EditNonObligated, Return, IEnumerable<Tuple<Guid, decimal?>>>,
        IMapWithParameter<AddNonObligated, Return, IEnumerable<NonObligatedWeee>>
    {
        public IEnumerable<Tuple<Guid, decimal?>> Map(EditNonObligated editMessage, Return aatfReturn)
        {
            return editMessage.CategoryValues.Select(v => new Tuple<Guid, decimal?>(v.Id, v.Tonnage));
        }

        public IEnumerable<NonObligatedWeee> Map(AddNonObligated addMessage, Return aatfReturn)
        {
            return GetFilteredValues(addMessage.CategoryValues)
                .Select(n => new NonObligatedWeee(aatfReturn, n.CategoryId, addMessage.Dcf, n.Tonnage));
        }

        public IEnumerable<NonObligatedValue> GetFilteredValues(IList<NonObligatedValue> rawValues)
        {
            return rawValues
                .GroupBy(n => n.CategoryId)
                .Select(g => g.FirstOrDefault())
                .Where(n => n != null);
        }
    }
}
