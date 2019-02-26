namespace EA.Weee.RequestHandlers.Tests.DataAccess.AatfReturn.ObligatedReceived
{
    using RequestHandlers.AatfReturn.ObligatedReceived;
    using Weee.DataAccess;
    using Weee.Tests.Core;
    using Weee.Tests.Core.Model;
    using Xunit;
    using WeeeReceivedAmount = Domain.AatfReturn.WeeeReceivedAmount;

    public class ObligatedReceivedDataAccessTests
    {
        private readonly ObligatedReceivedDataAccess dataAccess;
        private readonly WeeeContext context;
        private readonly DbContextHelper dbContextHelper;

        public ObligatedReceivedDataAccessTests()
        {
            

        }

        [Fact]
        public void UpdateAmounts_GivenAmountToUpdate_ContextShouldContainUpdatedAmounts()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new ObligatedReceivedDataAccess(database.WeeeContext);

                var receivedAmount = new WeeeReceivedAmount()
            }
        }
    }
}
