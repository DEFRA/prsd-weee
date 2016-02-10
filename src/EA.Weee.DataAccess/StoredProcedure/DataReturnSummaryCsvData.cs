namespace EA.Weee.DataAccess.StoredProcedure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    public class DataReturnSummaryCsvData
    {
        /// <summary>
        /// 1, 2, 3 or 4
        /// </summary>
        public int Quarter { get; set; }

        /// <summary>
        /// 0 = WEEE Collected
        /// 1 = WEEE Delivered
        /// 2 = EEE Output
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// For WEEE Collected:
        ///     0 = Collected from DCFs
        ///     1 = Collected from distributors
        ///     2 = Collected from final hodlers
        /// For WEEE Delivered:
        ///     0 = Delivered to AATFs
        ///     1 = Delivered to AEs
        /// For EEE Output
        ///     Not applicable (value will be null)
        /// </summary>
        public int? Source { get; set; }

        /// <summary>
        /// "B2B" or "B2C"
        /// </summary>
        public string ObligationType { get; set; }

        public decimal? Category1 { get; set; }

        public decimal? Category2 { get; set; }

        public decimal? Category3 { get; set; }

        public decimal? Category4 { get; set; }

        public decimal? Category5 { get; set; }

        public decimal? Category6 { get; set; }

        public decimal? Category7 { get; set; }

        public decimal? Category8 { get; set; }

        public decimal? Category9 { get; set; }

        public decimal? Category10 { get; set; }

        public decimal? Category11 { get; set; }

        public decimal? Category12 { get; set; }

        public decimal? Category13 { get; set; }

        public decimal? Category14 { get; set; }
    }
}
