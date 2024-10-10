namespace EA.Weee.RequestHandlers.Tests.DataAccess.Search
{
    using EA.Weee.Core.Search;
    using EA.Weee.Domain.Producer;
    using EA.Weee.RequestHandlers.Search.FetchSmallProducerSearchResultsForCache;
    using EA.Weee.Tests.Core;
    using EA.Weee.Tests.Core.Model;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class FetchSmallProducerSearchResultsForCacheDataAccessTests
    {
        [Fact]
        public async Task FetchLatestProducers_ShouldReturnValidProducers()
        {
            using (var wrapper = new DatabaseWrapper())
            {
                var (_, country) = DirectRegistrantHelper.SetupCommonTestData(wrapper);

                const int complianceYear = 2000;
                var (organisation1, directRegistrant1, registeredProducer1) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(wrapper, "My company", "WEE/AG48365JN", complianceYear);

                var directRegistrant1Submission1 = await DirectRegistrantHelper.CreateSubmission(wrapper, directRegistrant1, registeredProducer1, complianceYear, new List<DirectRegistrantHelper.EeeOutputAmountData>(), DirectProducerSubmissionStatus.Complete);

                var directRegistrant1Submission2 = await DirectRegistrantHelper.CreateSubmission(wrapper, directRegistrant1, registeredProducer1, complianceYear + 1, new List<DirectRegistrantHelper.EeeOutputAmountData>(), DirectProducerSubmissionStatus.Complete);

                var (organisation2, directRegistrant2, registeredProducer2) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(wrapper, "My company 2", "WEE/AG48365JX", complianceYear - 1);

                var directRegistrant2Submission2 = await DirectRegistrantHelper.CreateSubmission(wrapper, directRegistrant2, registeredProducer2, complianceYear, new List<DirectRegistrantHelper.EeeOutputAmountData>(), DirectProducerSubmissionStatus.Complete);

                var (organisation3, directRegistrant3, registeredProducer3) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(wrapper, "My company 3", "WEE/AG48365JY", complianceYear);

                var directRegistrant3Submission3 = await DirectRegistrantHelper.CreateSubmission(wrapper, directRegistrant3, registeredProducer3, complianceYear, new List<DirectRegistrantHelper.EeeOutputAmountData>(), DirectProducerSubmissionStatus.Incomplete);

                var authorisedRepresentitive = new Domain.Producer.AuthorisedRepresentative("Not company 4");

                var (organisation4, directRegistrant4, registeredProducer4) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(wrapper, "My company 4", "WEE/AG49365JY", complianceYear);

                var directRegistrant4Submission4 = await DirectRegistrantHelper.CreateSubmission(wrapper, directRegistrant4, registeredProducer4, complianceYear, new List<DirectRegistrantHelper.EeeOutputAmountData>(), DirectProducerSubmissionStatus.Complete);

                var dataAccess = new FetchSmallProducerSearchResultsForCacheDataAccess(wrapper.WeeeContext);

                var result = await dataAccess.FetchLatestProducers();

                // Assertions
                result.Should().NotBeNull();
                result.Should().BeOfType<List<SmallProducerSearchResult>>();
                result.Should().HaveCountGreaterOrEqualTo(2);

                var producer1 = result.Should().ContainSingle(p => p.RegistrationNumber == "WEE/AG48365JN").Subject;
                producer1.Name.Should().Be("My company");
                producer1.Id.Should().NotBe(Guid.Empty);

                var producer2 = result.Should().ContainSingle(p => p.RegistrationNumber == "WEE/AG48365JX").Subject;
                producer2.Name.Should().Be("My company 2");
                producer2.Id.Should().NotBe(Guid.Empty);

                result.Should().NotContain(c => c.RegistrationNumber == "WEE/AG48365JY");

                result.Select(p => p.RegistrationNumber).Should().BeInAscendingOrder();
                result.Select(p => p.RegistrationNumber).Should().OnlyHaveUniqueItems();

                result.Select(p => p.Id).Should().OnlyHaveUniqueItems();
            }
        }
    }
}
