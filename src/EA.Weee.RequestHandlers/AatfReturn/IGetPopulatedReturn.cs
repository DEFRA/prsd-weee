namespace EA.Weee.RequestHandlers.AatfReturn
{
    using Core.AatfReturn;
    using System;
    using System.Threading.Tasks;

    public interface IGetPopulatedReturn
    {
        Task<ReturnData> GetReturnData(Guid returnId, bool forSummary);
    }
}