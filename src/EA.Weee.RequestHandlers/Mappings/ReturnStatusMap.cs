namespace EA.Weee.RequestHandlers.Mappings
{
    using Domain.AatfReturn;
    using Prsd.Core.Mapper;

    public class ReturnStatusMap : IMap<ReturnStatus, Core.AatfReturn.ReturnStatus>
    {
        public Core.AatfReturn.ReturnStatus Map(ReturnStatus source)
        {
            if (source == ReturnStatus.Submitted)
            {
                return Core.AatfReturn.ReturnStatus.Submitted;
            }

            return Core.AatfReturn.ReturnStatus.Created;
        }
    }
}
