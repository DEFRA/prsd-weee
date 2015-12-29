namespace EA.Weee.Ibis
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public abstract class IbisFile
    {
        /// <summary>
        /// The 3 letter code for the feeder source assigned by 1B1S income section.
        /// </summary>
        public string FileSource { get; private set; }

        /// <summary>
        /// The ID of this Ibis file.
        /// </summary>
        public ulong FileID { get; private set; }

        /// <summary>
        /// The region identifier.
        /// </summary>
        public Region RegionIdentifier { get; set; }

        /// <summary>
        /// The date that the Ibis file was created.
        /// </summary>
        public DateTime CreatedDate { get; private set; }
        
        private IIbisFormatter mFormatter;
        /// <summary>
        /// The formatter to use when writing to the Ibis file.
        /// </summary>
        public IIbisFormatter Formatter
        {
            get { return mFormatter; }
            set
            {
                if (value == null)
                {
                    throw new InvalidOperationException();
                }
                mFormatter = value;
            }
        }

        /// <summary>
        /// Abstract constructor, which all derived classes should call.
        /// Sets the system source from the configuration file, the
        /// CreatedDate to today and the Formatter to a new instance of
        /// a DefaultFormatter.
        /// </summary>
        /// <param name="fileSource">A 3 letter code for the feeder source assigned by 1B1S Income section.</param>
        /// <param name="fileID">ID of the Ibis file.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the fileID exceeds 99999.</exception>
        public IbisFile(string fileSource, ulong fileID)
        {
            if (fileSource == null || fileSource.Length != 3)
            {
                throw new ArgumentException("The file source must be exactly 3 characters.");
            }

            if (fileID < 0 || fileID > 99999)
            {
                throw new ArgumentOutOfRangeException("The file ID must be between 0 and 99999.");
            }

            FileSource = fileSource;
            FileID = fileID;
            RegionIdentifier = Region.NationalScheme;
            CreatedDate = DateTime.UtcNow.Date;
            Formatter = new DefaultFormatter();
        }

        /// <summary>
        /// Returns a string representing the contents of the Ibis file.
        /// </summary>
        /// <returns></returns>
        public string Write()
        {
            LineNumberCounter counter = new LineNumberCounter();
            StringBuilder sb = new StringBuilder();

            // Write the header line
            sb.AppendLine(WriteLine("H", counter.Next, GetHeaderDataItems()));

            // Write out each line
            foreach (IIbisFileLine line in GetLines())
            {
                sb.AppendLine(WriteLine(line.GetLineTypeIdentifier(), counter.Next, line.GetDataItems()));
            }

            // Write the footer line
            sb.AppendLine(WriteLine("T", counter.Next, GetFooterDataItems()));

            return sb.ToString();
        }

        /// <summary>
        /// Returns the file type identifier of the Ibis file.
        /// </summary>
        protected abstract string GetFileTypeIdentifier();

        /// <summary>
        /// Returns the header data items of the Ibis file.
        /// </summary>
        private IEnumerable<string> GetHeaderDataItems()
        {
            yield return FileSource;
            yield return ConvertRegionToString(RegionIdentifier);
            yield return GetFileTypeIdentifier();
            yield return FileID.ToString("D5");

            foreach (string additionalDataItem in GetAdditionalHeaderDataItems())
            {
                yield return additionalDataItem;
            }

            yield return CreatedDate.ToString("dd-MMM-yyyy").ToUpperInvariant();
        }

        /// <summary>
        /// Returns the footer data items of the Ibis file.
        /// </summary>
        private IEnumerable<string> GetFooterDataItems()
        {
            yield return GetTotalLineCount().ToString("D7");

            foreach (string additionalDataItem in GetAdditionalFooterDataItems())
            {
                yield return additionalDataItem;
            }
        }

        /// <summary>
        /// Returns file-specific additional header data items of the Ibis file.
        /// </summary>
        protected virtual IEnumerable<string> GetAdditionalHeaderDataItems()
        {
            return new List<string>();
        }

        /// <summary>
        /// Returns file-specific additional footer data items of the Ibis file.
        /// </summary>
        protected virtual IEnumerable<string> GetAdditionalFooterDataItems()
        {
            return new List<string>();
        }

        /// <summary>
        /// Returns the lines that are contained within the Ibis file.
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<IIbisFileLine> GetLines();

        /// <summary>
        /// Returns an encoded string containing the specified parameters.
        /// </summary>
        private string WriteLine(string lineTypeIdentifier, int lineNumber, IEnumerable<string> items)
        {
            List<string> allItems = new List<string>();

            allItems.Add(Formatter.Format(lineTypeIdentifier));

            allItems.Add(Formatter.Format(lineNumber.ToString("D7")));

            foreach (string item in items)
            {
                allItems.Add(Formatter.Format(item ?? string.Empty));
            }

            return string.Join(",", allItems);
        }

        /// <summary>
        /// Returns the number of lines that will be written, including the header and footer lines.
        /// </summary>
        /// <returns></returns>
        private int GetTotalLineCount()
        {
            return 1 + GetLineCount() + 1;
        }

        /// <summary>
        /// Returns the number of lines that will be written, excluding the header and footer lines.
        /// </summary>
        /// <returns></returns>
        protected abstract int GetLineCount();

        private string ConvertRegionToString(Region region)
        {
            switch (region)
            {
                case Region.NationalScheme:
                    return "H";

                default:
                    throw new NotSupportedException();
            }
        }
    }
}