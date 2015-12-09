namespace EA.Weee.RequestHandlers.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Helpers;
    using Domain;
    using Domain.DataReturns;
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

        /// <summary>
        /// An implementation of the Fisher-Yates shuffle algorithm.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IList<TSource> Shuffle<TSource>(this IList<TSource> list)
        {
            Random r = new Random();

            List<TSource> results = new List<TSource>();
            List<int> indexes = Enumerable.Range(0, list.Count).ToList();

            while (indexes.Count > 0)
            {
                int n = r.Next(indexes.Count);
                int index = indexes[n];
                indexes.Remove(index);

                results.Add(list[index]);
            }

            return results;
        }
    }
}
