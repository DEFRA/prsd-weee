namespace EA.Weee.DataAccess.Tests.Integration.DataAccess
{
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.Producer;
    using EA.Weee.Domain.Producer.Classfication;
    using EA.Weee.Tests.Core;
    using EA.Weee.Tests.Core.Model;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;

    public class SmallProducerDataAccessTests
    {
        [Fact]
        public async Task GetCurrentDirectRegistrantSubmissionByComplianceYear_ShouldReturnCorrectSubmission()
        {
            using (var wrapper = new DatabaseWrapper())
            {
                DirectRegistrantHelper.SetupCommonTestData(wrapper);

                const int complianceYear = 2024;
                var (_, directRegistrant1, registeredProducer1) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(wrapper, "My company", "WEE/AG38365JN", complianceYear);

                var submission1 = await DirectRegistrantHelper.CreateSubmission(wrapper, directRegistrant1, registeredProducer1, complianceYear, new List<DirectRegistrantHelper.EeeOutputAmountData>(), DirectProducerSubmissionStatus.Complete, SellingTechniqueType.Both.Value);

                await DirectRegistrantHelper.CreateSubmission(wrapper, directRegistrant1, registeredProducer1, complianceYear + 1, new List<DirectRegistrantHelper.EeeOutputAmountData>(), DirectProducerSubmissionStatus.Complete, SellingTechniqueType.Both.Value);

                var dataAccess = new SmallProducerDataAccess(wrapper.WeeeContext);

                var result =
                    await dataAccess.GetCurrentDirectRegistrantSubmissionByComplianceYear(directRegistrant1.Id,
                        complianceYear);

                result.Should().NotBeNull();
                result.Id.Should().Be(submission1.Id);
            }
        }

        [Fact]
        public async Task GetCurrentDirectRegistrantSubmissionByComplianceYear_WithRemovedProducer_ShouldReturnNull()
        {
            using (var wrapper = new DatabaseWrapper())
            {
                DirectRegistrantHelper.SetupCommonTestData(wrapper);

                const int complianceYear = 2024;
                var (_, directRegistrant1, registeredProducer1) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(wrapper, "My company", "WEE/AG38345JN", complianceYear);

                registeredProducer1.Remove();

                await wrapper.WeeeContext.SaveChangesAsync();

                var submission1 = await DirectRegistrantHelper.CreateSubmission(wrapper, directRegistrant1, registeredProducer1, complianceYear, new List<DirectRegistrantHelper.EeeOutputAmountData>(), DirectProducerSubmissionStatus.Complete, SellingTechniqueType.Both.Value);

                var dataAccess = new SmallProducerDataAccess(wrapper.WeeeContext);

                var result =
                    await dataAccess.GetCurrentDirectRegistrantSubmissionByComplianceYear(directRegistrant1.Id,
                        complianceYear);

                result.Should().BeNull();
            }
        }

        [Fact]
        public async Task GetCurrentDirectRegistrantSubmissionByComplianceYear_WithNoMatchingSubmission_ShouldReturnNull()
        {
            using (var wrapper = new DatabaseWrapper())
            {
                DirectRegistrantHelper.SetupCommonTestData(wrapper);

                const int complianceYear = 2024;
                var (_, directRegistrant1, _) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(wrapper, "My company", "WEE/AG38365JN", complianceYear);

                var dataAccess = new SmallProducerDataAccess(wrapper.WeeeContext);

                var result =
                    await dataAccess.GetCurrentDirectRegistrantSubmissionByComplianceYear(directRegistrant1.Id,
                        complianceYear);

                result.Should().BeNull();
            }
        }

        [Fact]
        public async Task GetDirectRegistrantByOrganisationId_ShouldReturnCorrectDirectRegistrant()
        {
            using (var wrapper = new DatabaseWrapper())
            {
                DirectRegistrantHelper.SetupCommonTestData(wrapper);

                const int complianceYear = 2024;
                var (organisation1, directRegistrant1, _) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(wrapper, "My company", "WEE/AG38365JN", complianceYear);

                wrapper.WeeeContext.DirectRegistrants.Add(directRegistrant1);
                await wrapper.WeeeContext.SaveChangesAsync();
                var dataAccess = new SmallProducerDataAccess(wrapper.WeeeContext);

                var result =
                    await dataAccess.GetDirectRegistrantByOrganisationId(organisation1.Id);

                result.Should().NotBeNull();
                result.Id.Should().Be(directRegistrant1.Id);
            }
        }

        [Fact]
        public async Task GetDirectRegistrantByOrganisationId_WithNoMatchingDirectRegistrant_ShouldReturnNull()
        {
            using (var wrapper = new DatabaseWrapper())
            {
                DirectRegistrantHelper.SetupCommonTestData(wrapper);

                const int complianceYear = 2024;
                var (_, directRegistrant1, _) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(wrapper, "My company", "WEE/AG38365JN", complianceYear);

                wrapper.WeeeContext.DirectRegistrants.Add(directRegistrant1);
                await wrapper.WeeeContext.SaveChangesAsync();
                var dataAccess = new SmallProducerDataAccess(wrapper.WeeeContext);

                var result =
                    await dataAccess.GetDirectRegistrantByOrganisationId(Guid.NewGuid());

                result.Should().BeNull();
            }
        }

        [Fact]
        public async Task GetById_WithNoMatchingDirectRegistrant_ShouldReturnNull()
        {
            using (var wrapper = new DatabaseWrapper())
            {
                DirectRegistrantHelper.SetupCommonTestData(wrapper);

                const int complianceYear = 2024;
                var (_, directRegistrant1, _) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(wrapper, "My company", "WEE/AG38365JN", complianceYear);

                wrapper.WeeeContext.DirectRegistrants.Add(directRegistrant1);
                await wrapper.WeeeContext.SaveChangesAsync();
                var dataAccess = new SmallProducerDataAccess(wrapper.WeeeContext);

                var result =
                    await dataAccess.GetById(Guid.NewGuid());

                result.Should().BeNull();
            }
        }

        [Fact]
        public async Task GetById_ShouldReturnCorrectDirectRegistrant()
        {
            using (var wrapper = new DatabaseWrapper())
            {
                DirectRegistrantHelper.SetupCommonTestData(wrapper);

                const int complianceYear = 2024;
                var (organisation1, directRegistrant1, _) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(wrapper, "My company", "WEE/AG38365JN", complianceYear);

                wrapper.WeeeContext.DirectRegistrants.Add(directRegistrant1);
                await wrapper.WeeeContext.SaveChangesAsync();
                var dataAccess = new SmallProducerDataAccess(wrapper.WeeeContext);

                var result =
                    await dataAccess.GetById(directRegistrant1.Id);

                result.Should().NotBeNull();
                result.Id.Should().Be(directRegistrant1.Id);
            }
        }

        [Fact]
        public async Task GetById_ShouldReturnCorrectDirectRegistrant_WithNonRemovedProducerRecords()
        {
            using (var wrapper = new DatabaseWrapper())
            {
                DirectRegistrantHelper.SetupCommonTestData(wrapper);

                const int complianceYear = 2024;
                var (organisation1, directRegistrant1, registeredProducer1) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(wrapper, "My company", "WEE/AG38365JN", complianceYear);

                var submission1 = await DirectRegistrantHelper.CreateSubmission(wrapper, directRegistrant1, registeredProducer1, complianceYear, new List<DirectRegistrantHelper.EeeOutputAmountData>(), DirectProducerSubmissionStatus.Complete, SellingTechniqueType.Both.Value);

                registeredProducer1.Remove();
                await wrapper.WeeeContext.SaveChangesAsync();

                var registeredProducer2 = new Domain.Producer.RegisteredProducer("WEE/AG38365JN", complianceYear);
                wrapper.WeeeContext.AllRegisteredProducers.Add(registeredProducer2);

                await DirectRegistrantHelper.CreateSubmission(wrapper, directRegistrant1, registeredProducer2, complianceYear, new List<DirectRegistrantHelper.EeeOutputAmountData>(), DirectProducerSubmissionStatus.Complete, SellingTechniqueType.Both.Value);

                await wrapper.WeeeContext.SaveChangesAsync();

                var dataAccess = new SmallProducerDataAccess(wrapper.WeeeContext);

                var result =
                    await dataAccess.GetById(directRegistrant1.Id);

                result.Should().NotBeNull();
                result.Id.Should().Be(directRegistrant1.Id);
                result.DirectProducerSubmissions.Count.Should().Be(1);
                result.DirectProducerSubmissions.Should().NotContain(d => d.RegisteredProducer.Removed == true);
            }
        }
    }
}
