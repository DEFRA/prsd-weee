namespace EA.Weee.RequestHandlers.Admin.Obligations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Shared.CsvReading;
    using CuttingEdge.Conditions;
    using DataAccess.DataAccess;
    using Domain.Lookup;
    using Domain.Obligation;

    public class SchemeObligationsDataProcessor : ISchemeObligationsDataProcessor
    {
        private readonly ISchemeDataAccess schemeDataAccess;
        
        public SchemeObligationsDataProcessor(ISchemeDataAccess schemeDataAccess)
        {
            this.schemeDataAccess = schemeDataAccess;
        }

        public async Task<List<ObligationScheme>> Build(List<ObligationCsvUpload> obligationCsvUploads, int complianceYear)
        {
            var obligationSchemes = new List<ObligationScheme>();

            var weeeCategories = Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>().ToList();

            foreach (var obligationCsvUpload in obligationCsvUploads)
            {
                var scheme = await schemeDataAccess.GetSchemeOrDefaultByApprovalNumber(obligationCsvUpload.SchemeIdentifier);

                Condition.Requires(scheme).IsNotNull($"SchemeObligationsBuilder scheme with identifier{obligationCsvUpload.SchemeIdentifier} not found");

                var obligationScheme = new ObligationScheme(scheme, complianceYear);

                foreach (var weeeCategory in weeeCategories)
                {
                    var value = obligationCsvUpload.GetValue(weeeCategory);

                    obligationScheme.ObligationSchemeAmounts.Add(new ObligationSchemeAmount(weeeCategory, value));
                }

                obligationSchemes.Add(obligationScheme);
            }

            return obligationSchemes;
        }
    }
}