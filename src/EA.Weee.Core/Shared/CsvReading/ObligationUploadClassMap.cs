namespace EA.Weee.Core.Shared.CsvReading
{
    using CsvHelper;
    using CsvHelper.Configuration;

    public sealed class ObligationUploadClassMap : ClassMap<ObligationCsvUpload>
    {
        public ObligationUploadClassMap()
        {
            Map(o => o.SchemeIdentifier).Name("Scheme Identifier");
            Map(o => o.SchemeName).Name("Scheme Name");
            Map(o => o.Cat1).Name("Cat1 (t)");
            Map(o => o.Cat2).Name("Cat2 (t)");
            Map(o => o.Cat3).Name("Cat3 (t)");
            Map(o => o.Cat4).Name("Cat4 (t)");
            Map(o => o.Cat5).Name("Cat5 (t)");
            Map(o => o.Cat6).Name("Cat6 (t)");
            Map(o => o.Cat7).Name("Cat7 (t)");
            Map(o => o.Cat8).Name("Cat8 (t)");
            Map(o => o.Cat9).Name("Cat9 (t)");
            Map(o => o.Cat10).Name("Cat10 (t)");
            Map(o => o.Cat11).Name("Cat11 (t)");
            Map(o => o.Cat12).Name("Cat12 (t)");
            Map(o => o.Cat13).Name("Cat13 (t)");
            Map(o => o.Cat14).Name("Cat14 (t)");
        }
    }
}
