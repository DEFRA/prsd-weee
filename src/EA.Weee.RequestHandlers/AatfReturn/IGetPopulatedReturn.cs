namespace EA.Weee.RequestHandlers.AatfReturn
{
    using System;
    using System.Threading.Tasks;
    using Core.AatfReturn;

    public interface IGetPopulatedReturn
    {
        Task<ReturnData> GetReturnData(Guid returnId);
    }
}