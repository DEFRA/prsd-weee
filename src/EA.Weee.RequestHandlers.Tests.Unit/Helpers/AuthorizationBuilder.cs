namespace EA.Weee.RequestHandlers.Tests.Unit.Helpers
{
    using FakeItEasy;
    using System;
    using System.Security;
    using RequestHandlers.Security;

    /// <summary>
    /// This helper class creates a fake instance of an IWeeeAutorization
    /// which can be used in unit test.
    /// </summary>
    /// <example>
    /// Example usage:
    /// 
    ///     IWeeeAuthorization authorization = new AuthorizationBuilder()
    ///         .AllowInternalAreaAccess()
    ///         .DenyOrganisationAccess()
    ///         .Build();
    /// </example>
    internal class AuthorizationBuilder
    {
        private IWeeeAuthorization fake;

        public AuthorizationBuilder()
        {
            fake = A.Fake<IWeeeAuthorization>();
        }

        public AuthorizationBuilder AllowInternalAreaAccess()
        {
            A.CallTo(() => fake.EnsureCanAccessInternalArea()).DoesNothing();
            A.CallTo(() => fake.CheckCanAccessInternalArea()).Returns(true);
            return this;
        }

        public AuthorizationBuilder DenyInternalAreaAccess()
        {
            A.CallTo(() => fake.EnsureCanAccessInternalArea()).Throws<SecurityException>();
            A.CallTo(() => fake.CheckCanAccessInternalArea()).Returns(false);
            return this;
        }

        public AuthorizationBuilder AllowExternalAreaAccess()
        {
            A.CallTo(() => fake.EnsureCanAccessExternalArea()).DoesNothing();
            A.CallTo(() => fake.CheckCanAccessExternalArea()).Returns(true);
            return this;
        }

        public AuthorizationBuilder DenyExternalAreaAccess()
        {
            A.CallTo(() => fake.EnsureCanAccessExternalArea()).Throws<SecurityException>();
            A.CallTo(() => fake.CheckCanAccessExternalArea()).Returns(false);
            return this;
        }

        public AuthorizationBuilder AllowOrganisationAccess()
        {
            A.CallTo(() => fake.EnsureOrganisationAccess(A.Dummy<Guid>())).WithAnyArguments().DoesNothing();
            A.CallTo(() => fake.CheckOrganisationAccess(A.Dummy<Guid>())).WithAnyArguments().Returns(true);
            return this;
        }

        public AuthorizationBuilder DenyOrganisationAccess()
        {
            A.CallTo(() => fake.EnsureOrganisationAccess(A.Dummy<Guid>())).WithAnyArguments().Throws<SecurityException>();
            A.CallTo(() => fake.CheckOrganisationAccess(A.Dummy<Guid>())).WithAnyArguments().Returns(false);
            return this;
        }

        public AuthorizationBuilder AllowSchemeAccess()
        {
            A.CallTo(() => fake.EnsureSchemeAccess(A.Dummy<Guid>())).WithAnyArguments().DoesNothing();
            A.CallTo(() => fake.CheckSchemeAccess(A.Dummy<Guid>())).WithAnyArguments().Returns(true);
            return this;
        }

        public AuthorizationBuilder DenySchemeAccess()
        {
            A.CallTo(() => fake.EnsureSchemeAccess(A.Dummy<Guid>())).WithAnyArguments().Throws<SecurityException>();
            A.CallTo(() => fake.CheckSchemeAccess(A.Dummy<Guid>())).WithAnyArguments().Returns(false);
            return this;
        }

        public IWeeeAuthorization Build()
        {
            return fake;
        }
    }
}
