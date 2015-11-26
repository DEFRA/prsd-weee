namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
{
    using System;
    using System.Collections.Generic;
    using Core.Helpers;
    using Domain;
    using Domain.Scheme;
    using Weee.XmlValidation.Errors;

    public static class Extensions
    {
        public static MemberUploadError ToMemberUploadError(this XmlValidationError error)
        {
            return new MemberUploadError(error.ErrorLevel.ToDomainEnumeration<ErrorLevel>(),
                error.ErrorType.ToMemberUploadErrorType(), error.Message,
                error.LineNumber.HasValue ? error.LineNumber.Value : 0);
        }

        public static MemberUploadErrorType ToMemberUploadErrorType(this XmlErrorType errorType)
        {
            switch (errorType)
            {
                case XmlErrorType.Business:
                    return MemberUploadErrorType.Business;
                case XmlErrorType.Schema:
                    return MemberUploadErrorType.Schema;
                default:
                    throw new NotImplementedException(string.Format("Unknown error type '{0}'", errorType));
            }
        }
    }
}
