namespace EA.Weee.RequestHandlers.Tests.Unit.Charges.IssuePendingCharges
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;
    using Core.Shared;
    using Domain;
    using Domain.Charges;
    using Domain.Scheme;
    using FakeItEasy;
    using RequestHandlers.Charges.IssuePendingCharges;
    using RequestHandlers.Security;
    using RequestHandlers.Shared.DomainUser;
    using Weee.Tests.Core;
    using Xunit;

    public class IssuePendingChargesHandlerTests
    {
        /// <summary>
        /// This test ensures that a user with no access to the internal area cannot use
        /// the IssuePendingChargesHandler without a SecurityException being thrown.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task HandleAsync_WithNoInternalAccess_ThrowsSecurityException()
        {
            // Arrange
            IWeeeAuthorization authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();

            IssuePendingChargesHandler handler = new IssuePendingChargesHandler(
                authorization,
                A.Dummy<IIssuePendingChargesDataAccess>(),
                A.Dummy<IIbisFileDataGenerator>(),
                A.Dummy<IDomainUserContext>());

            // Act
            Func<Task<Guid>> testCode = async () => await handler.HandleAsync(A.Dummy<Requests.Charges.IssuePendingCharges>());

            // Assert
            await Assert.ThrowsAsync<SecurityException>(testCode);
        }

        /// <summary>
        /// This test ensures that the IssuePendingChargesHandler fetches member uploads from the data access
        /// and creates a new InvoiceRun domain object which it passes back to the data access before returning
        /// the ID.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task HandleAsync_Always_FetchesChargesFromDataAccessCreatesDomainObjectAndSaves()
        {
            // Arrange
            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();

            UKCompetentAuthority authority = A.Dummy<UKCompetentAuthority>();

            Scheme scheme = A.Fake<Scheme>();
            A.CallTo(() => scheme.CompetentAuthority).Returns(authority);

            List<MemberUpload> memberUploads = new List<MemberUpload>();

            MemberUpload memberUpload1 = new MemberUpload(
                A.Dummy<Guid>(),
                A.Dummy<string>(),
                A.Dummy<List<MemberUploadError>>(),
                10,
                2017,
                scheme,
                A.Dummy<string>());

            memberUpload1.Submit(A.Dummy<User>());

            MemberUpload memberUpload2 = new MemberUpload(
                A.Dummy<Guid>(),
                A.Dummy<string>(),
                A.Dummy<List<MemberUploadError>>(),
                20,
                2017,
                scheme,
                A.Dummy<string>());

            memberUpload2.Submit(A.Dummy<User>());

            memberUploads.Add(memberUpload1);
            memberUploads.Add(memberUpload2);

            IIssuePendingChargesDataAccess dataAccess = A.Fake<IIssuePendingChargesDataAccess>();
            A.CallTo(() => dataAccess.FetchCompetentAuthority(CompetentAuthority.NorthernIreland)).Returns(authority);
            A.CallTo(() => dataAccess.FetchSubmittedNonInvoicedMemberUploadsAsync(authority))
                .Returns(memberUploads);

            InvoiceRun capturedInvoiceRun = null;
            A.CallTo(() => dataAccess.SaveAsync(A<InvoiceRun>._))
                .Invokes(call => { capturedInvoiceRun = call.Arguments.Get<InvoiceRun>("invoiceRun"); })
                .Returns(Task.FromResult(true));

            IssuePendingChargesHandler handler = new IssuePendingChargesHandler(
                authorization,
                dataAccess,
                A.Dummy<IIbisFileDataGenerator>(),
                A.Dummy<IDomainUserContext>());

            Requests.Charges.IssuePendingCharges request = new Requests.Charges.IssuePendingCharges(CompetentAuthority.NorthernIreland);

            // Act
            Guid invoiceRunId = await handler.HandleAsync(request);

            // Assert
            Assert.NotNull(capturedInvoiceRun);
            Assert.Equal(capturedInvoiceRun.Id, invoiceRunId);
            Assert.Equal(2, capturedInvoiceRun.MemberUploads.Count);
            Assert.Equal(authority, capturedInvoiceRun.CompetentAuthority);
        }
    }
}
