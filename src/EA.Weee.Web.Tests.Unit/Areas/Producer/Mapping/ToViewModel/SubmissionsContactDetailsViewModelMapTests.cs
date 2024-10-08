namespace EA.Weee.Web.Tests.Unit.Areas.Producer.Mapping.ToViewModel
{
    using AutoFixture;
    using AutoFixture.AutoFakeItEasy;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.Web.Areas.Producer.Mappings.ToViewModel;
    using FakeItEasy;
    using FluentAssertions;
    using System.Collections.Generic;
    using System.Security.AccessControl;
    using System.Web.Mvc;
    using Xunit;

    public class SubmissionsContactDetailsViewModelMapTests
    {
        private readonly IFixture fixture;
        private readonly SubmissionsContactDetailsViewModelMap map;
        private readonly IMapper mapper;

        public SubmissionsContactDetailsViewModelMapTests()
        {
            fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            mapper = fixture.Freeze<IMapper>();

            map = new SubmissionsContactDetailsViewModelMap(mapper);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(2024)]
        public void Map_ShouldMapValues(int? year)
        {
            // Arrange
            var producerSubmission = fixture.Create<SmallProducerSubmissionData>();
            producerSubmission.SubmissionHistory = new Dictionary<int, SmallProducerSubmissionHistoryData>()
            {
                { year.HasValue ? year.Value : 2024, fixture.Create<SmallProducerSubmissionHistoryData>() }
            };

            producerSubmission.OrganisationData = fixture.Build<OrganisationData>().Create();

            var source = fixture.Build<SubmissionsYearDetails>()
            .With(x => x.SmallProducerSubmissionData, producerSubmission)
            .With(x => x.Year, year)
            .Create();

            var expectedAddress = fixture.Create<AddressPostcodeRequiredData>();
            A.CallTo(() => mapper.Map<AddressData, AddressPostcodeRequiredData>(A<AddressData>._))
                .Returns(expectedAddress);

            // Act
            ContactDetailsViewModel result = map.Map(source);

            // Assert
            if (year.HasValue)
            {
                result.FirstName.Should().Be(source.SmallProducerSubmissionData.SubmissionHistory[year.Value].ContactData.FirstName);
                result.LastName.Should().Be(source.SmallProducerSubmissionData.SubmissionHistory[year.Value].ContactData.LastName);
                result.Position.Should().Be(source.SmallProducerSubmissionData.SubmissionHistory[year.Value].ContactData.Position);

                A.CallTo(() => mapper
                           .Map<AddressData, AddressPostcodeRequiredData>(source.SmallProducerSubmissionData.SubmissionHistory[year.Value].ContactAddressData))
                           .MustHaveHappenedOnceExactly();
            }
            else
            {
                result.FirstName.Should().Be(source.SmallProducerSubmissionData.ContactData.FirstName);
                result.LastName.Should().Be(source.SmallProducerSubmissionData.ContactData.LastName);
                result.Position.Should().Be(source.SmallProducerSubmissionData.ContactData.Position);

                A.CallTo(() => mapper
                           .Map<AddressData, AddressPostcodeRequiredData>(A<AddressData>.That
                           .Matches(x => x == source.SmallProducerSubmissionData.ContactAddressData)))
                           .MustHaveHappenedOnceExactly();
            }
        }

        //[Theory]
        //[InlineData(OrganisationType.Partnership, ExternalOrganisationType.Partnership, null)]
        //[InlineData(OrganisationType.RegisteredCompany, ExternalOrganisationType.RegisteredCompany, null)]
        //[InlineData(OrganisationType.SoleTraderOrIndividual, ExternalOrganisationType.SoleTrader, null)]
        //[InlineData(OrganisationType.Partnership, ExternalOrganisationType.Partnership, 2024)]
        //[InlineData(OrganisationType.RegisteredCompany, ExternalOrganisationType.RegisteredCompany, 2024)]
        //[InlineData(OrganisationType.SoleTraderOrIndividual, ExternalOrganisationType.SoleTrader, 2024)]
        //public void Map_ShouldMapOrganisationTypeCorrectly(OrganisationType sourceType, ExternalOrganisationType expectedType, int? year)
        //{
        //    // Arrange
        //    var producerSubmission = fixture.Create<SmallProducerSubmissionData>();
        //    producerSubmission.SubmissionHistory = new Dictionary<int, SmallProducerSubmissionHistoryData>()
        //    {
        //        { 2024, fixture.Create<SmallProducerSubmissionHistoryData>() }
        //    };

        //    producerSubmission.OrganisationData = fixture.Build<OrganisationData>()
        //            .With(o => o.OrganisationType, sourceType)
        //            .Create();
        //    var source = fixture.Build<SubmissionsYearDetails>()
        //    .With(x => x.SmallProducerSubmissionData, producerSubmission)
        //    .With(x => x.Year, year)
        //    .Create();

        //    // Act
        //    var result = map.Map(source);

        //    // Assert
        //    result.OrganisationType.Should().Be(expectedType);
        //}

        //[Theory]
        //[InlineData(null)]
        //[InlineData(2024)]
        //public void Map_ShouldMapAddress(int? year)
        //{
        //    // Arrange
        //    var producerSubmission = fixture.Create<SmallProducerSubmissionData>();
        //    producerSubmission.SubmissionHistory = new Dictionary<int, SmallProducerSubmissionHistoryData>() 
        //    {
        //        { 2024, fixture.Create<SmallProducerSubmissionHistoryData>() }
        //    };

        //    var source = fixture.Build<SubmissionsYearDetails>()
        //    .With(x => x.SmallProducerSubmissionData, producerSubmission)
        //    .With(x => x.Year, year)
        //    .Create();

        //    // Act
        //    Core.Organisations.Base.OrganisationViewModel result = map.Map(source);

        //    // Assert
        //    if (year.HasValue)
        //    {
        //        A.CallTo(() => mapper
        //                    .Map<AddressData, ExternalAddressData>(source.SmallProducerSubmissionData.SubmissionHistory[year.Value].BusinessAddressData))
        //                    .MustHaveHappenedOnceExactly();
        //    }
        //    else
        //    {
        //        A.CallTo(() => mapper
        //                   .Map<AddressData, ExternalAddressData>(source.SmallProducerSubmissionData.OrganisationData.BusinessAddress))
        //                   .MustHaveHappenedOnceExactly();
        //    }
        //}
    }
}