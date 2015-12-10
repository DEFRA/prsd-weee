namespace EA.Weee.Web.Areas.Scheme.Requests
{
    using Prsd.Core.Mapper;
    using Services;
    using ViewModels;
    using Weee.Requests.DataReturns;

    public class ProcessDataReturnXMLFileRequest : IMap<PCSFileUploadViewModel, ProcessDataReturnXMLFile>
    {
        private readonly IFileConverterService fileConverter;

        public ProcessDataReturnXMLFileRequest(IFileConverterService fileConverter)
        {
            this.fileConverter = fileConverter;
        }

        public ProcessDataReturnXMLFile Map(PCSFileUploadViewModel source)
        {
            return new ProcessDataReturnXMLFile(source.PcsId, fileConverter.Convert(source.File), source.File.FileName);
        }
    }
}