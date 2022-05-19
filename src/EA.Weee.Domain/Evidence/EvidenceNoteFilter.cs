namespace EA.Weee.Domain.Evidence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Prsd.Core.Domain;

    public class EvidenceNoteFilter
    {
        public Guid? AatfId { get; set; }

        public Guid? OrganisationId { get; set; }

        public Guid? SchemeId { get; set; }

        public List<NoteStatus> AllowedStatuses { get; set; }

        public string SearchRef { get; set; }

        public int? WasteTypeId { get; set; }

        public int? NoteStatusId { get; set; }

        public DateTime? StartDateSubmitted { get; set; }

        public DateTime? EndDateSubmitted { get; set; }

        public string FormattedSearchRef
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(SearchRef))
                {
                    if (MatchReferenceWithNoteType())
                    {
                        return SearchRef.Trim().Remove(0, 1);
                    }
                }

                return SearchRef;
            }
        }

        private bool MatchReferenceWithNoteType()
        {
            var regex = new Regex("^[E?|T?|e?|t?][1-9]+");
            return regex.Match(SearchRef.Trim()).Success;
        }

        public int FormattedNoteType
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(SearchRef))
                {
                    if (MatchReferenceWithNoteType())
                    {
                        return Enumeration.FromDisplayName<NoteType>(SearchRef.ToUpper().Substring(0, 1)).Value;
                    }

                    return 0;
                }

                return -1;
            }
        }
    }
}
