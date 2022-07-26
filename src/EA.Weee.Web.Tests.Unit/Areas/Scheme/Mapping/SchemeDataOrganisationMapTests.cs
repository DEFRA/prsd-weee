namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Mapping
{
    using AutoFixture;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using Weee.Tests.Core;
    using Xunit;

    public class SchemeDataOrganisationMapTests : SimpleUnitTestBase
    {
        private readonly SchemeDataOrganisationMap schemeDataOrganisationMap;
        private readonly IMapper mapper;

        public SchemeDataOrganisationMapTests()
        {
            mapper = A.Fake<IMapper>();

            schemeDataOrganisationMap = new SchemeDataOrganisationMap(mapper);
        }

        [Fact]
        public void Map_GiveListOfSchemeDataIsNull_ArgumentNullExceptionExpected()
        {
            // act
            var exception = Record.Exception(() => schemeDataOrganisationMap.Map(null));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GiveListOfSchemeDataIsEmpty_ArgumentExceptionExpected()
        {
            // act
            var exception = Record.Exception(() => schemeDataOrganisationMap.Map(new List<SchemeData>()));

            //assert
            exception.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void Map_GivenListOfOrganisationSchemeData_PropertiesShouldBeMapped()
        {
            //arrange
            var schemeData1 = A.Fake<SchemeData>();
            var schemeDataName1 = TestFixture.Create<string>();
            var schemeDataId1 = TestFixture.Create<Guid>();

            ObjectInstantiator<SchemeData>.SetProperty(o => o.OrganisationId, schemeDataId1, schemeData1);
            ObjectInstantiator<SchemeData>.SetProperty(o => o.SchemeName, schemeDataName1, schemeData1);

            var schemeData2 = A.Fake<SchemeData>();
            var schemeDataName2 = TestFixture.Create<string>();
            var schemeDataId2 = TestFixture.Create<Guid>();

            ObjectInstantiator<SchemeData>.SetProperty(o => o.OrganisationId, schemeDataId2, schemeData2);
            ObjectInstantiator<SchemeData>.SetProperty(o => o.SchemeName, schemeDataName2, schemeData2);

            var listOfSchemeData = new List<SchemeData>
            {
                 schemeData1,
                 schemeData2
            };

            //act
            var result = schemeDataOrganisationMap.Map(listOfSchemeData);

            // assert 
            result.Should().NotBeEmpty();
            result[0].Id.Should().Be(schemeDataId1);
            result[0].DisplayName.Should().Be(schemeDataName1);
            result[1].Id.Should().Be(schemeDataId2);
            result[1].DisplayName.Should().Be(schemeDataName2);
        }
    }
}
