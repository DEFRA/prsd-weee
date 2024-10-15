﻿namespace EA.Weee.DataAccess.DataAccess
{
    using System;
    using System.Threading.Tasks;
    using EA.Weee.Domain.Producer;

    public interface ISmallProducerDataAccess
    {
        Task<DirectProducerSubmission> GetCurrentDirectRegistrantSubmissionByComplianceYear(Guid directRegistrantId, int complianceYear);
    }
}