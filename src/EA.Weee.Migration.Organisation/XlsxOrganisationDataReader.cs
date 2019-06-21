namespace EA.Weee.Migration.Organisation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using ExcelDataReader;
    using Model;

    public static class XlsxOrganisationDataReader
    {
        public static bool TryGetOrganisationData(string fileName, out IList<Organisation> organisations)
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

            organisations = new List<Organisation>();

            var dataTable = result.Tables[0].Rows;

            for (var i = 1; i < dataTable.Count; i++)
            {
                if (dataTable[i][0] == DBNull.Value)
                {
                    Console.WriteLine("Skipping empty row {0}", i);
                    break;
                }

                try
                {
                    var addressData = new Address(dataTable[i].Field<string>(4),
                        dataTable[i].Field<string>(5),
                        dataTable[i].Field<string>(6),
                        dataTable[i].Field<string>(7),
                        dataTable[i].Field<string>(8),
                        dataTable[i].Field<string>(9),
                        dataTable[i].Field<object>(10).ToString(),
                        dataTable[i].Field<string>(11));

                    organisations.Add(new Organisation(
                        i,
                        dataTable[i].Field<string>(0),
                        dataTable[i].Field<string>(1),
                        TryParseEnum(dataTable[i].Field<string>(2)),
                        dataTable[i].Field<object>(3).ToString(),
                        addressData));
                }
                catch
                {
                    hasErrors = true;
                    Console.WriteLine("Unable to parse row {0}:{1}", i, string.Join(",", dataTable[i].ItemArray.Select(x => x.ToString()).ToArray()));
                }
            }

            return !hasErrors;
        }

        private static OrganisationType? TryParseEnum(string input)
        {
            switch (input)
            {
                case "Registered company":
                    return OrganisationType.RegisteredCompany;
                case "Partnership":
                    return OrganisationType.Partnership;
                case "Sole trader or individual":
                    return OrganisationType.SoleTraderOrIndividual;
                default:
                    return null;
            }
        }

        public static string GetDisplayName(this Enum enumValue)
        {
            return enumValue.GetType()
                .GetMember(enumValue.ToString())
                .First()
                .GetCustomAttribute<DisplayAttribute>()
                .GetName();
        }
    }
}
