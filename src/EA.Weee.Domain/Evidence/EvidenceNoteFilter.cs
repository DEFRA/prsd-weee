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

        public string FormattedSearchRef
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(SearchRef))
                {
                    var regex = new Regex("^[E?|T?|e?|t?][1-9]+");
                    if (regex.Match(SearchRef.Trim()).Success)
                    {
                        return SearchRef.Trim().Remove(0, 1);
                    }
                }

                return SearchRef;
            }
        }
    }
}
