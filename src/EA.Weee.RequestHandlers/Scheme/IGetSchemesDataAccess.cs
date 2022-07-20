namespace EA.Weee.RequestHandlers.Scheme
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EA.Weee.Domain.Organisation;

    public interface IGetSchemesDataAccess
    {
        Task<IList<Domain.Scheme.Scheme>> GetCompleteSchemes();

        Task<Domain.Scheme.Scheme> GetSchemeBasedOnId(Guid id);

        Task<ProducerBalancingScheme> GetProducerBalancingScheme();
    }
}
