namespace EA.Weee.RequestHandlers.Admin.Obligations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Shared.CsvReading; 
    using Core.Validation;
    using DataAccess.DataAccess;
    using Domain;
    using Domain.Error;
    using Domain.Obligation;

    public class ObligationUploadValidator : IObligationUploadValidator
    {
        private readonly ISchemeDataAccess schemeDataAccess;
        private readonly ITonnageValueValidator tonnageValueValidator;

        public ObligationUploadValidator(ISchemeDataAccess schemeDataAccess, 
            ITonnageValueValidator tonnageValueValidator)
        {
            this.schemeDataAccess = schemeDataAccess;
            this.tonnageValueValidator = tonnageValueValidator;
        }

        public async Task<IList<ObligationUploadError>> Validate(UKCompetentAuthority authority, IList<ObligationCsvUpload> obligations)
        {
            var validationErrors = new List<ObligationUploadError>();

            if (!obligations.Any())
            {
                validationErrors.Add(new ObligationUploadError(ObligationUploadErrorType.File, "File contains no data"));
            }
            else
            {
                foreach (var obligationCsvUpload in obligations)
                {
                    var findScheme =
                        await schemeDataAccess.GetSchemeOrDefaultByApprovalNumber(
                            obligationCsvUpload.SchemeIdentifier.Trim());

                    if (findScheme == null)
                    {
                        validationErrors.Add(new ObligationUploadError(ObligationUploadErrorType.Scheme, obligationCsvUpload.SchemeName.Trim(), obligationCsvUpload.SchemeIdentifier.Trim(), $"Scheme with identifier {obligationCsvUpload.SchemeIdentifier} not recognised"));
                    }
                    else if (findScheme.CompetentAuthorityId != authority.Id)
                    {
                        validationErrors.Add(new ObligationUploadError(ObligationUploadErrorType.Scheme, obligationCsvUpload.SchemeName.Trim(), obligationCsvUpload.SchemeIdentifier.Trim(), $"Scheme with identifier {obligationCsvUpload.SchemeIdentifier} is not part of {authority.Name}"));
                    }

                    ValidateTonnages(obligationCsvUpload, validationErrors);
                }
            }
            
            return validationErrors;
        }

        private void ValidateTonnages(ObligationCsvUpload obligationCsvUpload, ICollection<ObligationUploadError> errors)
        {
            var props = typeof(ObligationCsvUpload).GetProperties();
            foreach (var prop in props)
            {
                var categoryAttributes = prop.GetCustomAttributes(true);
                foreach (var categoryAttribute in categoryAttributes)
                {
                    if (categoryAttribute is WeeeCategoryAttribute weeeCategoryAttribute)
                    {
                        var result = tonnageValueValidator.Validate(prop.GetValue(obligationCsvUpload));

                        if (result != TonnageValidationResult.Success)
                        {
                            string message = string.Empty;
                            switch (result.Type)
                            {
                                case TonnageValidationTypeEnum.MaximumDigits:
                                    message = $"Category {(int)weeeCategoryAttribute.Category} is too long";
                                    break;
                                case TonnageValidationTypeEnum.NotNumerical:
                                    message = $"Category {(int)weeeCategoryAttribute.Category} is wrong";
                                    break;
                                case TonnageValidationTypeEnum.LessThanZero:
                                    message = $"Category {(int)weeeCategoryAttribute.Category} is a negative value";
                                    break;
                                case TonnageValidationTypeEnum.DecimalPlaces:
                                    message = $"Category {(int)weeeCategoryAttribute.Category} exceeds decimal place limit";
                                    break;
                                case TonnageValidationTypeEnum.DecimalPlaceFormat:
                                    message = $"Category {(int)weeeCategoryAttribute.Category} is wrong";
                                    break;
                            }

                            errors.Add(new ObligationUploadError(ObligationUploadErrorType.Data, weeeCategoryAttribute.Category, obligationCsvUpload.SchemeName.Trim(), obligationCsvUpload.SchemeIdentifier.Trim(), message));
                        }
                    }
                }
            }
        }
    }
}
