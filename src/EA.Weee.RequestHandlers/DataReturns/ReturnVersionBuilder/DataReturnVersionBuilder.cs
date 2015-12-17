namespace EA.Weee.RequestHandlers.DataReturns.ReturnVersionBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using BusinessValidation;
    using Core.DataReturns;
    using Core.Shared;
    using Domain.DataReturns;
    using Domain.Lookup;

    public class DataReturnVersionBuilder : IDataReturnVersionBuilder
    {
        private readonly string schemeApprovalNumber;

        private readonly int complianceYear;

        private readonly Core.DataReturns.QuarterType quarter;

        private readonly IEeeValidator eeeValidator;

        private readonly IDataReturnVersionBuilderDataAccess dataAccess;

        public DataReturnVersionBuilder(string schemeApprovalNumber, int complianceYear, Core.DataReturns.QuarterType quarter,
            IEeeValidator eeeValidator, IDataReturnVersionBuilderDataAccess dataAccess)
        {
            this.schemeApprovalNumber = schemeApprovalNumber;
            this.complianceYear = complianceYear;
            this.quarter = quarter;
            this.eeeValidator = eeeValidator;
            this.dataAccess = dataAccess;
        }

        public void AddAatfDeliveredAmount(string aatfApprovalNumber, string facilityName, WeeeCategory category, ObligationType obligationType, decimal tonnage)
        {
            throw new NotImplementedException();
        }

        public void AddAeDeliveredAmount(string approvalNumber, string operatorName, WeeeCategory category, ObligationType obligationType, decimal tonnage)
        {
            throw new NotImplementedException();
        }

        public void AddEeeOutputAmount(string producerRegistrationNumber, string producerName, WeeeCategory category, ObligationType obligationType, decimal tonnage)
        {
            throw new NotImplementedException();
        }

        public void AddWeeeCollectedAmount(WeeeCollectedAmountSourceType sourceType, WeeeCategory category, ObligationType obligationType, decimal tonnage)
        {
            throw new NotImplementedException();
        }

        public DataReturnVersionBuilderResult Build()
        {
            //if (!xmlBusinessValidatorResult.ErrorData.Any(e => e.ErrorLevel == ErrorLevel.Error))
            //{
            //    // Try to fetch the existing data return for the scheme and quarter, otherwise create a new data return.
            //    Quarter quarterDb = new Quarter(complianceYear, (QuarterType)quarter);
            //    DataReturn dataReturn = await dataAccess.FetchDataReturnOrDefaultAsync(scheme, quarterDb);
            //    if (dataReturn == null)
            //    {
            //        dataReturn = new DataReturn(scheme, quarterDb);
            //    }

            //    DataReturnVersion dataReturnVersion = new DataReturnVersion(dataReturn);
            //}

            throw new NotImplementedException();
        }
    }
}
