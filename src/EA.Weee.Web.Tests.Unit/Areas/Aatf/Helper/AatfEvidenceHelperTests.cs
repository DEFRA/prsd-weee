namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Helper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using Core.AatfReturn;
    using FluentAssertions;
    using Web.Areas.Aatf.Helpers;
    using Weee.Tests.Core;
    using Xunit;

    public class AatfEvidenceHelperTests : SimpleUnitTestBase
    {
        private readonly AatfEvidenceHelper aatfHelper;

        public AatfEvidenceHelperTests()
        {
            aatfHelper = new AatfEvidenceHelper();
        }

        [Fact]
        public void AatfCanEditCreateNotes_GivenAatfIsNotFound_ArgumentNullExceptionExpected()
        {
            //arrange
            var aatfs = TestFixture.CreateMany<AatfData>().ToList();

            //act
            var exception = Record.Exception(() =>
                aatfHelper.AatfCanEditCreateNotes(aatfs, TestFixture.Create<Guid>(), TestFixture.Create<int>()));
            
            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void AatfCanEditCreateNotes_GivenMatchingCriteria_TrueShouldBeReturned()
        {
            //arrange
            var id = TestFixture.Create<Guid>();
            var aatfId = TestFixture.Create<Guid>();
            var complianceYear = TestFixture.Create<int>();

            var aatf = TestFixture.Build<AatfData>()
                .With(a => a.Id, id)
                .With(a => a.AatfId, aatfId)
                .With(a => a.ComplianceYear, complianceYear)
                .With(a => a.CanCreateEditEvidence, true)
                .Create();

            var aatfs = new List<AatfData>()
            {
                TestFixture.Create<AatfData>(),
                aatf
            };

            //act
            var result = aatfHelper.AatfCanEditCreateNotes(aatfs, id, complianceYear);

            //assert
            result.Should().BeTrue();
        }

        [Fact]
        public void AatfCanEditCreateNotes_GivenMatchingCriteriaInAnotherComplianceYear_TrueShouldBeReturned()
        {
            //arrange
            var id = TestFixture.Create<Guid>();
            var aatfId = TestFixture.Create<Guid>();
            var complianceYear = TestFixture.Create<int>();

            var aatf1 = TestFixture.Build<AatfData>()
                .With(a => a.Id, id)
                .With(a => a.AatfId, aatfId)
                .With(a => a.ComplianceYear, complianceYear - 1)
                .With(a => a.CanCreateEditEvidence, false)
                .Create();

            var aatf2 = TestFixture.Build<AatfData>()
                .With(a => a.Id, id)
                .With(a => a.AatfId, aatfId)
                .With(a => a.ComplianceYear, complianceYear)
                .With(a => a.CanCreateEditEvidence, true)
                .Create();

            var aatfs = new List<AatfData>()
            {
                aatf1,
                aatf2
            };

            //act
            var result = aatfHelper.AatfCanEditCreateNotes(aatfs, id, complianceYear);

            //assert
            result.Should().BeTrue();
        }

        [Fact]
        public void AatfCanEditCreateNotes_GivenNonMatchingCriteriaInAnotherComplianceYear_FalseShouldBeReturned()
        {
            //arrange
            var id = TestFixture.Create<Guid>();
            var aatfId = TestFixture.Create<Guid>();
            var complianceYear = TestFixture.Create<int>();

            var aatf1 = TestFixture.Build<AatfData>()
                .With(a => a.Id, id)
                .With(a => a.AatfId, aatfId)
                .With(a => a.ComplianceYear, complianceYear - 1)
                .With(a => a.CanCreateEditEvidence, false)
                .Create();

            var aatf2 = TestFixture.Build<AatfData>()
                .With(a => a.Id, id)
                .With(a => a.AatfId, aatfId)
                .With(a => a.ComplianceYear, complianceYear)
                .With(a => a.CanCreateEditEvidence, false)
                .Create();

            var aatfs = new List<AatfData>()
            {
                aatf1,
                aatf2
            };

            //act
            var result = aatfHelper.AatfCanEditCreateNotes(aatfs, id, complianceYear);

            //assert
            result.Should().BeFalse();
        }

        [Fact]
        public void AatfCanEditCreateNotes_GivenNonMatchingComplianceYear_FalseShouldBeReturned()
        {
            //arrange
            var id = TestFixture.Create<Guid>();
            var aatfId = TestFixture.Create<Guid>();
            var complianceYear = TestFixture.Create<int>();

            var aatf = TestFixture.Build<AatfData>()
                .With(a => a.Id, id)
                .With(a => a.AatfId, aatfId)
                .With(a => a.ComplianceYear, complianceYear + 1)
                .With(a => a.CanCreateEditEvidence, true)
                .Create();

            var aatfs = new List<AatfData>()
            {
                TestFixture.Create<AatfData>(),
                aatf
            };

            //act
            var result = aatfHelper.AatfCanEditCreateNotes(aatfs, id, complianceYear);

            //assert
            result.Should().BeFalse();
        }

        [Fact]
        public void AatfCanEditCreateNotes_GivenNonMatchingCanCreateEditEvidence_FalseShouldBeReturned()
        {
            //arrange
            var id = TestFixture.Create<Guid>();
            var aatfId = TestFixture.Create<Guid>();
            var complianceYear = TestFixture.Create<int>();

            var aatf = TestFixture.Build<AatfData>()
                .With(a => a.Id, id)
                .With(a => a.AatfId, aatfId)
                .With(a => a.ComplianceYear, complianceYear + 1)
                .With(a => a.CanCreateEditEvidence, false)
                .Create();

            var aatfs = new List<AatfData>()
            {
                TestFixture.Create<AatfData>(),
                aatf
            };

            //act
            var result = aatfHelper.AatfCanEditCreateNotes(aatfs, id, complianceYear);

            //assert
            result.Should().BeFalse();
        }
    }
}
