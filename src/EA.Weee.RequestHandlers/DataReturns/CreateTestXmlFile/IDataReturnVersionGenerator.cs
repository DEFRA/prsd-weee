namespace EA.Weee.RequestHandlers.DataReturns.CreateTestXmlFile
{
    using System.Threading.Tasks;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Domain.DataReturns;

    public interface IDataReturnVersionGenerator
    {
        Task<DataReturnVersion> GenerateAsync(TestFileSettings settings);
    }
}
