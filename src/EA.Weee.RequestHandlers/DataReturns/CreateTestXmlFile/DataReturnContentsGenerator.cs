namespace EA.Weee.RequestHandlers.DataReturns.CreateTestXmlFile
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Domain.DataReturns;
    using Quarter = EA.Weee.Domain.DataReturns.Quarter;
    using QuarterType = EA.Weee.Domain.DataReturns.QuarterType;

    public class DataReturnContentsGenerator : IDataReturnContentsGenerator
    {
        private readonly IDataReturnContentsGeneratorDataAccess dataAccess;

        public DataReturnContentsGenerator(IDataReturnContentsGeneratorDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public async Task<DataReturnContents> GenerateAsync(TestFileSettings settings)
        {
            Domain.Scheme.Scheme scheme = await dataAccess.FetchSchemeAsync(settings.OrganisationID);

            Quarter quarter = new Quarter(
                settings.Quarter.Year,
                (QuarterType)settings.Quarter.Q);

            DataReturn dataReturn = new DataReturn(scheme, quarter);

            DataReturnContents dataReturnContents = new DataReturnContents(dataReturn);

            // TODO: Populate the data return contents domain object.

            dataReturn.SetContents(dataReturnContents);

            return dataReturnContents;
        }
    }
}
