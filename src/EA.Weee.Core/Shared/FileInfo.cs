namespace EA.Weee.Core.Shared
{
    public class FileInfo
    {
        public string FileName { get; private set; }

        public byte[] Data { get; private set; }

        public FileInfo(string fileName, byte[] data)
        {
            FileName = fileName;
            Data = data;
        }
    }
}
