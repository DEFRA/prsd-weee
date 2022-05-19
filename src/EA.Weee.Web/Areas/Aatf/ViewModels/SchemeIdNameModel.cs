namespace EA.Weee.Web.Areas.Aatf.ViewModels
{
    using System;

    public class SchemeIdNameModel
    {
        public Guid Id { get; set; }

        public string SchemeName { get; set; }

        public SchemeIdNameModel(Guid id, string schemeName)
        {
            Id = id;
            SchemeName = schemeName;
        }
    }
}
