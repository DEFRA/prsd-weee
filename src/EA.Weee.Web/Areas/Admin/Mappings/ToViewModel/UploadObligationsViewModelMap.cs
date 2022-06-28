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

            var model = new UploadObligationsViewModel(source.CompetentAuthority);

            //TODO: Take the ComplianceYears property of the UploadObligationsViewModelMapTransfer source and create the ComplianceYearList on UploadObligationsViewModel. Also need unit tests for these properties
            
            model.ComplianceYearList = new List<int>() { 2022, 2021 };
            model.SelectedComplianceYear = source.SelectedComplianceYear;
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

            if (source.ErrorData != null)
            {
                displayDataError = source.ErrorData.Any(r => dataErrorTypes.Contains(r.ErrorType));
                displayFileError =
                    source.ErrorData.Any(r => r.ErrorType == SchemeObligationUploadErrorType.File);
                model.NumberOfDataErrors = source.ErrorData.Count(r => dataErrorTypes.Contains(r.ErrorType));
                model.DisplaySuccessMessage = displayDataError == false && displayFileError == false;
            }

            model.DisplayFormatError = displayFileError;
            model.DisplayDataError = displayFileError != true && displayDataError;
        }
    }
}