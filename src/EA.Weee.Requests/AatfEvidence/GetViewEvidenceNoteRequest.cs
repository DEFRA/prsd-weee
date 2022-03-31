namespace EA.Weee.Requests.AatfEvidence
{
    using System;
    using System.Collections.Generic;
    using Aatf;
    using Core.AatfEvidence;
    using Prsd.Core;

    [Serializable]
    public class GetViewEvidenceNoteRequest : CreateEvidenceNoteRequest
    {
        public GetViewEvidenceNoteRequest(Guid organisationId,
            Guid aatfId,
            Guid recipientId,
            DateTime startDate,
            DateTime endDate,
            WasteType? wasteType,
            Protocol? protocol,
            IList<TonnageValues> tonnages)
        {
            // all checkings will be done in base class
            //Guard.ArgumentNotDefaultValue(() => organisationId, organisationId);
            //Guard.ArgumentNotDefaultValue(() => aatfId, aatfId);
            //Guard.ArgumentNotDefaultValue(() => recipientId, recipientId);
            //Guard.ArgumentNotDefaultValue(() => startDate, startDate);
            //Guard.ArgumentNotDefaultValue(() => endDate, endDate);

            // IMPORTANT
            // NOT SURE WHICH PARAMETERS ARE COMING FROM THE CALLING CREATE PAGE
            // IF SAME PARAMETERS THEN QUERY NEW NOTE AND HIDE GUID IN PAGE FOR DOWNLOAD PDF

            OrganisationId = organisationId;
            RecipientId = recipientId;
            AatfId = aatfId;
            StartDate = startDate;
            EndDate = endDate;
            WasteType = wasteType;
            Protocol = protocol;
            TonnageValues = tonnages;

            /*
             * SELECT  Evidence.Note.AatfId, Evidence.Note.OrganisationId, Evidence.Note.StartDate, Evidence.Note.EndDate, Evidence.Note.RecipientId, Evidence.Note.WasteType, Evidence.Note.Protocol, Evidence.Note.Status, 
                       Evidence.Note.Id AS Expr4, Evidence.NoteTonnage.NoteId, Evidence.NoteTonnage.CategoryId, Evidence.NoteTonnage.Received, Evidence.NoteTonnage.Reused, Evidence.Note.Reference
                FROM   Evidence.Note INNER JOIN
                       Evidence.NoteTonnage ON Evidence.Note.Id = Evidence.NoteTonnage.NoteId
             * 
             * DID NOT FIND WHERE TO PLACE MY STORED PROC / VIEW - TBD
             */
        }
    }
}
