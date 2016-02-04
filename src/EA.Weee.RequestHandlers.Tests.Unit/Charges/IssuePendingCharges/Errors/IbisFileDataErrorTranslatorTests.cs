namespace EA.Weee.RequestHandlers.Tests.Unit.Charges.IssuePendingCharges.Errors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FakeItEasy;
    using RequestHandlers.Charges.IssuePendingCharges.Errors;
    using Xunit;
    using Scheme = Domain.Scheme.Scheme;

    public class IbisFileDataErrorTranslatorTests
    {
        [Fact]
        public void MakeFriendlyErrorMessages_TranslateAllErrors()
        {
            // Arrange
            var scheme1 = A.Fake<Scheme>();
            A.CallTo(() => scheme1.SchemeName)
                .Returns("My scheme 1 name");

            var scheme2 = A.Fake<Scheme>();
            A.CallTo(() => scheme2.SchemeName)
                .Returns("My scheme 2 name");

            var errors = new List<Exception>
            {
                new SchemeFieldException(scheme1, new Exception("The post code is mandatory.")),
                new SchemeFieldException(scheme2, new Exception("The post code is mandatory."))
            };

            var translator = new IbisFileDataErrorTranslator();

            // Act
            var result = translator.MakeFriendlyErrorMessages(errors);

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void MakeFriendlyErrorMessages_ReturnsUniqueErrorMessages()
        {
            // Arrange
            var scheme = A.Fake<Scheme>();
            A.CallTo(() => scheme.ApprovalNumber)
                .Returns("WEEE/1234");

            var errors = new List<Exception>
            {
                new SchemeFieldException(scheme, new Exception("The post code is mandatory.")),
                new SchemeFieldException(scheme, new Exception("The post code is mandatory."))
            };

            var translator = new IbisFileDataErrorTranslator();

            // Act
            var result = translator.MakeFriendlyErrorMessages(errors);

            // Assert
            Assert.Equal(1, result.Count);
        }

        [Fact]
        public void MakeFriendlyErrorMessages_ThrowsException_WhenUnableToTranslateErrorMessage()
        {
            // Arrange
            var scheme = A.Fake<Scheme>();
            A.CallTo(() => scheme.ApprovalNumber)
                .Returns("WEEE/1234");

            var errors = new List<Exception>
            {
                new SchemeFieldException(scheme, new Exception("Undefined error message"))
            };

            var translator = new IbisFileDataErrorTranslator();

            // Act, Assert
            Assert.Throws<Exception>(() => translator.MakeFriendlyErrorMessages(errors));
        }

        [Fact]
        public void MakeFriendlyErrorMessages_ForMissingSchemeOrganisationPostCode_ReturnsTranslatedErrorMessage()
        {
            // Arrange
            var scheme = A.Fake<Scheme>();
            A.CallTo(() => scheme.SchemeName)
                .Returns("My scheme name");

            var errors = new List<Exception>
            {
                new SchemeFieldException(scheme, new Exception("The post code is mandatory."))
            };

            var translator = new IbisFileDataErrorTranslator();

            // Act
            var result = translator.MakeFriendlyErrorMessages(errors);

            // Assert
            Assert.Equal("PCS My scheme name is missing an organisation contact postcode.", result.Single());
        }

        [Fact]
        public void MakeFriendlyErrorMessages_ForMissingSchemeFinanceCustomerReference_ReturnsTranslatedErrorMessage()
        {
            // Arrange
            var scheme = A.Fake<Scheme>();
            A.CallTo(() => scheme.SchemeName)
                .Returns("My scheme name");

            var errors = new List<Exception>
            {
                new SchemeFieldException(scheme, new Exception("The customer reference is mandatory."))
            };

            var translator = new IbisFileDataErrorTranslator();

            // Act
            var result = translator.MakeFriendlyErrorMessages(errors);

            // Assert
            Assert.Equal("PCS My scheme name is missing a billing reference.", result.Single());
        }
    }
}
