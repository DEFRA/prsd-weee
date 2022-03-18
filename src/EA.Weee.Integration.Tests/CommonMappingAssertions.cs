namespace EA.Weee.Integration.Tests
{
    using Core.Scheme;
    using Domain.Scheme;
    using FluentAssertions;

    public static class CommonMappingAssertions
    {
        public static void ShouldMapScheme(this SchemeData schemeData, Scheme scheme)
        {
            schemeData.SchemeName.Should().Be(scheme.SchemeName);
            schemeData.Address.Id.Should().Be(scheme.AddressId.Value);
            schemeData.IbisCustomerReference.Should().Be(scheme.IbisCustomerReference);
            schemeData.ApprovalName.Should().Be(scheme.ApprovalNumber);
            ((int)schemeData.ObligationType.Value).Should().Be((int)scheme.ObligationType);
            schemeData.CompetentAuthorityId.Should().Be(scheme.CompetentAuthorityId);
            schemeData.Contact.Id.Should().Be(scheme.ContactId.Value);
            schemeData.Name.Should().Be(scheme.Organisation.Name);
            schemeData.OrganisationId.Should().Be(scheme.OrganisationId);
            ((int)schemeData.SchemeStatus).Should().Be((int)scheme.SchemeStatus.Value);
        }
    }
}
