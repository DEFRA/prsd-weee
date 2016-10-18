namespace EA.Weee.Requests.Shared
{
    public enum DataReturnSubmissionsHistoryOrderBy
    {
        /// <summary>
        /// Results are sorted by compliance year in ascending order
        /// followed by the submission date in descending order
        /// </summary>
        ComplianceYearAscending,

        /// <summary>
        /// Results are sorted by compliance year in descending order
        /// followed by the submission date in descending order
        /// </summary>
        ComplianceYearDescending,

        /// <summary>
        /// Results are grouped by compliance year in descending order, sorted by quarter in 
        /// ascending order followed by submission date in descending order
        /// </summary>
        QuarterAscending,

        /// <summary>
        /// Results are grouped by compliance year in descending order, sorted by quarter in 
        /// descending order followed by submission date in descending order
        /// </summary>
        QuarterDescending,

        /// <summary>
        /// Results are sorted by submission date in ascending order
        /// </summary>
        SubmissionDateAscending,

        /// <summary>
        /// Results are sorted by submission date in descending order
        /// </summary>
        SubmissionDateDescending
    }
}
