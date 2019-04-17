namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme.MemberRegistration
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using DataAccess;
    using Domain.Scheme;
    using FakeItEasy;
    using RequestHandlers.Scheme.MemberRegistration;
    using Weee.Tests.Core;
    using Xunit;

    public class TotalChargeCalculatorDataAccessTests
    {
        private readonly DbContextHelper helper = new DbContextHelper();
        private readonly TotalChargeCalculatorDataAccess dataAccess;
        private readonly WeeeContext context;

        public TotalChargeCalculatorDataAccessTests()
        {
            context = A.Fake<WeeeContext>();

            dataAccess = new TotalChargeCalculatorDataAccess(context);
        }

        [Fact]
        public void CheckSchemeHasAnnualCharge_GivenValidCriteria_ShouldBeTrue()
        {
            var schemeId = Guid.NewGuid();
            var scheme = A.Fake<Scheme>();
            A.CallTo(() => scheme.OrganisationId).Returns(schemeId);
            var memberUpload = MemberUpload(scheme);

            var list = new List<MemberUpload>()
            {
                memberUpload
            };

            var memberUploads = helper.GetAsyncEnabledDbSet(list);
            A.CallTo(() => context.MemberUploads).Returns(memberUploads);

            var result = dataAccess.CheckSchemeHasAnnualCharge(scheme, 2019);

            Assert.Equal(result, true);
        }

        [Fact]
        public void CheckSchemeHasAnnualCharge_GivenSchemeCriteriaDoesNotMatch_ShouldBeFalse()
        {
            var schemeId = Guid.NewGuid();
            var scheme = A.Fake<Scheme>();
            A.CallTo(() => scheme.OrganisationId).Returns(schemeId);
            var memberUpload = MemberUpload(scheme);
            A.CallTo(() => memberUpload.Scheme).Returns(A.Fake<Scheme>());

            var list = new List<MemberUpload>()
            {
                memberUpload
            };

            var memberUploads = helper.GetAsyncEnabledDbSet(list);
            A.CallTo(() => context.MemberUploads).Returns(memberUploads);

            var result = dataAccess.CheckSchemeHasAnnualCharge(scheme, 2019);

            Assert.Equal(result, false);
        }

        [Fact]
        public void CheckSchemeHasAnnualCharge_GivenSubmittedCriteriaDoesNotMatch_ShouldBeFalse()
        {
            var schemeId = Guid.NewGuid();
            var scheme = A.Fake<Scheme>();
            A.CallTo(() => scheme.OrganisationId).Returns(schemeId);
            var memberUpload = MemberUpload(scheme);
            A.CallTo(() => memberUpload.IsSubmitted).Returns(false);

            var list = new List<MemberUpload>()
            {
                memberUpload
            };

            var memberUploads = helper.GetAsyncEnabledDbSet(list);
            A.CallTo(() => context.MemberUploads).Returns(memberUploads);

            var result = dataAccess.CheckSchemeHasAnnualCharge(scheme, 2019);

            Assert.Equal(result, false);
        }

        [Fact]
        public void CheckSchemeHasAnnualCharge_GivenAnnualChargeCriteriaDoesNotMatch_ShouldBeFalse()
        {
            var schemeId = Guid.NewGuid();
            var scheme = A.Fake<Scheme>();
            A.CallTo(() => scheme.OrganisationId).Returns(schemeId);
            var memberUpload = MemberUpload(scheme);
            A.CallTo(() => memberUpload.HasAnnualCharge).Returns(false);

            var list = new List<MemberUpload>()
            {
                memberUpload
            };

            var memberUploads = helper.GetAsyncEnabledDbSet(list);
            A.CallTo(() => context.MemberUploads).Returns(memberUploads);

            var result = dataAccess.CheckSchemeHasAnnualCharge(scheme, 2019);

            Assert.Equal(result, false);
        }

        [Fact]
        public void CheckSchemeHasAnnualCharge_GivenComplianceYearCriteriaDoesNotMatch_ShouldBeFalse()
        {
            var schemeId = Guid.NewGuid();
            var scheme = A.Fake<Scheme>();
            A.CallTo(() => scheme.OrganisationId).Returns(schemeId);
            var memberUpload = MemberUpload(scheme);
            A.CallTo(() => memberUpload.ComplianceYear).Returns(2018);

            var list = new List<MemberUpload>()
            {
                memberUpload
            };

            var memberUploads = helper.GetAsyncEnabledDbSet(list);
            A.CallTo(() => context.MemberUploads).Returns(memberUploads);

            var result = dataAccess.CheckSchemeHasAnnualCharge(scheme, 2019);

            Assert.Equal(result, false);
        }

        private MemberUpload MemberUpload(Scheme scheme)
        {
            var memberUpload = A.Fake<MemberUpload>();
            A.CallTo(() => memberUpload.Scheme).Returns(scheme);
            A.CallTo(() => memberUpload.ComplianceYear).Returns(2019);
            A.CallTo(() => memberUpload.IsSubmitted).Returns(true);
            A.CallTo(() => memberUpload.HasAnnualCharge).Returns(true);
            return memberUpload;
        }
    }
}
