namespace EA.Weee.RequestHandlers.DataReturns.ProcessDataReturnXmlFile
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Domain.DataReturns;
    using EA.Weee.Domain.Scheme;

    public interface IProcessDataReturnXmlFileDataAccess
    {
        Task<Scheme> FetchSchemeByOrganisationIdAsync(Guid organisationId);

        Task SaveAsync(DataReturnsUpload dataReturn);
    }
}
