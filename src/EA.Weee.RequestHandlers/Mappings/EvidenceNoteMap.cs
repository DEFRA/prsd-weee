﻿namespace EA.Weee.RequestHandlers.Mappings
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Domain;

    public class EvidenceNoteMap : IMap<EvidenceNoteMappingTransfer, EvidenceNoteData>
    {
        public EvidenceNoteData Map(EvidenceNoteMappingTransfer source)
        {
            return new EvidenceNoteData(source.SchemeData)
            {
                Reference = source.Note.Reference
            };
        }
    }
}
