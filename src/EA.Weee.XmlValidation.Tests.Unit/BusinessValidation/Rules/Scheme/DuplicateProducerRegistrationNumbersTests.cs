namespace EA.Weee.XmlValidation.Tests.Unit.BusinessValidation.Rules.Scheme
{
    using System;
    using System.Linq;
    using Core.Shared;
    using Xml.MemberUpload;
    using XmlValidation.BusinessValidation.Rules.Scheme;
    using Xunit;
    using schemeType = Xml.MemberUpload.schemeType;

    public class DuplicateProducerRegistrationNumbersTests
    {
        [Fact]
        public void DuplicateRegistrationNumbers_ValidationFails_IncludesRegistraionNumberInMessage_AndErrorLevelIsError()
        {
            const string registrationNumber = "ABC12345";
            var scheme = new schemeType
            {
                producerList = ProducersWithRegistrationNumbers(registrationNumber, registrationNumber)
            };

            var results = Rule().Evaluate(scheme);

            var duplicate = results.Single(r => !r.IsValid);

            Assert.Contains(registrationNumber, duplicate.Message);
            Assert.Equal(ErrorLevel.Error, duplicate.ErrorLevel);
        }

        [Fact]
        public void SetOfEmptyRegistrationNumbers_ValidationSucceeds()
        {
            var scheme = new schemeType
            {
                producerList = ProducersWithRegistrationNumbers(string.Empty, string.Empty)
            };

            var results = Rule().Evaluate(scheme);

            Assert.True(results.All(r => r.IsValid));
        }

        [Fact]
        public void TwoSetsOfDuplicateRegistrationNumbers_ValidationFails_IncludesBothRegistrationNumbersInMessages()
        {
            const string firstRegistrationNumber = "ABC12345";
            const string secondRegistrationNumber = "XYZ54321";

            var scheme = new schemeType
            {
                producerList =
                    ProducersWithRegistrationNumbers(firstRegistrationNumber, firstRegistrationNumber,
                        secondRegistrationNumber, secondRegistrationNumber)
            };

            var results = Rule().Evaluate(scheme);

            var invalidResults = results.Where(r => !r.IsValid);

            Assert.Equal(2, invalidResults.Count());

            var aggregatedErrorMessages =
                invalidResults.Select(r => r.Message).Aggregate((curr, next) => curr + ", " + next);

            Assert.Contains(firstRegistrationNumber, aggregatedErrorMessages);
            Assert.Contains(secondRegistrationNumber, aggregatedErrorMessages);
        }

        [Fact]
        public void TwoProducersWithDifferentRegistrationNumbers_ValidationSucceeds()
        {
            var scheme = new schemeType
            {
                producerList = ProducersWithRegistrationNumbers("ABC12345", "XYZ54321").ToArray()
            };

            var result = Rule().Evaluate(scheme);

            Assert.True(result.All(r => r.IsValid));
        }

        private DuplicateProducerRegistrationNumbers Rule()
        {
            return new DuplicateProducerRegistrationNumbers();
        }

        private static producerType[] ProducersWithRegistrationNumbers(params string[] regstrationNumbers)
        {
            return regstrationNumbers.Select(r => new producerType
            {
                status = statusType.A,
                registrationNo = r,
                producerBusiness = new producerBusinessType
                {
                    Item = new partnershipType
                    {
                        partnershipName = Guid.NewGuid().ToString()
                    }
                }
            }).ToArray();
        }
    }
}
