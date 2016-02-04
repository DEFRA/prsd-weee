namespace EA.Weee.RequestHandlers.DataReturns.ReturnVersionBuilder
{
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Domain.DataReturns;
    using Domain.Producer;

    public interface IDataReturnVersionBuilderDataAccess
    {
        Task<DataReturn> FetchDataReturnOrDefault();

        Task<RegisteredProducer> GetRegisteredProducer(string producerRegistrationNumber);

        Task<AatfDeliveryLocation> GetOrAddAatfDeliveryLocation(string approvalNumber, string facilityName);

        Task<AeDeliveryLocation> GetOrAddAeDeliveryLocation(string approvalNumber, string operatorName);
    }
}
