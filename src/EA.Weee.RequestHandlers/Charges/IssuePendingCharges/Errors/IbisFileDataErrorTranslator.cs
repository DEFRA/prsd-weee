namespace EA.Weee.RequestHandlers.Charges.IssuePendingCharges.Errors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class IbisFileDataErrorTranslator : IIbisFileDataErrorTranslator
    {
        public List<string> MakeFriendlyErrorMessages(List<Exception> errors)
        {
            var translatedErrors = new List<string>();

            foreach (var error in errors)
            {
                var schemeDetailsException = error as SchemeFieldException;
                if (schemeDetailsException == null)
                {
                    ThrowUnableToTranslateError(error);
                }
                else
                {
                    translatedErrors.Add(TranslateSchemeFieldException(schemeDetailsException));
                }
            }

            return translatedErrors.Distinct().ToList();
        }

        private string TranslateSchemeFieldException(SchemeFieldException schemeFieldException)
        {
            string translatedMessage = null;

            switch (schemeFieldException.Exception.Message)
            {
                case "The post code is mandatory.":
                    translatedMessage = string.Format("PCS {0} is missing an organisation contact postcode.",
                        schemeFieldException.Scheme.SchemeName);
                    break;
                case "The customer reference is mandatory.":
                    translatedMessage = string.Format("PCS {0} is missing a billing customer reference.",
                        schemeFieldException.Scheme.SchemeName);
                    break;
                default:
                    ThrowUnableToTranslateError(schemeFieldException);
                    break;
            }

            return translatedMessage;
        }

        private void ThrowUnableToTranslateError(Exception exception)
        {
            throw new Exception("Unable to translate an error that occurred while generating Ibis files. See inner exception for details", exception);
        }
    }
}
