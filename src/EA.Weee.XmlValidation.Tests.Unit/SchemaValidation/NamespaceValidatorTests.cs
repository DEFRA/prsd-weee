namespace EA.Weee.XmlValidation.Tests.Unit.SchemaValidation
{
    using System;
    using System.Linq;
    using Core.Shared;
    using Xml;
    using XmlValidation.SchemaValidation;
    using Xunit;

    public class NamespaceValidatorTests
    {
        [Fact]
        public void ExpectedNamespaceIsNotValid_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(
                () => new NamespaceValidator().Validate("Some invalid namespace", "Any namespace"));
        }

        [Theory]
        [InlineData(XmlNamespace.MemberRegistration)]
        [InlineData(XmlNamespace.DataReturns)]
        public void ActualNamespaceIsNotOneOfTheSystemsXmlNamespaces_ShouldReturnSchemaError_IncludingExpectedNamespaceInMessage(string expectedNamespace)
        {
            var result = new NamespaceValidator().Validate(expectedNamespace, "Any old rubbish");

            var error = result.Single();

            Assert.Equal(ErrorLevel.Error, error.ErrorLevel);
            Assert.Contains(expectedNamespace, error.Message);
        }

        [Theory]
        [InlineData(XmlNamespace.MemberRegistration, XmlNamespace.DataReturns)]
        [InlineData(XmlNamespace.DataReturns, XmlNamespace.MemberRegistration)]
        public void ActualNamespaceIsOneOfTheSystemsXmlNamespaces_ButNotTheExpectedNamespace_ShouldReturnSchemaError(string expectedNamespace, string actualNamespace)
        {
            var result = new NamespaceValidator().Validate(expectedNamespace, actualNamespace);

            Assert.Single(result);
        }

        [Theory]
        [InlineData(XmlNamespace.MemberRegistration, XmlNamespace.MemberRegistration)]
        [InlineData(XmlNamespace.DataReturns, XmlNamespace.DataReturns)]
        public void ActualNamespaceAndExpectedNamespaceMatch_NoErrorsReturned(string expectedNamespace, string actualNamespace)
        {
            var result = new NamespaceValidator().Validate(expectedNamespace, actualNamespace);

            Assert.Empty(result);
        }
    }
}
