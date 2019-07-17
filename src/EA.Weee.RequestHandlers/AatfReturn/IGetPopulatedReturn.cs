namespace EA.Weee.RequestHandlers.AatfReturn
{
    using System;
    using System.Threading.Tasks;
    using Core.AatfReturn;
    using Domain.AatfReturn;

    public interface IGetPopulatedReturn
    {
        Task<ReturnData> GetReturnData(Guid returnId, bool forSummary);
    }
}