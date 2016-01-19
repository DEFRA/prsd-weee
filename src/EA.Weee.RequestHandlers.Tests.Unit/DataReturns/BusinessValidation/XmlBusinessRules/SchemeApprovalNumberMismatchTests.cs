namespace EA.Weee.RequestHandlers.Tests.Unit.DataReturns.BusinessValidation.XmlBusinessRules
{
    using System;
    using System.Linq;
    using Core.Shared;
    using Domain.Scheme;
    using RequestHandlers.DataReturns.BusinessValidation.Rules;
    using Xml.DataReturns;
    using Xunit;

    public class SchemeApprovalNumberMismatchTests
    {
        /// <summary>
        /// This test ensures that the scheme approval number provided in the XML file matches
        /// the approval number of the scheme for which the data return version is being built.
        /// If not, an "Error" level error should be returned with a description that contains
        /// the scheme approval number that was provided.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void Validate_DifferentSchemeApprovalNumber_ReturnsError()
        {
            var scheme = BuildScheme("WEE/AA1111AA/SCH");

            var result = new SchemeApprovalNumberMismatch().Validate("WEE/ZZ9999ZZ/SCH", scheme);

            Assert.NotEmpty(result);

            ErrorData firstError = result.ToArray()[0];

            Assert.NotNull(firstError);
            Assert.Equal(ErrorLevel.Error, firstError.ErrorLevel);
            Assert.Contains("WEE/ZZ9999ZZ/SCH", firstError.Description);
        }

        private Scheme BuildScheme(string approvalNumber)
        {
            var scheme = new Scheme(new Guid("FE4056B3-F892-476E-A4AB-7C111AE1EF14"));

            scheme.UpdateScheme(
                "Test scheme",
                approvalNumber,
                "1B1S",
                Domain.Obligation.ObligationType.Both,
                new Guid("C5D400BE-0CE7-43D7-BD7B-B7936967E500"));

            return scheme;
        }
    }
}
