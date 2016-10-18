namespace EA.Weee.Requests.Shared
{
    public enum SubmissionsHistoryOrderBy
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
        /// Results are sorted by submission date in ascending order
        /// </summary>
        SubmissionDateAscending,

        /// <summary>
        /// Results are sorted by submission date in descending order
        /// </summary>
        SubmissionDateDescending
    }
}
