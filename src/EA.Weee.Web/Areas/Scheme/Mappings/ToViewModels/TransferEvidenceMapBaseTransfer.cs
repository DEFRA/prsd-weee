namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using EA.Weee.Core.AatfEvidence;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class TransferEvidenceMapBaseTransfer
    {
        public Guid OrganisationId { get; set; }
        public Guid SchemeId { get; set; }
        public IList<EvidenceNoteData> Notes { get; set; }
        public IList<int> CategoryIds { get; set; }
    }
}