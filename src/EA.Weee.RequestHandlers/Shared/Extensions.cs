namespace EA.Weee.RequestHandlers.Shared
{
    using System;
    using Core.Helpers;
    using Domain;
    using Domain.Scheme;
    using Weee.XmlValidation.Errors;

    public static class Extensions
    {
        public static MemberUploadError ToMemberUploadError(this XmlValidationError error)
        {
            return new MemberUploadError(error.ErrorLevel.ToDomainEnumeration<ErrorLevel>(),
                error.ErrorType.ToUploadErrorType(), error.Message,
                error.LineNumber.HasValue ? error.LineNumber.Value : 0);
        }

        public static DataReturnsUploadError ToDataReturnsUploadError(this XmlValidationError error)
        {
            return new DataReturnsUploadError(error.ErrorLevel.ToDomainEnumeration<ErrorLevel>(),
                error.ErrorType.ToUploadErrorType(), error.Message,
                error.LineNumber.HasValue ? error.LineNumber.Value : 0);
        }

        public static UploadErrorType ToUploadErrorType(this XmlErrorType errorType)
        {
            switch (errorType)
            {
                case XmlErrorType.Business:
                    return UploadErrorType.Business;
                case XmlErrorType.Schema:
                    return UploadErrorType.Schema;
                default:
                    throw new NotImplementedException(string.Format("Unknown error type '{0}'", errorType));
            }
        }
    }
}
