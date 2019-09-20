namespace EA.Weee.RequestHandlers.AatfReturn.AatfTaskList
{
    using EA.Weee.Domain.AatfReturn;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IFetchAatfDataAccess
    {
        Task<List<Aatf>> FetchAatfByReturnQuarterWindow(Return @return);

        Task<List<Aatf>> FetchAatfByReturnId(Guid returnId);

        Task<Aatf> FetchByApprovalNumber(string approvalNumber, int? complianceYear);

        Task<Aatf> FetchById(Guid id);
    }
}
