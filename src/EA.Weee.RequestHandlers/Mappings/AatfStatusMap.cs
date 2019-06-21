namespace EA.Weee.RequestHandlers.Mappings
{
    using Prsd.Core.Mapper;

    public class AatfStatusMap : IMap<Domain.AatfReturn.AatfStatus, Core.AatfReturn.AatfStatus>
    {
        public Core.AatfReturn.AatfStatus Map(Domain.AatfReturn.AatfStatus source)
        {
            return new Core.AatfReturn.AatfStatus(source.Value, source.DisplayName);
        }
    }
}