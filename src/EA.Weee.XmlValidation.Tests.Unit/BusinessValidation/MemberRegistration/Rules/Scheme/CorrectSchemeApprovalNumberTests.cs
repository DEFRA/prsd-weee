namespace EA.Weee.XmlValidation.Tests.Unit.BusinessValidation.MemberRegistration.Rules.Scheme
{
    using FakeItEasy;
    using System;
    using Xml.MemberRegistration;
    using XmlValidation.BusinessValidation.MemberRegistration.QuerySets;
    using XmlValidation.BusinessValidation.MemberRegistration.Rules.Scheme;
    using Xunit;

    public class CorrectSchemeApprovalNumberTests
    {
        private readonly ISchemeQuerySet schemeQuerySet;

        public CorrectSchemeApprovalNumberTests()
        {
            schemeQuerySet = A.Fake<ISchemeQuerySet>();
        }

        [Theory]
        [InlineData("ApprovalNo1", "ApprovalNo1")]
        [InlineData("ApprovalNo1", "approvalno1")]
        public void SchemeApprovalNumber_MatchesApprovalNumberinXML_ValidationSucceeds(string approvalNumber, string existingApprovalNumber)
        {
            var schemeId = Guid.NewGuid();
            var scheme = new schemeType
            {
                approvalNo = approvalNumber,
            };

            A.CallTo(() => schemeQuerySet.GetSchemeApprovalNumber(schemeId))
                .Returns(existingApprovalNumber);

            var result = Rule().Evaluate(scheme, schemeId);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void SchemeApprovalNumber_DoesNotMatchApprovalNumberinXML_ValidationFails_AndIncludesApprovalNumberInMessage()
        {
            var schemeId = Guid.NewGuid();
            const string approvalNumber = "Test Approval Number 1";
            const string nonMatchingApprovalNumber = "Test Approval Number 2";
            var scheme = new schemeType
            {
                approvalNo = approvalNumber,
            };

            A.CallTo(() => schemeQuerySet.GetSchemeApprovalNumber(schemeId))
                .Returns(nonMatchingApprovalNumber);

            var result = Rule().Evaluate(scheme, schemeId);

            Assert.False(result.IsValid);
            Assert.Contains(approvalNumber, result.Message);
        }

        private CorrectSchemeApprovalNumber Rule()
        {
            return new CorrectSchemeApprovalNumber(schemeQuerySet);
        }
    }
}
