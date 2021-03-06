﻿namespace EA.Weee.Requests.Admin
{
    using Prsd.Core.Mediator;

    public class IsProducerRegisteredForComplianceYear : IRequest<bool>
    {
        public string RegistrationNumber { get; set; }

        public int ComplianceYear { get; set; }

        public IsProducerRegisteredForComplianceYear(string registrationNumber, int complianceYear)
        {
            RegistrationNumber = registrationNumber;
            ComplianceYear = complianceYear;
        }
    }
}
