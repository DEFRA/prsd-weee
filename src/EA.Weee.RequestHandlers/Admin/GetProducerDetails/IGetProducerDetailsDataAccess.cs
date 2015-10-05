namespace EA.Weee.RequestHandlers.Admin.GetProducerDetails
{
    using EA.Weee.Domain.Producer;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IGetProducerDetailsDataAccess
    {
        Task<List<Producer>> Fetch(string registrationNumber);
    }
}
