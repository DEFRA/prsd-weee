namespace EA.Weee.DataAccess.StoredProcedure
{
    using System;

    /// <summary>
    /// This class maps to the results of [AATF].[getAatfAeReturnDataCsvData]
    /// </summary>
    public class AatfAeReturnData
    {
        public string Name { get; set; }

        public string ApprovalNumber { get; set; }

        public string OrganisationName { get; set; }

        public string ReturnStatus { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public string SubmittedBy { get; set; }
        public string CompetentAuthorityAbbr { get; set; }
    }
}
