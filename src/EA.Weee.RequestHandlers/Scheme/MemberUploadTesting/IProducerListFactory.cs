namespace EA.Weee.RequestHandlers.Scheme.MemberUploadTesting
{
    using System.Threading.Tasks;
    using Core.Scheme.MemberUploadTesting;

    /// <summary>
    /// Creates a <see cref="ProducerList"/> based on a collection of settings specifying the
    /// schema version, the compliance year, the number of new/existing producers etc.
    /// </summary>
    public interface IProducerListFactory
    {
        Task<ProducerList> CreateAsync(ProducerListSettings listSettings);
    }
}
