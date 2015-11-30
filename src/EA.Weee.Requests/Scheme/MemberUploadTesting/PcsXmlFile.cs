namespace EA.Weee.Requests.Scheme.MemberUploadTesting
{
    /// <summary>
    /// An XML file that has been generated to assist
    /// with testing of the PCS Member upload functionality.
    /// </summary>
    public class PcsXmlFile
    {
        public string FileName { get; private set; }

        public byte[] Data { get; private set; }

        public PcsXmlFile(string fileName, byte[] data)
        {
            FileName = fileName;
            Data = data;
        }
    }
}
