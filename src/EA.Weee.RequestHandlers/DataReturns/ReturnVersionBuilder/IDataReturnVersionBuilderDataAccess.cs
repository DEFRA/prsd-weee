namespace EA.Weee.RequestHandlers.DataReturns.ReturnVersionBuilder
{
    using System.Threading.Tasks;
    using Domain.DataReturns;
    using Domain.Producer;
    public interface IDataReturnVersionBuilderDataAccess
    {
        Task<DataReturn> FetchDataReturnOrDefault();

        Task<RegisteredProducer> GetRegisteredProducer(string producerRegistrationNumber);
    }
}
