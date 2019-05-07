namespace EA.Weee.RequestHandlers.Admin.Aatf
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    public interface IGetAatfsDataAccess
    {
        Task<List<Domain.AatfReturn.Aatf>> GetAatfs();
    }
}
