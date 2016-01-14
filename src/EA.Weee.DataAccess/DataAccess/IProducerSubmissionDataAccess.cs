namespace EA.Weee.DataAccess.DataAccess
{
    using System.Collections.Generic;
    using Domain.Producer;

    public interface IProducerSubmissionDataAccess
    {
        void AddRange(IEnumerable<ProducerSubmission> producerSubmissions);
    }
}
