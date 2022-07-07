namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using Core.Admin.Obligation;
    using CuttingEdge.Conditions;
    using Prsd.Core.Mapper;
    using System.Collections.Generic;
    using System.Linq;
    using ViewModels.Obligations;

    public class UploadObligationsViewModelMap : IMap<UploadObligationsViewModelMapTransfer, UploadObligationsViewModel>
    {
        public UploadObligationsViewModel Map(UploadObligationsViewModelMapTransfer source)
        {
            Condition.Requires(source).IsNotNull();

            var model = new UploadObligationsViewModel(source.CompetentAuthority)
            {
                ComplianceYearList = source.ComplianceYears.OrderBy(c => c),
                SelectedComplianceYear = source.SelectedComplianceYear
            };

            SetErrors(source, model);

            SetSchemeObligations(source, model);

            return model;
        }

        private void SetSchemeObligations(UploadObligationsViewModelMapTransfer source, UploadObligationsViewModel model)
        {
            foreach (var schemeObligationData in source.ObligationData.OrderBy(o => o.SchemeName))
            {
                model.SchemeObligations.Add(new SchemeObligationViewModel()
                {
                    SchemeName = schemeObligationData.SchemeName,
                    UpdateDate = schemeObligationData.UpdatedDate != null ? schemeObligationData.UpdatedDate.ToString() : "-"
                });
            }
        }

        private void SetErrors(UploadObligationsViewModelMapTransfer source, UploadObligationsViewModel model)
        {
            var dataErrorTypes = new List<SchemeObligationUploadErrorType>() { SchemeObligationUploadErrorType.Data, SchemeObligationUploadErrorType.Scheme };

            var displayDataError = false;
            var displayFileError = false;

            if (source.DisplayNotification)
            {
                if (source.ErrorData != null)
                {
                    displayDataError = source.ErrorData.Any(r => dataErrorTypes.Contains(r.ErrorType));
                    displayFileError =
                        source.ErrorData.Any(r => r.ErrorType == SchemeObligationUploadErrorType.File);
                    model.NumberOfDataErrors = source.ErrorData.Count(r => dataErrorTypes.Contains(r.ErrorType));
                    model.DisplaySuccessMessage = displayDataError == false && displayFileError == false && source.DisplayNotification;
                }

                model.UploadedMessage =
                    $"You have successfully uploaded the obligations for the compliance year {source.SelectedComplianceYear}";
                model.DisplayFormatError = displayFileError && source.DisplayNotification;
                model.DisplayDataAndSchemeErrors = displayFileError != true && displayDataError && source.DisplayNotification;
            }
        }
    }
}