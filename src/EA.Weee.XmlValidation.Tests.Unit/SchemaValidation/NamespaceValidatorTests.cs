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

        [Fact]
        public void ActualNamespaceIsNotOneOfTheSystemsXmlNamespaces_ShouldReturnSchemaError_IncludingExpectedNamespaceInMessage()
        {
            var result = new NamespaceValidator().Validate(XmlNamespace.MemberRegistration, "Any old rubbish");

            var error = result.Single();

            Assert.Equal(ErrorLevel.Error, error.ErrorLevel);
            Assert.Contains(XmlNamespace.MemberRegistration.NamespaceName, error.Message);
        }

        [Fact]
        public void ActualNamespaceIsOneOfTheSystemsXmlNamespaces_ButNotTheExpectedNamespace_ShouldReturnSchemaError()
        {
            var result = new NamespaceValidator().Validate(XmlNamespace.MemberRegistration, XmlNamespace.DataReturns);

            Assert.Single(result);
        }

        [Fact]
        public void ActualNamespaceAndExpectedNamespaceMatch_NoErrorsReturned()
        {
            var result = new NamespaceValidator().Validate(XmlNamespace.MemberRegistration, XmlNamespace.MemberRegistration);

            Assert.Empty(result);
        }
    }
}
