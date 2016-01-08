namespace EA.Weee.RequestHandlers.Tests.Unit.Charges.FetchInvoiceRuns
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;
    using Core.Charges;
    using FakeItEasy;
    using RequestHandlers.Charges.FetchInvoiceRuns;
    using RequestHandlers.Security;
    using Weee.Tests.Core;
    using Xunit;

    public class FetchInvoiceRunsHandlerTests
    {
        /// <summary>
        /// This test ensures that a user with no access to the internal area cannot use
        /// the FetchInvoiceRunsHandler without a SecurityException being thrown.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task HandleAsync_WithNoInternalAccess_ThrowsSecurityException()
        {
            // Arrange
            IWeeeAuthorization authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();

            FetchInvoiceRunsHandler handler = new FetchInvoiceRunsHandler(
                authorization,
                A.Dummy<IFetchInvoiceRunsDataAccess>());

            // Act
            Func<Task<IReadOnlyList<InvoiceRunInfo>>> testCode = async () => await handler.HandleAsync(A.Dummy<Requests.Charges.FetchInvoiceRuns>());

            // Assert
            await Assert.ThrowsAsync<SecurityException>(testCode);
        }
    }
}
