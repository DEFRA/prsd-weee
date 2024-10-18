namespace EA.Weee.RequestHandlers.Shared
{
    using System.Threading.Tasks;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Domain.Producer;

    public interface ISmallProducerSubmissionService
    {
        Task<SmallProducerSubmissionData> GetSmallProducerSubmissionData(DirectRegistrant directRegistrant);
    }
}