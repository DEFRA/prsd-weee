namespace EA.Weee.RequestHandlers.DataReturns.CreateTestXmlFile
{
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Domain.DataReturns;
    using System.Threading.Tasks;

    public interface IDataReturnVersionGenerator
    {
        Task<DataReturnVersion> GenerateAsync(TestFileSettings settings);
    }
}
