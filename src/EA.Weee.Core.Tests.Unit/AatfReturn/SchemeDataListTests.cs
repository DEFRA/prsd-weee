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
                var schemeDataList = new SchemeDataList(null, OperatorData());
            };

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Construct_GivenNullOperatorData_ArgumentNullExceptionExpected()
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
            var @operator = OperatorData();
            var list = new List<SchemeData>();

            var schemeDataList = new SchemeDataList(list, @operator);

            schemeDataList.OperatorData.Should().Be(@operator);
            schemeDataList.SchemeDataItems.Should().BeSameAs(list);
        }

        private OperatorData OperatorData()
        {
            return new OperatorData(A.Dummy<Guid>(), A.Dummy<string>(), A.Dummy<OrganisationData>(), A.Dummy<Guid>());
        }
    }
}
