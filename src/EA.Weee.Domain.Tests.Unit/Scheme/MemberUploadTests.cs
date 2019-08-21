﻿namespace EA.Weee.Domain.Tests.Unit.Scheme
{
    using Domain.Charges;
    using Domain.Producer;
    using Domain.Producer.Classfication;
    using Domain.Producer.Classification;
    using Domain.Scheme;
    using Domain.User;
    using Error;
    using Events;
    using FakeItEasy;
    using Lookup;
    using Obligation;
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
        public void Submit_AddSubmissionEventToEventsList()
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
            Assert.Equal(1, memberUpload.Events.Count());
            Assert.IsType<SchemeMemberSubmissionEvent>(memberUpload.Events.Single());
            Assert.Same(memberUpload, ((SchemeMemberSubmissionEvent)memberUpload.Events.Single()).MemberUpload);
        }

        [Fact]
        public void Submit_WhenContainsErrors_ThrowInvalidOperationException()
        {
            var error = new MemberUploadError(ErrorLevel.Error, A.Dummy<UploadErrorType>(), A.Dummy<string>());

            var memberUpload = new MemberUpload(
                A.Dummy<Guid>(),
                A.Dummy<string>(),
                new List<MemberUploadError> { error },
                A.Dummy<int>(),
                A.Dummy<int>(),
                A.Dummy<Scheme>(),
                A.Dummy<string>());

            Assert.Throws<InvalidOperationException>(() => memberUpload.Submit(A.Dummy<User>()));
        }

        [Fact]
        public void Submit_WhenContainsErrorsAndWarnings_ThrowInvalidOperationException()
        {
            var error = new MemberUploadError(ErrorLevel.Error, A.Dummy<UploadErrorType>(), A.Dummy<string>());
            var warning = new MemberUploadError(ErrorLevel.Warning, A.Dummy<UploadErrorType>(), A.Dummy<string>());

            var memberUpload = new MemberUpload(
                A.Dummy<Guid>(),
                A.Dummy<string>(),
                new List<MemberUploadError> { error, warning },
                A.Dummy<int>(),
                A.Dummy<int>(),
                A.Dummy<Scheme>(),
                A.Dummy<string>());

            Assert.Throws<InvalidOperationException>(() => memberUpload.Submit(A.Dummy<User>()));
        }

        [Fact]
        public void Submit_WhenContainsWarnings_SubmitsWithNoException()
        {
            var warning = new MemberUploadError(ErrorLevel.Warning, A.Dummy<UploadErrorType>(), A.Dummy<string>());

            var memberUpload = new MemberUpload(
                A.Dummy<Guid>(),
                A.Dummy<string>(),
                new List<MemberUploadError> { warning },
                A.Dummy<int>(),
                A.Dummy<int>(),
                A.Dummy<Scheme>(),
                A.Dummy<string>());

            var exception = Record.Exception(() => memberUpload.Submit(A.Dummy<User>()));

            Assert.Null(exception);
            Assert.True(memberUpload.IsSubmitted);
        }

        [Fact]
        public void Submit_WhenContainsNoErrorsOrWarnings_SubmitsWithNoException()
        {
            var memberUpload = new MemberUpload(
                A.Dummy<Guid>(),
                A.Dummy<string>(),
                null,
                A.Dummy<int>(),
                A.Dummy<int>(),
                A.Dummy<Scheme>(),
                A.Dummy<string>());

            var exception = Record.Exception(() => memberUpload.Submit(A.Dummy<User>()));

            Assert.Null(exception);
            Assert.True(memberUpload.IsSubmitted);
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
                0,
                A.Dummy<StatusType>());

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
            registeredProducer.Remove();

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
                0,
                A.Dummy<StatusType>());

            memberUpload.ProducerSubmissions.Add(producer);
            memberUpload.Submit(A.Dummy<User>());

            memberUpload.AssignToInvoiceRun(A.Dummy<InvoiceRun>());

            Assert.False(producer.Invoiced);
        }

        [Fact]
        public void GetNumberOfWarnings_WithNullErrorsCollection_ReturnsZero()
        {
            // Arrange
            var memberUpload = new MemberUpload(
                A.Dummy<Guid>(),
                A.Dummy<string>(),
                null,
                A.Dummy<int>(),
                A.Dummy<int>(),
                A.Dummy<Scheme>(),
                A.Dummy<string>());

            // Act
            var result = memberUpload.GetNumberOfWarnings();

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void GetNumberOfWarnings_CountsWarningsOnlyFromErrorsCollection()
        {
            // Arrange
            var errorsAndWarnings = new List<MemberUploadError>
            {
                new MemberUploadError(Error.ErrorLevel.Warning, A.Dummy<Error.UploadErrorType>(), "Description1"),
                new MemberUploadError(Error.ErrorLevel.Error, A.Dummy<Error.UploadErrorType>(), "Description2"),
                new MemberUploadError(Error.ErrorLevel.Warning, A.Dummy<Error.UploadErrorType>(), "Description3"),
            };

            var memberUpload = new MemberUpload(
                A.Dummy<Guid>(),
                A.Dummy<string>(),
                errorsAndWarnings,
                A.Dummy<int>(),
                A.Dummy<int>(),
                A.Dummy<Scheme>(),
                A.Dummy<string>());

            // Act
            var result = memberUpload.GetNumberOfWarnings();

            // Assert
            Assert.Equal(2, result);
        }
    }
}
