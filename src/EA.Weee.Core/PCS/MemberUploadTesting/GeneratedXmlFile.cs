using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EA.Weee.Core.PCS.MemberUploadTesting
{
    /// <summary>
    /// An XML file that has been generated to assist
    /// with testing of the PCS Member upload functionality.
    /// </summary>
    public class GeneratedXmlFile
    {
        public string FileName { get; private set; }

        public byte[] Data { get; private set; }

        public GeneratedXmlFile(string fileName, byte[] data)
        {
            FileName = fileName;
            Data = data;
        }
    }
}
