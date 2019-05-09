namespace EA.Weee.RequestHandlers.Mappings
{
    using Prsd.Core.Mapper;

    public class AatfSizeMap : IMap<Domain.AatfReturn.AatfSize, Core.AatfReturn.AatfSize>
    {
        public Core.AatfReturn.AatfSize Map(Domain.AatfReturn.AatfSize source)
        {
            return new Core.AatfReturn.AatfSize(source.Value, source.DisplayName);
        }
    }
}