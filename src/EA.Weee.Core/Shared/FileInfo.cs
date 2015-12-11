namespace EA.Weee.Core.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

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
