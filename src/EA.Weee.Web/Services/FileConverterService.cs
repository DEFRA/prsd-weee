﻿namespace EA.Weee.Web.Services
{
    using System.IO;
    using System.Web;

    public class FileConverterService : IFileConverterService
    {
        public byte[] Convert(HttpPostedFileBase file)
        {
            var reader = new BinaryReader(file.InputStream);
            return reader.ReadBytes((int)file.InputStream.Length);
        }

        public byte[] ConvertCsv(HttpPostedFileBase file)
        {
            var reader = new StreamReader(file.InputStream);
            return System.Text.Encoding.UTF8.GetBytes(reader.ReadToEnd());
        }
    }
}