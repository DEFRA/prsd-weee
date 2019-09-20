namespace EA.Weee.RequestHandlers.DataReturns.ReturnVersionBuilder
{
    using Domain.DataReturns;
    using Domain.Producer;
    using System.Threading.Tasks;

    public interface IDataReturnVersionBuilderDataAccess
    {
        Task<DataReturn> FetchDataReturnOrDefault();

        Task<RegisteredProducer> GetRegisteredProducer(string producerRegistrationNumber);

        Task<AatfDeliveryLocation> GetOrAddAatfDeliveryLocation(string approvalNumber, string facilityName);

        Task<AeDeliveryLocation> GetOrAddAeDeliveryLocation(string approvalNumber, string operatorName);
    }
}
