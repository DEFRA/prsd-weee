namespace EA.Weee.RequestHandlers.Admin.Reports.GetProducerEeeDataCsv
{
    using EA.Weee.Core.Constants;
    using EA.Weee.DataAccess;
    using EA.Weee.DataAccess.StoredProcedure;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Threading.Tasks;

    public class GetProducerEeeDataCsvDataAccess : IGetProducerEeeDataCsvDataAccess
    {
        private readonly WeeeContext context;

        public GetProducerEeeDataCsvDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<Domain.Scheme.Scheme> GetSchemeAsync(Guid schemeId)
        {
            return await context.Schemes.SingleAsync(s => s.Id == schemeId);
        }

        public async Task<List<ProducerEeeCsvData>> GetItemsAsync(int complianceYear, Guid? schemeId, string obligationType)
        {
            var filterByDirectRegistrant = false;
            if (schemeId == DirectRegistrantFixedIdConstant.DirectRegistrantFixedId)
            {
                schemeId = null;
                filterByDirectRegistrant = true;
            }

            return await context.StoredProcedures.SpgProducerEeeCsvData(
                complianceYear,
                schemeId,
                obligationType,
                filterByDirectRegistrant);
        }
    }
}
