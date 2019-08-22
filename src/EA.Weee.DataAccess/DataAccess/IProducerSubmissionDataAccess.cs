namespace EA.Weee.DataAccess.DataAccess
{
    using Domain.Producer;
    using System.Collections.Generic;

    public interface IProducerSubmissionDataAccess
    {
        void AddRange(IEnumerable<ProducerSubmission> producerSubmissions);
    }
}
