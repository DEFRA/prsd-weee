namespace EA.Weee.Core.Admin
{
    using System;

    public class SchemeSearchData
    {
        public Guid SchemeId { get; set; }

        public string SchemeName { get; set; }

        public SchemeSearchData(Guid id, string name)
        {
            SchemeId = id;
            SchemeName = name;
        }
    }
}
