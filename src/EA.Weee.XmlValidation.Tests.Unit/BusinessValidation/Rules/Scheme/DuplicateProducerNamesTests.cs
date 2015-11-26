namespace EA.Weee.XmlValidation.Tests.Unit.BusinessValidation.Rules.Scheme
{
    using System;
    using System.Linq;
    using Core.Shared;
    using Xml.MemberUpload;
    using XmlValidation.BusinessValidation.Rules.Scheme;
    using Xunit;
    using schemeType = Xml.MemberUpload.schemeType;

    public class DuplicateProducerNamesTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ProducerWithoutProducerName_ValidationSucceeds(string producerName)
        {
            var scheme = new schemeType
            {
                producerList = ProducersWithProducerNames(producerName)
            };

            var results = Rule().Evaluate(scheme);

            Assert.True(results.All(r => r.IsValid));
        }

        [Fact]
        public void SetOfDuplicateProducerNames_ValidationFails_IncludesProducerNameInMessage_AndErrorLevelIsError()
        {
            const string producerName = "Producer Name";
            var scheme = new schemeType
            {
                producerList = ProducersWithProducerNames(producerName, producerName)
            };

            var result = Rule().Evaluate(scheme);

            var invalidResult = result.Single(r => !r.IsValid);

            Assert.Contains(producerName, invalidResult.Message);
            Assert.Equal(ErrorLevel.Error, invalidResult.ErrorLevel);
        }

        [Fact]
        public void TwoSetsOfDuplicateProducerNames_ValidationFails_IncludesBothProducerNamesInMessages()
        {
            const string firstProducerName = "First Producer Name";
            const string secondProducerName = "Second Producer Name";
            var scheme = new schemeType
            {
                producerList =
                    ProducersWithProducerNames(firstProducerName, firstProducerName, secondProducerName,
                        secondProducerName)
            };

            var results = Rule().Evaluate(scheme);

            var invalidResults = results.Where(r => !r.IsValid);

            var aggregatedErrorMessages =
                invalidResults.Select(r => r.Message).Aggregate((curr, next) => curr + ", " + next);

            Assert.Contains(firstProducerName, aggregatedErrorMessages);
            Assert.Contains(secondProducerName, aggregatedErrorMessages);
        }

        [Fact]
        public void TwoProducersWithDifferentProducerNames_ValidationSucceeds()
        {
            var scheme = new schemeType
            {
                producerList = ProducersWithProducerNames("First Producer Name", "Second Producer Name")
            };

            var results = Rule().Evaluate(scheme);

            Assert.True(results.All(r => r.IsValid));
        }

        private DuplicateProducerNames Rule()
        {
            return new DuplicateProducerNames();
        }

        private producerType[] ProducersWithProducerNames(params string[] producerNames)
        {
            return producerNames.Select(n => new producerType
            {
                status = statusType.I,
                producerBusiness = new producerBusinessType
                {
                    Item = new partnershipType
                    {
                        partnershipName = n
                    }
                }
            }).ToArray();
        }
    }
}
