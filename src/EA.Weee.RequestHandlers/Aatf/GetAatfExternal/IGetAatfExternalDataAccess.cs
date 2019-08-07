namespace EA.Weee.RequestHandlers.Aatf.GetAatfExternal
{
    using EA.Weee.Domain.AatfReturn;
    using System;
    using System.Threading.Tasks;

    public interface IGetAatfExternalDataAccess
    {
        Task<Aatf> GetAatfById(Guid id);
    }
}
