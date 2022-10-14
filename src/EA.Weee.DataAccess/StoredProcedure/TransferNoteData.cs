namespace EA.Weee.DataAccess.StoredProcedure
{
    using System;

    /// <summary>
    /// Maps to [Evidence].[getTransferNotes]
    /// </summary>
    public class TransferNoteData
    {
        public string TransferReference { get; set; }

        public string TransferStatus { get; set; }

        public DateTime? TransferApprovalDate { get; set; }

        public string TransferredByName { get; set; }

        public string TransferredByApprovalNumber { get; set; }
        
        public string RecipientName { get; set; }

        public string RecipientApprovalNumber { get; set; }

        public string EvidenceNoteReference { get; set; }

        public DateTime? EvidenceNoteApprovalDate { get; set; }  // sql query moanied about this not being nullable

        public string AatfIssuedByName { get; set; }

        public string AatfIssuedByApprovalNumber { get; set; }

        public string Protocol { get; set; }

        public decimal Cat1Received { get; set; }

        public decimal Cat2Received { get; set; }

        public decimal Cat3Received { get; set; }

        public decimal Cat4Received { get; set; }

        public decimal Cat5Received { get; set; }

        public decimal Cat6Received { get; set; }

        public decimal Cat7Received { get; set; }

        public decimal Cat8Received { get; set; }

        public decimal Cat9Received { get; set; }

        public decimal Cat10Received { get; set; }

        public decimal Cat11Received { get; set; }

        public decimal Cat12Received { get; set; }

        public decimal Cat13Received { get; set; }

        public decimal Cat14Received { get; set; }

        public decimal TotalReceived { get; set; }

        public decimal Cat1Reused { get; set; }

        public decimal Cat2Reused { get; set; }

        public decimal Cat3Reused { get; set; }

        public decimal Cat4Reused { get; set; }

        public decimal Cat5Reused { get; set; }

        public decimal Cat6Reused { get; set; }

        public decimal Cat7Reused { get; set; }

        public decimal Cat8Reused { get; set; }

        public decimal Cat9Reused { get; set; }

        public decimal Cat10Reused { get; set; }

        public decimal Cat11Reused { get; set; }

        public decimal Cat12Reused { get; set; }

        public decimal Cat13Reused { get; set; }

        public decimal Cat14Reused { get; set; }

        public decimal TotalReused { get; set; }
    }
}
