namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.ViewModels
{
    using Web.Areas.Scheme.ViewModels;
    using Xunit;

    public class LatestMemberUploadViewModelTests
    {
        [Theory]
        [InlineData(99, "100", "bytes")]
        [InlineData(100, "0.1", "kb")]
        [InlineData(102399, "100", "kb")]
        [InlineData(102400, "0.1", "Mb")]
        [InlineData(104857599, "100", "Mb")]
        [InlineData(104857600, "0.1", "Gb")]
        public void SizeInBytes_IsFormattedAsExpected(double fileSizeInBytes, string expectedRoundedValue, string expectedSuffix)
        {
            var model = new LatestMemberUploadViewModel
            {
                CsvFileSizeEstimate = fileSizeInBytes
            };

            Assert.Equal(expectedSuffix, model.ScaledCsvFileSizeSuffix);
            Assert.Equal(expectedRoundedValue, model.ScaledCsvFileSize);
        }
    }
}
