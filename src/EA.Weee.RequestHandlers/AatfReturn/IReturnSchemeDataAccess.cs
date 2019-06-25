namespace EA.Weee.RequestHandlers.AatfReturn
{
    using EA.Weee.Domain.AatfReturn;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.Organisation;

    public interface IReturnSchemeDataAccess
    {
        Task<List<Guid>> Submit(List<ReturnScheme> scheme);

        Task<List<ReturnScheme>> GetSelectedSchemesByReturnId(Guid returnId);

        Task<Organisation> GetOrganisationByReturnId(Guid returnId);

        Task RemoveReturnScheme(Guid schemeId);
    }
}
