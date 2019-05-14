namespace EA.Weee.RequestHandlers.Mappings
{
    using Core.Shared;
    using Domain.AatfReturn;
    using Prsd.Core.Mapper;
    using SchemeStatus = Domain.Scheme.SchemeStatus;

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
