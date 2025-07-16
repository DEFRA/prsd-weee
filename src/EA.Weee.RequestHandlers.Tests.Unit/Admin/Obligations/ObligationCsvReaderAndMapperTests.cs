namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.Obligations
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using EA.Prsd.Core.Helpers;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Core.Shared;
    using EA.Weee.Core.Shared.CsvReading;
    using EA.Weee.RequestHandlers.Admin.Obligations;
    using Xunit;

    /// <summary>
    /// The <see cref="ObligationsCsvReaderTests"/> class does not flag missing mappings in <see cref="ObligationUploadClassMap"/>
    /// It seemed sensible to add a test that did.
    /// Since this requires quite different setup, it seemed sensible to move this to a new class
    /// </summary>
    public class ObligationCsvReaderAndMapperTests
    {
        [Fact]
        public void ObligationCsvReader_GivenReadableData_ShouldBeMappedCorrectly()
        {
            //arrange
            var categories = EnumHelper.GetValues(typeof(WeeeCategory));

            // columns are Scheme Identifier, Scheme Name, then Cat<number> (t)
            // so create some data containing this, and make sure it comes out correctly

            var values = new List<KeyValuePair<string, string>>();
            var expected = new ObligationCsvUpload();

            const string schemeIdentifier = "the scheme identifer";
            values.Add(new KeyValuePair<string, string>("Scheme Identifier", schemeIdentifier));
            expected.SchemeIdentifier = schemeIdentifier;
            const string schemeName = "the scheme name";
            values.Add(new KeyValuePair<string, string>("Scheme Name", schemeName));
            expected.SchemeName = schemeName;

            foreach (var category in categories)
            {
                var dataValue = category.Key * 100;
                values.Add(new KeyValuePair<string, string>($"Cat{category.Key} (t)", $"{dataValue}"));
                var type = expected.GetType();
                var categoryPropertyName = $"Cat{category.Key}";
                var categoryProperty = type.GetProperty(categoryPropertyName);
                categoryProperty.SetValue(expected, $"{dataValue}");
            }

            // now convert to CSV
            var csvData = string.Join(",", values.Select(x => x.Key))
                .Concat(System.Environment.NewLine)
                .Concat(string.Join(",", values.Select(x => x.Value)));

            var csvBytes = Encoding.UTF8.GetBytes(csvData.ToArray());

            // act
            var reader = new ObligationCsvReader(new FileHelper());
            var result = reader.Read(csvBytes);

            //assert
            // hard-code one specific category to allow this test to be found in a code references search
            Assert.Equal("1500", expected.Cat15);

            Assert.Equivalent(new[] { expected }, result);
        }
    }
}
