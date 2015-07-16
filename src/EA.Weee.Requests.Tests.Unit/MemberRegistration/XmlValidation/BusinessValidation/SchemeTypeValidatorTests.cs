namespace EA.Weee.Requests.Tests.Unit.MemberRegistration.XmlValidation.BusinessValidation
{
    using System.Collections.Generic;
    using System.Linq;
    using Domain;
    using RequestHandlers;
    using RequestHandlers.PCS.MemberRegistration.XmlValidation.BusinessValidation;
    using Xunit;

    public class SchemeTypeValidatorTests
    {
        [Fact]
        public void SetOfDuplicateRegistrationNumbers_ValidationFails_IncludesRegistraionNumberInMessage_AndErrorLevelIsError()
        {
            const string registrationNumber = "ABC12345";
            var xml = new schemeType
            {
                producerList = Producers(registrationNumber, registrationNumber).ToArray()
            };

            var result = new SchemeTypeValidator().Validate(xml);

            Assert.False(result.IsValid);
            Assert.Contains(registrationNumber, result.Errors.Single().ErrorMessage);
            Assert.Equal(ErrorLevel.Error, result.Errors.Single().CustomState);
        }

        [Fact]
        public void TwoSetsOfDuplicateRegistrationNumbers_ValidationFails_IncludesBothRegistrationNumbersInMessages()
        {
            const string firstRegistrationNumber = "ABC12345";
            const string secondRegistrationNumber = "XYZ54321";
            var xml = new schemeType
            {
                producerList = Producers(firstRegistrationNumber, firstRegistrationNumber, secondRegistrationNumber, secondRegistrationNumber)
            };

            var result = new SchemeTypeValidator().Validate(xml);

            Assert.False(result.IsValid);

            var aggregatedErrorMessages = result.Errors.Select(err => err.ErrorMessage).Aggregate((curr, next) => curr + ", " + next);

            Assert.Contains(firstRegistrationNumber, aggregatedErrorMessages);
            Assert.Contains(secondRegistrationNumber, aggregatedErrorMessages);
        }

        [Fact]
        public void TwoProducersWithDifferentRegistrationNumbers_ValidationSucceeds()
        {
            var xml = new schemeType
            {
                producerList = Producers("ABC12345", "XYZ54321").ToArray()
            };

            var result = new SchemeTypeValidator().Validate(xml);

            Assert.True(result.IsValid);
        }

        private producerType[] Producers(params string[] regstrationNumbers)
        {
            return regstrationNumbers.Select(r => new producerType
            {
                status = statusType.A,
                registrationNo = r
            }).ToArray();
        }
    }
}
