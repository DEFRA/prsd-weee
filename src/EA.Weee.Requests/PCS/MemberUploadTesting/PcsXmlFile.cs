namespace EA.Weee.Requests.PCS.MemberUploadTesting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

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
