namespace EA.Weee.Core.Admin.Obligation
{
    using System.Collections.Generic;

    public class SchemeObligationUploadData
    {
        public List<SchemeObligationUploadErrorData> ErrorData { get; set; }

        public List<SchemeObligationData> ObligationData { get; set; }

        public SchemeObligationUploadData()
        {
            ErrorData = new List<SchemeObligationUploadErrorData>();
            ObligationData = new List<SchemeObligationData>();
        }
    }
}
