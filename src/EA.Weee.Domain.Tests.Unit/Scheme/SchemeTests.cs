namespace EA.Weee.Domain.Tests.Unit.Scheme
{
    using System;
    using Domain.Scheme;
    using Obligation;
    using Xunit;

    public class SchemeTests
    {
        private const string orgGuid = "2AE69682-E9D8-4AC5-991D-A4CF00C42F14";

        [Fact]
        public void UpdateSchemeInformation_SchemeWithBasicInformation_ReturnsUpdatedSchemeInformation()
        {
            var scheme = GetTestScheme();
            const string schemeName = "WEE/AB1234CD/SCH";
            const string approvalNumber = "Approval number";
            const string ibisCustomerReference = "Any value";
            var obligationType = ObligationType.B2B;
            var competentAuthorityId = Guid.NewGuid();

            scheme.UpdateScheme(schemeName, approvalNumber, ibisCustomerReference, obligationType, competentAuthorityId);

            Assert.Equal(scheme.SchemeName, schemeName);
            Assert.Equal(scheme.ApprovalNumber, approvalNumber);
            Assert.Equal(scheme.IbisCustomerReference, ibisCustomerReference);
            Assert.Equal(scheme.ObligationType, obligationType);
            Assert.Equal(scheme.CompetentAuthorityId, competentAuthorityId);
        }

        [Fact]
        public void UpdateSchemeStatus_ChangeFromPending_IsOk()
        {
            var scheme = GetTestScheme();
            scheme.SetStatus(SchemeStatus.Approved);

            scheme = GetTestScheme();
            scheme.SetStatus(SchemeStatus.Rejected);

            scheme = GetTestScheme();
            scheme.SetStatus(SchemeStatus.Pending);
        }

        [Fact]
        public void UpdateSchemeStatus_NoChangeOfApprovedStatus_IsOk()
        {
            var scheme = GetTestScheme();
            scheme.SetStatus(SchemeStatus.Approved);
            scheme.SetStatus(SchemeStatus.Approved);
        }

        [Fact]
        public void UpdateSchemeStatus_NoChangeOfRejectedStatus_IsOk()
        {
            var scheme = GetTestScheme();
            scheme.SetStatus(SchemeStatus.Rejected);
            scheme.SetStatus(SchemeStatus.Rejected);
        }

        [Fact]
        public void UpdateSchemeStatus_NoChangeOfWithdrawnStatus_IsOk()
        {
            var scheme = GetTestScheme();
            scheme.SetStatus(SchemeStatus.Approved);
            scheme.SetStatus(SchemeStatus.Withdrawn);
            scheme.SetStatus(SchemeStatus.Withdrawn);
        }

        [Fact]
        public void UpdateSchemeStatus_ApprovedToWithdrawnStatus_IsOk()
        {
            var scheme = GetTestScheme();
            scheme.SetStatus(SchemeStatus.Approved);
            scheme.SetStatus(SchemeStatus.Withdrawn);
        }

        [Fact]
        public void UpdateSchemeStatus_WithdrawnToSomethingElse_ThrowsInvalidOperationException()
        {
            var scheme = GetTestScheme();
            scheme.SetStatus(SchemeStatus.Approved);
            scheme.SetStatus(SchemeStatus.Withdrawn);

            Assert.Throws<InvalidOperationException>(() => scheme.SetStatus(SchemeStatus.Pending));
            Assert.Throws<InvalidOperationException>(() => scheme.SetStatus(SchemeStatus.Rejected));
            Assert.Throws<InvalidOperationException>(() => scheme.SetStatus(SchemeStatus.Approved));
        }

        [Fact]
        public void UpdateSchemeStatus_RejectedToSomethingElse_ThrowsInvalidOperationException()
        {
            var scheme = GetTestScheme();
            scheme.SetStatus(SchemeStatus.Rejected);

            Assert.Throws<InvalidOperationException>(() => scheme.SetStatus(SchemeStatus.Pending));
            Assert.Throws<InvalidOperationException>(() => scheme.SetStatus(SchemeStatus.Approved));
        }

        [Fact]
        public void UpdateSchemeStatus_PendingToWithdrawn_ThrowsInvalidOperationException()
        {
            var scheme = GetTestScheme();

            Assert.Throws<InvalidOperationException>(() => scheme.SetStatus(SchemeStatus.Withdrawn));
        }

        private static Scheme GetTestScheme()
        {
            var orgId = new Guid(orgGuid);
            var scheme = new Scheme(orgId);
            return scheme;
        }
    }
}
