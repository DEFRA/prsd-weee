namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.Scheme;
    using FakeItEasy;
    using Helpers;
    using RequestHandlers.Scheme;
    using Requests.Scheme;
    using Xunit;
    using ObligationType = Domain.ObligationType;

    public class VerifySchemeNameExistsHandlerTests
    {
        private readonly DbContextHelper helper = new DbContextHelper();

        [Fact]
        public async Task VerifySchemeNameExistsHandler_SchemeNameNotExists_ReturnsFalse()
        {
            var schemes = MakeScheme();

            var context = A.Fake<WeeeContext>();

            A.CallTo(() => context.Schemes).Returns(schemes);

            var handler = new VerifySchemeNameExistsHandler(context);

            const string schemeName = "WEE/AB4444CD/SCH";

            var schemeNameExists = await handler.HandleAsync(new VerifySchemeNameExists(schemeName));

            Assert.False(schemeNameExists);
        }

        [Fact]
        public async Task VerifySchemeNameExistsHandler_SchemeNameExists_ReturnsTrue()
        {
            var schemes = MakeScheme();

            var context = A.Fake<WeeeContext>();

            A.CallTo(() => context.Schemes).Returns(schemes);

            var handler = new VerifySchemeNameExistsHandler(context);

            const string schemeName = "WEE/AB1234CD/SCH";

            var schemeNameExists = await handler.HandleAsync(new VerifySchemeNameExists(schemeName));

            Assert.True(schemeNameExists);
        }

        private DbSet<Scheme> MakeScheme()
        {
            return helper.GetAsyncEnabledDbSet(new[]
            {
                CreateScheme()
            });
        }

        private static Scheme CreateScheme()
        {
            var scheme = new Scheme(Guid.NewGuid());
            scheme.UpdateScheme("WEE/AB1234CD/SCH", "Any value", "Any value", ObligationType.B2B, Guid.NewGuid());
            return scheme;
        }
    }
}
