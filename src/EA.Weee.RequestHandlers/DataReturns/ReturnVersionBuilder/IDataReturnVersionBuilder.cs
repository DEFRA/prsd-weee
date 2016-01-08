namespace EA.Weee.RequestHandlers.DataReturns.ReturnVersionBuilder
{
    using System.Threading.Tasks;
    using Domain;
    using Domain.DataReturns;
    using Domain.Lookup;
    using Scheme = Domain.Scheme.Scheme;

    /// <summary>
    /// Builds data return versions for a specified scheme and quarter.
    /// </summary>
    public interface IDataReturnVersionBuilder
    {
        Scheme Scheme { get; }

        Quarter Quarter { get; }

        Task AddEeeOutputAmount(string producerRegistrationNumber, string producerName, WeeeCategory category, ObligationType obligationType, decimal tonnage);

        Task AddWeeeCollectedAmount(WeeeCollectedAmountSourceType sourceType, WeeeCategory category, ObligationType obligationType, decimal tonnage);

        Task AddAatfDeliveredAmount(string aatfApprovalNumber, string facilityName, WeeeCategory category, ObligationType obligationType, decimal tonnage);

        Task AddAeDeliveredAmount(string approvalNumber, string operatorName, WeeeCategory category, ObligationType obligationType, decimal tonnage);

        DataReturnVersionBuilderResult Build();
    }
}
