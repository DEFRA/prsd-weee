namespace EA.Weee.Domain.Tests.Unit.Scheme
{
    using System.Collections.Generic;
    using System.Linq;
    using Domain.Scheme;
    using Xunit;

    public class SchemeDataAvailabilityTests
    {
        [Fact]
        public void Create_WithNullListsOfYears_ReturnsEmptyListOfDownloads()
        {
            var result = SchemeDataAvailability.Create(null, null);

            Assert.Empty(result.DownloadsByYears);
        }

        [Fact]
        public void Create_WithEmptyListsOfYears_ReturnsEmptyListOfDownloads()
        {
            var result = SchemeDataAvailability.Create(new List<int>(), new List<int>());

            Assert.Empty(result.DownloadsByYears);
        }

        [Fact]
        public void Create_HasOneMemberUploadYear_ButNullDataReturnsYears_HasSingleResultWithThatYear_AndHasMemberUploadIsTrue_AndHasDataReturnIsFalse()
        {
            const int complianceYear = 2016;

            var result = SchemeDataAvailability.Create(new List<int> { complianceYear }, null);

            Assert.Single(result.DownloadsByYears);

            var downloadsByYear = result.DownloadsByYears.Single();

            Assert.Equal(complianceYear, complianceYear);
            Assert.True(downloadsByYear.IsMembersDownloadAvailable);
            Assert.False(downloadsByYear.IsDataReturnsDownloadAvailable);
        }

        [Fact]
        public void Create_HasOneMemberUploadYear_AndTheSameDataReturnYear_HasSingleResultWithThatYear_AndHasMemberUploadIsTrue_AndHasDataReturnIsFalse()
        {
            const int complianceYear = 2016;

            var result = SchemeDataAvailability.Create(new List<int> { complianceYear }, new List<int> { complianceYear });

            Assert.Single(result.DownloadsByYears);

            var downloadsByYear = result.DownloadsByYears.Single();

            Assert.Equal(complianceYear, complianceYear);
            Assert.True(downloadsByYear.IsMembersDownloadAvailable);
            Assert.True(downloadsByYear.IsDataReturnsDownloadAvailable);
        }

        [Fact]
        public void
            Create_HasOneMemberUploadYear_AndDifferentDataReturnYear_HasTwoResults_OneWithMemberUploadIsTrue_OneWithDataReturnTrue
            ()
        {
            const int memberUploadComplianceYear = 2016;
            const int dataReturnComplianceYear = 2017;

            var result = SchemeDataAvailability.Create(new List<int> { memberUploadComplianceYear }, new List<int> { dataReturnComplianceYear });

            Assert.Equal(2, result.DownloadsByYears.Count);

            Assert.Single(
                result.DownloadsByYears.Where(
                    d => d.IsMembersDownloadAvailable && d.Year == memberUploadComplianceYear));

            Assert.Single(
                result.DownloadsByYears.Where(
                    d => d.IsDataReturnsDownloadAvailable && d.Year == dataReturnComplianceYear));
        }
    }
}
