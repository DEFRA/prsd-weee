namespace EA.Weee.RequestHandlers.DataReturns.ReturnVersionBuilder
{
    using Core.Shared;
    using Domain.DataReturns;
    using Domain.Lookup;

    public interface IDataReturnVersionBuilder
    {
        void AddEeeOutputAmount(string producerRegistrationNumber, string producerName, WeeeCategory category, ObligationType obligationType, decimal tonnage);

        void AddWeeeCollectedAmount(WeeeCollectedAmountSourceType sourceType, WeeeCategory category, ObligationType obligationType, decimal tonnage);

        void AddAatfDeliveredAmount(string aatfApprovalNumber, string facilityName, WeeeCategory category, ObligationType obligationType, decimal tonnage);

        void AddAeDeliveredAmount(string approvalNumber, string operatorName, WeeeCategory category, ObligationType obligationType, decimal tonnage);

        DataReturnVersionBuilderResult Build();
    }
}
