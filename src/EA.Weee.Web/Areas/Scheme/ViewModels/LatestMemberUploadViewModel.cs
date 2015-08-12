namespace EA.Weee.Web.Areas.PCS.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Helpers;

    public class LatestMemberUploadViewModel
    {
        private readonly string[] fileSizeSuffixes =
        {
            "bytes",
            "kb",
            "Mb",
            "Gb"
        };

        public int ComplianceYear { get; set; }

        public Guid UploadId { get; set; }

        public double CsvFileSizeEstimate { get; set; }

        public string ScaledCsvFileSizeSuffix 
        {
            get
            {
                var value = CsvFileSizeEstimate;
                for (var i = 0; i < fileSizeSuffixes.Length; i++)
                {
                    if (value < 100)
                    {
                        return fileSizeSuffixes[i];
                    }

                    value = value / 1024;
                }

                return fileSizeSuffixes[fileSizeSuffixes.Length - 1];
            }
        }

        public string ScaledCsvFileSize 
        {
            get
            {
                var scale = Array.IndexOf(fileSizeSuffixes, ScaledCsvFileSizeSuffix);
                var scaledSize = CsvFileSizeEstimate / Math.Pow(1024, scale);
                return scaledSize.ToStringWithXSignificantFigures(1);
            }
        }
    }
}