namespace EA.Weee.RequestHandlers.Admin.Obligations
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;
    using Core.Shared.CsvReading; 
    using Core.Validation;
    using DataAccess.DataAccess;
    using Domain.Error;
    using Domain.Lookup;
    using Domain.Obligation;

    public class ObligationUploadValidator : IObligationUploadValidator
    {
        private readonly ISchemeDataAccess schemeDataAccess;

        public ObligationUploadValidator(ISchemeDataAccess schemeDataAccess)
        {
            this.schemeDataAccess = schemeDataAccess;
        }

        public async Task<IList<ObligationUploadError>> Validate(IList<ObligationCsvUpload> obligations)
        {
            //TODO validate PCS belongs to AA
            var validationErrors = new List<ObligationUploadError>();
            
            foreach (var obligationCsvUpload in obligations)
            {
                var findScheme =
                    await schemeDataAccess.GetSchemeOrDefaultByApprovalNumber(
                        obligationCsvUpload.SchemeIdentifier.Trim());

                if (findScheme == null)
                {
                    validationErrors.Add(new ObligationUploadError(ObligationUploadErrorType.Scheme, obligationCsvUpload.SchemeName, obligationCsvUpload.SchemeIdentifier, $"Scheme with identifier {obligationCsvUpload.SchemeIdentifier} not recognised"));
                }

                ValidateTonnages(obligationCsvUpload, validationErrors);
            }

            return validationErrors;
        }

        private void ValidateTonnages(ObligationCsvUpload obligationCsvUpload, ICollection<ObligationUploadError> errors)
        {
            var props = typeof(ObligationCsvUpload).GetProperties();
            foreach (var prop in props)
            {
                var attrs = prop.GetCustomAttributes(true);
                foreach (var attr in attrs)
                {
                    if (attr is WeeeCategoryAttribute authAttr)
                    {
                        var validationError = ValidateTonnage(prop.GetValue(obligationCsvUpload).ToString(), obligationCsvUpload.SchemeName,
                            obligationCsvUpload.SchemeIdentifier, authAttr.Category);

                        if (validationError != null)
                        {
                            errors.Add(validationError);
                        }
                    }
                }
            }
        }

        private ObligationUploadError ValidateTonnage(string tonnage, 
            string schemeName,
            string schemeIdentifier,
            WeeeCategory category)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tonnage))
                {
                    return null;
                }

                if (tonnage.WeeeDecimalLength())
                {
                    throw new ValidationException($"Category {category} is too long");
                }

                if (!tonnage.WeeeDecimal(out var decimalResult))
                {
                    throw new ValidationException($"Category {category} is wrong");
                }

                if (tonnage.WeeeNegativeDecimal(decimalResult))
                {
                    throw new ValidationException($"Category {category} is a negative value");
                }

                if (!tonnage.WeeeDecimalWithWhiteSpace(out var decimalResult1))
                {
                    throw new ValidationException($"Category {category} is wrong");
                }

                if (decimalResult1.WeeeDecimalThreePlaces())
                {
                    throw new ValidationException($"Category {category} exceeds decimal place limit");
                }

                if (tonnage.WeeeThousandSeparator())
                {
                    throw new ValidationException($"Category {category} is wrong");
                }
            }
            catch (ValidationException exception)
            {
                return new ObligationUploadError(ObligationUploadErrorType.Data, category, schemeIdentifier, schemeName,
                    exception.Message);
            }

            return null;
        }
    }
}
