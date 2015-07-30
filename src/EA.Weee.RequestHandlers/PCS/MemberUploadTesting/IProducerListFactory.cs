namespace EA.Weee.RequestHandlers.PCS.MemberUploadTesting
{
    using EA.Weee.Core.PCS.MemberUploadTesting;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Creates a <see cref="ProducerList"/> based on a collection of settings specifying the
    /// schema version, the compliance year, the number of new/existing producers etc.
    /// </summary>
    public interface IProducerListFactory
    {
        Task<ProducerList> CreateAsync(ProducerListSettings listSettings);
    }
}
