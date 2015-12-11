namespace EA.Weee.Web.Areas.Scheme.Requests
{
    using Prsd.Core.Mapper;
    using Services;
    using ViewModels;
    using Weee.Requests.DataReturns;

    public class ProcessDataReturnXmlFileRequest : IMap<PCSFileUploadViewModel, ProcessDataReturnXmlFile>
    {
        private readonly IFileConverterService fileConverter;

        public ProcessDataReturnXmlFileRequest(IFileConverterService fileConverter)
        {
            this.fileConverter = fileConverter;
        }

        public ProcessDataReturnXmlFile Map(PCSFileUploadViewModel source)
        {
            return new ProcessDataReturnXmlFile(source.PcsId, fileConverter.Convert(source.File), source.File.FileName);
        }
    }
}