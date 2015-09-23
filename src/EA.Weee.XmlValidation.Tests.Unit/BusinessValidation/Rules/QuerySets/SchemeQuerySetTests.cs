namespace EA.Weee.XmlValidation.Tests.Unit.BusinessValidation.Rules.QuerySets
{
    using System;
    using System.Collections.Generic;
    using DataAccess;
    using FakeItEasy;
    using Weee.Domain.Scheme;
    using Weee.Tests.Core;
    using XmlValidation.BusinessValidation.QuerySets;
    using Xunit;

    public class SchemeQuerySetTests
    {
        private readonly WeeeContext context;
        private readonly DbContextHelper helper;

        public SchemeQuerySetTests()
        {
            context = A.Fake<WeeeContext>();
            helper = new DbContextHelper();

            // By default, context returns no schemes
            A.CallTo(() => context.Schemes)
                .Returns(helper.GetAsyncEnabledDbSet(new List<Scheme>()));
        }

        [Fact]
        public void GetSchemeApprovalNumber_SchemeIdDoesNotMatch_ReturnsNull()
        {
            var schemeId = Guid.NewGuid();
            var approvalNumber = "WEE/ZZ1234AA/SCH";

            A.CallTo(() => context.Schemes)
                .Returns(helper.GetAsyncEnabledDbSet(new List<Scheme>
                {
                    SchemeWithApprovalNumber(approvalNumber)
                }));

            var result = SchemeQuerySet().GetSchemeApprovalNumber(schemeId);

            Assert.Null(result);
        }

        [Fact]
        public void GetSchemeApprovalNumber_SchemeIdDoesMatch_ReturnsApprovalNumber()
        {
            var schemeId = Guid.Empty; // This will match because Id is not set against existing scheme
            var approvalNumber = "WEE/ZZ1234AA/SCH";

            A.CallTo(() => context.Schemes)
                .Returns(helper.GetAsyncEnabledDbSet(new List<Scheme>
                {
                    SchemeWithApprovalNumber(approvalNumber)
                }));

            var result = SchemeQuerySet().GetSchemeApprovalNumber(schemeId);

            Assert.Equal(approvalNumber, result);
        }

        private SchemeQuerySet SchemeQuerySet()
        {
            return new SchemeQuerySet(context);
        }

        private Scheme SchemeWithApprovalNumber(string approvalNumber)
        {
            var scheme = A.Fake<Scheme>();

            A.CallTo(() => scheme.ApprovalNumber)
                .Returns(approvalNumber);

            return scheme;
        }
    }
}
