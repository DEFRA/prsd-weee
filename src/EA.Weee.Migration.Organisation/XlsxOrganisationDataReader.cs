namespace EA.Weee.Migration.Organisation
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using ExcelDataReader;
    using NLog;

    public static class XlsxOrganisationDataReader
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static bool TryGetOrganisationData(string fileName, out IList<OrganisationData> organisations)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException("File not found", fileName);
            }

            bool hasErrors = false;
            DataSet result;

            using (var fileStream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var excelReader = ExcelReaderFactory.CreateReader(fileStream))
                {
                    result = excelReader.AsDataSet();
                }
            }

            Logger.Info("Found {0} rows to import", result.Tables[0].Rows.Count);
            organisations = new List<OrganisationData>();

            var dataTable = result.Tables[0].Rows;

            for (var i = 0; i < dataTable.Count; i++)
            {
                if (dataTable[i][0] == DBNull.Value)
                {
                    Console.WriteLine("Skipping empty row {0}", i);
                    break;
                }

                try
                {
                    organisations.Add(new OrganisationData()
                    {
                        RowNumber = i,
                        Name = dataTable[i].Field<string>(0),
                        TradingName = dataTable[i].Field<string>(1),
                        OrganisationType = dataTable[i][2] == DBNull.Value ? (OrganisationType?)null : (OrganisationType)Enum.Parse(typeof(OrganisationType), dataTable[i].Field<string>(2)),
                        RegistrationNumber = dataTable[i].Field<string>(3),
                        AddressLine1 = dataTable[i].Field<string>(4),
                        AddressLine2 = dataTable[i].Field<string>(5),
                        TownOrCity = dataTable[i].Field<string>(6),
                        CountyOrRegion = dataTable[i].Field<string>(7),
                        Postcode = dataTable[i].Field<string>(8),
                        Country = dataTable[i].Field<string>(9),
                        Telephone = dataTable[i].Field<string>(10),
                        Email = dataTable[i].Field<string>(11),
                    });
                }
                catch
                {
                    hasErrors = true;
                    Console.WriteLine("Unable to parse row {0}:{1}", i, string.Join(",", dataTable[i].ItemArray.Select(x => x.ToString()).ToArray()));
                }
            }

            return !hasErrors;
        }
    }
}
