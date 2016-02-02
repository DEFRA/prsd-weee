namespace EA.Weee.DataAccess.Tests.DataAccess.StoredProcedure
{
    using Domain.Obligation;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class SpgSchemeWeeeCsvAsyncTests
    {
        [Theory]
        [InlineData(ObligationType.Both, 2017)]
        [InlineData(ObligationType.B2B, 2017)]
        [InlineData(ObligationType.B2C, 2017)]
        [InlineData(ObligationType.Both, 2016)]
        [InlineData(ObligationType.B2B, 2016)]
        [InlineData(ObligationType.B2C, 2016)]
        public async void GivenVaryingParameters_ProcessesSucessfully(ObligationType obligationType, int complianceYear)
        {
            using (var dbWrapper = new DatabaseWrapper())
            {
                var ex = await Record.ExceptionAsync(() => dbWrapper.StoredProcedures.SpgSchemeWeeeCsvAsync(complianceYear, obligationType.ToString()));

                Assert.Null(ex);
            }
        }
    }
}
