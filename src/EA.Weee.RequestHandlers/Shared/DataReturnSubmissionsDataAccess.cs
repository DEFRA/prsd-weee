namespace EA.Weee.RequestHandlers.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.DataReturns;

    public class DataReturnSubmissionsDataAccess : IDataReturnSubmissionsDataAccess
    {
        private readonly WeeeContext context;

        /// <summary>
        /// Maintains a collection of submissions grouped by scheme, year and quarter.
        /// The list is then sorted by submission date with the earliest at the beginning
        /// of the list.
        /// </summary>
        private readonly Dictionary<SchemeYearQuarter, List<DataReturnVersion>> dataReturnVersions;

        public DataReturnSubmissionsDataAccess(WeeeContext context)
        {
            this.context = context;
            dataReturnVersions = new Dictionary<SchemeYearQuarter, List<DataReturnVersion>>();
        }

        public async Task<DataReturnVersion> GetPreviousSubmission(DataReturnVersion dataReturnVersion)
        {
            if (!dataReturnVersion.IsSubmitted)
            {
                throw new ArgumentException("The data return version must be submitted.");
            }

            var key = new SchemeYearQuarter
            {
                SchemeId = dataReturnVersion.DataReturn.Scheme.Id,
                Year = dataReturnVersion.DataReturn.Quarter.Year,
                Quarter = dataReturnVersion.DataReturn.Quarter.Q
            };

            List<DataReturnVersion> schemeYearQuarterSubmissions;
            if (!dataReturnVersions.TryGetValue(key, out schemeYearQuarterSubmissions))
            {
                schemeYearQuarterSubmissions = await context.DataReturnVersions
                    .Where(d => d.DataReturn.Scheme.Id == key.SchemeId)
                    .Where(d => d.DataReturn.Quarter.Year == key.Year)
                    .Where(d => d.DataReturn.Quarter.Q == key.Quarter)
                    .Where(d => d.SubmittedDate.HasValue)
                    .OrderBy(d => d.SubmittedDate)
                    .ToListAsync();

                dataReturnVersions.Add(key, schemeYearQuarterSubmissions);
            }

            var index = schemeYearQuarterSubmissions.BinarySearch(dataReturnVersion, new DataReturnVersionComparer());
            if (index < 0)
            {
                throw new InvalidOperationException("The data return version is not currently stored in the database.");
            }

            return schemeYearQuarterSubmissions.ElementAtOrDefault(index - 1);
        }

        private struct SchemeYearQuarter : IEquatable<SchemeYearQuarter>
        {
            public Guid SchemeId { get; set; }

            public QuarterType Quarter { get; set; }

            public int Year { get; set; }

            public bool Equals(SchemeYearQuarter other)
            {
                return other.SchemeId == SchemeId
                    && other.Year == Year
                    && other.Quarter == Quarter;
            }

            public override bool Equals(object obj)
            {
                return Equals((SchemeYearQuarter)obj);
            }

            public override int GetHashCode()
            {
                return SchemeId.GetHashCode() ^ Year.GetHashCode() ^ Quarter.GetHashCode();
            }
        }

        private class DataReturnVersionComparer : IComparer<DataReturnVersion>
        {
            public int Compare(DataReturnVersion x, DataReturnVersion y)
            {
                return x.Id.CompareTo(y.Id);
            }
        }
    }
}