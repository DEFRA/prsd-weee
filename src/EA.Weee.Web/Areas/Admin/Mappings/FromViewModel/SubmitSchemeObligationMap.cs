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

            return new SubmitSchemeObligation(new FileInfo(source.File.FileName, fileConverter.ConvertCsv(source.File)),
                source.Authority);
        }
    }
}