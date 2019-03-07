namespace EA.Weee.RequestHandlers.Mappings
{
    using System.Linq;
    using Core.AatfReturn;
    using Domain.AatfReturn;
    using Prsd.Core;
    using Prsd.Core.Mapper;

    public class OperatorMap : IMap<Operator, OperatorData>
    {
        public OperatorData Map(Operator source)
        {
            Guard.ArgumentNotNull(() => source, source);

            return new OperatorData(source.Id, source.Organisation.Name, source.Organisation.Id);
        }
    }
}
