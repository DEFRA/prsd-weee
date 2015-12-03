namespace EA.Weee.Core.DataReturns
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class DataReturnForSubmission
    {
        public Guid DataReturnId { get; private set; }

        public Guid OrganisationId { get; private set; }

        public Quarter Quarter { get; private set; }

        public IReadOnlyCollection<DataReturnWarning> Warnings { get; private set; }

        public IReadOnlyCollection<DataReturnError> Errors { get; private set; }

        public DataReturnForSubmission(
            Guid dataReturnId,
            Guid organisationId,
            Quarter quarter,
            IReadOnlyCollection<DataReturnWarning> warnings,
            IReadOnlyCollection<DataReturnError> errors)
        {
            DataReturnId = dataReturnId;
            OrganisationId = organisationId;
            Quarter = quarter;
            Warnings = warnings;
            Errors = errors;
        }
    }
}
