namespace EA.Weee.Tests.Core
{
    using System;
    using System.Security;
    using FakeItEasy;
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
    public class AuthorizationBuilder
    {
        public enum UserType
        {
            Unauthenticated,
            External,
            Internal,
        }

        /// <summary>
        /// Create authorization for a user of a specific type. This method can be used
        /// in conjuection with the [Theory] and [InlineData] attribute to create XUnit
        /// tests that check security rules for multiple user types.
        /// </summary>
        /// <param name="userType"></param>
        /// <returns></returns>
        public static IWeeeAuthorization CreateFromUserType(UserType userType)
        {
            switch (userType)
            {
                case UserType.Unauthenticated:
                    return CreateUserWithNoRights();

                case UserType.External:
                    return new AuthorizationBuilder()
                        .AllowExternalAreaAccess()
                        .DenyInternalAreaAccess()
                        .DenyOrganisationAccess()
                        .DenyInternalOrOrganisationAccess()
                        .Build();

                case UserType.Internal:
                    return new AuthorizationBuilder()
                        .DenyExternalAreaAccess()
                        .AllowInternalAreaAccess()
                        .DenyOrganisationAccess()
                        .AllowInternalOrOrganisationAccess()
                        .Build();

                default:
                    throw new NotSupportedException();
            }
        }

        public static IWeeeAuthorization CreateUserWithAllRights()
        {
            return new AuthorizationBuilder().AllowEverything().Build();
        }

        public static IWeeeAuthorization CreateUserWithNoRights()
        {
            return new AuthorizationBuilder().DenyEverything().Build();
        }

        /// <summary>
        /// Create authorization for a user that can access an organisation, but does not have internal access.
        /// </summary>
        public static IWeeeAuthorization CreateUserAllowedToAccessOrganisation()
        {
            return new AuthorizationBuilder()
                .AllowOrganisationAccess()
                .AllowInternalOrOrganisationAccess()
                .DenyInternalAreaAccess()
                .DenyExternalAreaAccess()
                .Build();
        }

        /// <summary>
        /// Create authorization for a user that cannot access an organisation and does not have internal access.
        /// </summary>
        public static IWeeeAuthorization CreateUserDeniedFromAccessingOrganisation()
        {
            return new AuthorizationBuilder()
                .DenyOrganisationAccess()
                .DenyInternalOrOrganisationAccess()
                .DenyInternalAreaAccess()
                .DenyExternalAreaAccess()
                .Build();
        }

        private IWeeeAuthorization fake;

        public AuthorizationBuilder()
        {
            fake = A.Fake<IWeeeAuthorization>();
        }

        public AuthorizationBuilder AllowEverything()
        {
            AllowInternalAreaAccess();
            AllowExternalAreaAccess();
            AllowOrganisationAccess();
            AllowSchemeAccess();
            return this;
        }

        public AuthorizationBuilder DenyEverything()
        {
            DenyInternalAreaAccess();
            DenyExternalAreaAccess();
            DenyOrganisationAccess();
            DenySchemeAccess();
            return this;
        }

        public AuthorizationBuilder AllowInternalAreaAccess()
        {
            A.CallTo(() => fake.EnsureCanAccessInternalArea()).DoesNothing();
            A.CallTo(() => fake.EnsureCanAccessInternalArea(A<bool>._)).DoesNothing();
            A.CallTo(() => fake.CheckCanAccessInternalArea()).Returns(true);
            A.CallTo(() => fake.CheckCanAccessInternalArea(A<bool>._)).Returns(true);
            return this;
        }

        public AuthorizationBuilder DenyInternalAreaAccess()
        {
            A.CallTo(() => fake.EnsureCanAccessInternalArea()).Throws<SecurityException>();
            A.CallTo(() => fake.EnsureCanAccessInternalArea(A<bool>._)).Throws<SecurityException>();
            A.CallTo(() => fake.CheckCanAccessInternalArea()).Returns(false);
            A.CallTo(() => fake.CheckCanAccessInternalArea(A<bool>._)).Returns(false);
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

        public AuthorizationBuilder AllowInternalOrOrganisationAccess()
        {
            A.CallTo(() => fake.EnsureInternalOrOrganisationAccess(A.Dummy<Guid>())).WithAnyArguments().DoesNothing();
            A.CallTo(() => fake.CheckInternalOrOrganisationAccess(A.Dummy<Guid>())).WithAnyArguments().Returns(true);
            return this;
        }

        public AuthorizationBuilder DenyInternalOrOrganisationAccess()
        {
            A.CallTo(() => fake.EnsureInternalOrOrganisationAccess(A.Dummy<Guid>())).WithAnyArguments().Throws<SecurityException>();
            A.CallTo(() => fake.CheckInternalOrOrganisationAccess(A.Dummy<Guid>())).WithAnyArguments().Returns(false);
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
