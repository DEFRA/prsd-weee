namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using System.Collections.Generic;
    using CuttingEdge.Conditions;
    using Prsd.Core.Mapper;
    using System.Linq;
    using Core.Admin.Obligation;
    using ViewModels.Obligations;

    public class UploadObligationsViewModelMap : IMap<UploadObligationsViewModelMapTransfer, UploadObligationsViewModel>
    {
        public UploadObligationsViewModel Map(UploadObligationsViewModelMapTransfer source)
        {
            Condition.Requires(source).IsNotNull();

            var model = new UploadObligationsViewModel(source.CompetentAuthority);
            var dataErrorTypes = new List<SchemeObligationUploadErrorType>() { SchemeObligationUploadErrorType.Data, SchemeObligationUploadErrorType.Scheme };

            var displayDataError = false;
            var displayFileError = false;

            if (source.UploadData != null)
            {
                displayDataError = source.UploadData.ErrorData.Any(r => dataErrorTypes.Contains(r.ErrorType));
                displayFileError =
                    source.UploadData.ErrorData.Any(r => r.ErrorType == SchemeObligationUploadErrorType.File);
            }

            model.DisplayFormatError = displayFileError;
            model.DisplayDataError = displayFileError != true && displayDataError;
            
            return model;
        }
    }
}