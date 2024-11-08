namespace EA.Weee.Core.Tests.Unit.Validation
{
    using DataReturns;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Xunit;

    public class CompaniesRegistrationNumberStringLengthAttributeTests
    {
        private readonly List<ValidationResult> validationResults;
      
        public CompaniesRegistrationNumberStringLengthAttributeTests()
        {
            validationResults = new List<ValidationResult>();
        }
    }
}
