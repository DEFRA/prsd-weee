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
                var allowedNoteTypes = Enumeration.GetAll<NoteType>().Select(n => n.DisplayName.ToCharArray()[0]).ToArray();

                if (!string.IsNullOrWhiteSpace(SearchRef))
                {
                    return SearchRef.ToUpper().Trim().Trim(allowedNoteTypes);
                }

                return SearchRef;
            }
        }

        public int? NoteType
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(SearchRef))
                {
                    var regex = new Regex("[E|T][1-9]+");

                    if (regex.Match(SearchRef.ToUpper()).Success)
                    {
                        var type = SearchRef.Substring(0, 1).ToUpper();
                        return Enumeration.FromDisplayName<NoteType>(type).Value;
                    }
                }

                return null;
            }
        }
    }
}
