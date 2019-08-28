namespace EA.Prsd.Core.Tests.Validation
{
    using Core.Validation;
    using Stubs;
    using Xunit;

    public class HttpPostedFileTypeAttributeTests  
    {
        [Fact]
        public void HttpPostedFileIsNull_IsInvalid()
        {
            var httpPostedFileValidationAttribute = new HttpPostedFileTypeAttribute("text/xml");

            Assert.False(httpPostedFileValidationAttribute.IsValid(null));
        }

        [Theory]
        [InlineData("text/xml", "TEXT/XML")]
        [InlineData("TEXT/XML", "text/xml")]
        public void CasesOfFileTypesDoNotMatch_IsValid(string allowedFileType, string submittedFileType)
        {
            var httpPostedFile = new StubbedHttpPostedFile(submittedFileType);
            var httpPostedFileValidationAttribute = new HttpPostedFileTypeAttribute(allowedFileType);

            Assert.True(httpPostedFileValidationAttribute.IsValid(httpPostedFile));
        }

        [Theory]
        [InlineData("text/xml", "")]
        [InlineData("text/xml", null)]
        public void SubmittedFileTypeIsNullOrEmpty_IsInvalid(string allowedFileType, string submittedFileType)
        {
            var httpPostedFile = new StubbedHttpPostedFile(submittedFileType);
            var httpPostedFileValidationAttribute = new HttpPostedFileTypeAttribute(allowedFileType);

            Assert.False(httpPostedFileValidationAttribute.IsValid(httpPostedFile));
        }

        [Theory]
        [InlineData("text/xml")]
        [InlineData("AnyFileType")]
        [InlineData("")]
        [InlineData(null)]
        public void NoAllowedFileTypes_AnySubmittedFileTypeIsInvalid(string submittedFileType)
        {
            var httpPostedFile = new StubbedHttpPostedFile(submittedFileType);
            var httpPostedFileValidationAttribute = new HttpPostedFileTypeAttribute();

            Assert.False(httpPostedFileValidationAttribute.IsValid(httpPostedFile));
        }
    }
}
