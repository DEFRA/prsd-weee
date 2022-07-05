namespace EA.Weee.Web.Areas.Admin.Mappings.FromViewModel
{
    using Core.Shared;
    using CuttingEdge.Conditions;
    using Prsd.Core.Mapper;
    using Services;
    using ViewModels.Obligations;
    using Weee.Requests.Admin.Obligations;

    public class SubmitSchemeObligationMap : IMap<UploadObligationsViewModel, SubmitSchemeObligation>
    {
        private readonly IFileConverterService fileConverter;

        public SubmitSchemeObligationMap(IFileConverterService fileConverter)
        {
            this.fileConverter = fileConverter;
        }

        public SubmitSchemeObligation Map(UploadObligationsViewModel source)
        {
            Condition.Requires(source).IsNotNull();
            Condition.Requires(source.SelectedComplianceYear).IsNotNull("SubmitSchemeObligationMap selected compliance year cannot be null");

            return new SubmitSchemeObligation(new FileInfo(source.File.FileName, fileConverter.ConvertCsv(source.File)),
                source.Authority, source.SelectedComplianceYear.Value);
        }
    }
}