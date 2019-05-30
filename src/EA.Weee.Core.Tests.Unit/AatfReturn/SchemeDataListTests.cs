namespace EA.Weee.Core.Tests.Unit.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using Core.AatfReturn;
    using EA.Weee.Core.Organisations;
    using FakeItEasy;
    using FluentAssertions;
    using Scheme;
    using Xunit;

    public class SchemeDataListTests
    {
        [Fact]
        public void Construct_GivenNullSchemeData_ArgumentNullExceptionExpected()
        {
            Action act = () =>
            {
                var schemeDataList = new SchemeDataList(null, OrganisationData());
            };

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Construct_GivenNullOrganisationData_ArgumentNullExceptionExpected()
        {
            Action act = () =>
            {
                var schemeDataList = new SchemeDataList(new List<SchemeData>(), null);
            };

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Construct_GivenParameters_PropertiesShouldBeSet()
        {
            var organisationData = OrganisationData();
            var list = new List<SchemeData>();

            var schemeDataList = new SchemeDataList(list, organisationData);

            schemeDataList.OrganisationData.Should().Be(organisationData);
            schemeDataList.SchemeDataItems.Should().BeSameAs(list);
        }

        private OrganisationData OrganisationData()
        {
            return new OrganisationData();
        }
    }
}
