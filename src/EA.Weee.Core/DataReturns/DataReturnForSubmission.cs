namespace EA.Weee.Core.DataReturns
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class DataReturnForSubmission
    {
        public Guid DataReturnUploadId { get; private set; }

        public Guid OrganisationId { get; private set; }

        public QuarterType? Quarter { get; private set; }

        public int? Year { get; private set; }

        public IReadOnlyCollection<DataReturnWarning> Warnings { get; private set; }

        public IReadOnlyCollection<DataReturnError> Errors { get; private set; }

        public bool IsResubmission { get; private set; }

        public DataReturnForSubmission(
            Guid dataReturnUploadId,
            Guid organisationId,
            int? year,
            QuarterType? quarter,
            IReadOnlyCollection<DataReturnWarning> warnings,
            IReadOnlyCollection<DataReturnError> errors,
            bool isResubmission)
        {
            DataReturnUploadId = dataReturnUploadId;
            OrganisationId = organisationId;
            Quarter = quarter;
            Year = year;
            Warnings = warnings;
            Errors = errors;
            IsResubmission = isResubmission;
        }
    }
}
