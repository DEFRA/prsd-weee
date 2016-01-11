namespace EA.Weee.Domain.Tests.Unit.Scheme
{
    using System;
    using System.Collections.Generic;
    using Domain.Charges;
    using Domain.Producer;
    using Domain.Scheme;
    using FakeItEasy;
    using Lookup;
    using Xunit;

    public class MemberUploadTests
    {
        [Fact]
        public void Submit_WhenNotYetSubmitted_MarksMemberUploadAsSubmitted()
        {
            // Arrange
            var memberUpload = new MemberUpload(
                A.Dummy<Guid>(),
                A.Dummy<string>(),
                A.Dummy<List<MemberUploadError>>(),
                A.Dummy<int>(),
                A.Dummy<int>(),
                A.Dummy<Scheme>(),
                A.Dummy<string>());

            // Act
            memberUpload.Submit(A.Dummy<User>());

            // Assert
            Assert.True(memberUpload.IsSubmitted);
        }

        [Fact]
        public void Submit_WhenAlreadySubmitted_ThrowInvalidOperationException()
        {
            // Arrange
            var memberUpload = new MemberUpload(
                A.Dummy<Guid>(),
                A.Dummy<string>(),
                A.Dummy<List<MemberUploadError>>(),
                A.Dummy<int>(),
                A.Dummy<int>(),
                A.Dummy<Scheme>(),
                A.Dummy<string>());

            memberUpload.Submit(A.Dummy<User>());

            // Act
            Action action = () => memberUpload.Submit(A.Dummy<User>());

            // Assert
            Assert.Throws<InvalidOperationException>(action);
        }

        [Fact]
        public void SetProcessTime_WhenCurrentValueIsZero_SetProcessTime()
        {
            // Arrange
            var memberUpload = new MemberUpload(
                A.Dummy<Guid>(),
                A.Dummy<string>(),
                A.Dummy<List<MemberUploadError>>(),
                A.Dummy<int>(),
                A.Dummy<int>(),
                A.Dummy<Scheme>(),
                A.Dummy<string>());

            // Act
            memberUpload.SetProcessTime(TimeSpan.FromSeconds(15));

            // Assert
            Assert.Equal(TimeSpan.FromSeconds(15), memberUpload.ProcessTime);
        }

        [Fact]
        public void SetProcessTime_WhenCurrentValueIsNotZero_ThrowInvalidOperationException()
        {
            // Arrange
            var memberUpload = new MemberUpload(
                A.Dummy<Guid>(),
                A.Dummy<string>(),
                A.Dummy<List<MemberUploadError>>(),
                A.Dummy<int>(),
                A.Dummy<int>(),
                A.Dummy<Scheme>(),
                A.Dummy<string>());

            memberUpload.SetProcessTime(TimeSpan.FromSeconds(15));

            // Act
            Action action = () => memberUpload.SetProcessTime(TimeSpan.FromSeconds(25));

            // Assert
            Assert.Throws<InvalidOperationException>(action);
        }

        /// <summary>
        /// This test ensures that a member upload which has not been submitted cannot be assigned to an invoice run. 
        /// </summary>
        [Fact]
        public void AssignToInvoiceRun_ForUnsubmittedMemberUpload_ThrowsInvalidOperationException()
        {
            Scheme scheme = A.Fake<Scheme>();
            MemberUpload memberUpload = new MemberUpload(new Guid("A2A01A99-A97D-4219-9060-D7CDF7435114"), scheme, "data", "filename");

            Assert.Throws<InvalidOperationException>(() => memberUpload.AssignToInvoiceRun(A.Dummy<InvoiceRun>()));
        }

        [Fact]
        public void AssignToInvoiceRun_SetsProducerSubmissionAsInvoiced()
        {
            Scheme scheme = A.Fake<Scheme>();
            MemberUpload memberUpload = new MemberUpload(A.Dummy<Guid>(), "data", null, 100, 2015, scheme, "fileName");

            var registeredProducer = new RegisteredProducer("PRN", 2015, scheme);

            var producer = new ProducerSubmission(
                registeredProducer,
                memberUpload,
                new ProducerBusiness(),
                null,
                new DateTime(2015, 1, 1),
                0,
                false,
                null,
                "Trading Name 1",
                EEEPlacedOnMarketBandType.Lessthan5TEEEplacedonmarket,
                SellingTechniqueType.Both,
                ObligationType.Both,
                AnnualTurnOverBandType.Greaterthanonemillionpounds,
                new List<BrandName>(),
                new List<SICCode>(),
                A.Dummy<ChargeBandAmount>(),
                0);

            memberUpload.ProducerSubmissions.Add(producer);
            memberUpload.Submit(A.Dummy<User>());

            memberUpload.AssignToInvoiceRun(A.Dummy<InvoiceRun>());

            Assert.True(producer.Invoiced);
        }

        [Fact]
        public void AssignToInvoiceRun_DoesNotSetRemovedProducerSubmissionAsInvoiced()
        {
            Scheme scheme = A.Fake<Scheme>();
            MemberUpload memberUpload = new MemberUpload(A.Dummy<Guid>(), "data", null, 100, 2015, scheme, "fileName");

            var registeredProducer = new RegisteredProducer("PRN", 2015, scheme);
            registeredProducer.Unalign();

            var producer = new ProducerSubmission(
                registeredProducer,
                memberUpload,
                new ProducerBusiness(),
                null,
                new DateTime(2015, 1, 1),
                0,
                false,
                null,
                "Trading Name 1",
                EEEPlacedOnMarketBandType.Lessthan5TEEEplacedonmarket,
                SellingTechniqueType.Both,
                ObligationType.Both,
                AnnualTurnOverBandType.Greaterthanonemillionpounds,
                new List<BrandName>(),
                new List<SICCode>(),
                A.Dummy<ChargeBandAmount>(),
                0);

            memberUpload.ProducerSubmissions.Add(producer);
            memberUpload.Submit(A.Dummy<User>());

            memberUpload.AssignToInvoiceRun(A.Dummy<InvoiceRun>());

            Assert.False(producer.Invoiced);
        }
    }
}
