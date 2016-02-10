namespace EA.Weee.Email.Tests.Unit.EventHandlers
{
    using System;
    using System.Threading.Tasks;
    using Domain;
    using Domain.Events;
    using Domain.Scheme;
    using Email.EventHandlers;
    using FakeItEasy;
    using Xunit;
    using Scheme = Domain.Scheme.Scheme;

    public class SchemeMemberSubmissionEventHandlerTests
    {
        [Fact]
        public async Task HandleAsync_SendsEmailToCompetentAuthorityNotificationAddress()
        {
            // Arrange
            string emailAddress = "test@sfwltd.co.uk";
            var competentAuthority = new UKCompetentAuthority(Guid.NewGuid(), "Name", "Abbreviation", A.Dummy<Country>(), emailAddress);

            Scheme scheme = A.Fake<Scheme>();
            A.CallTo(() => scheme.CompetentAuthority)
                .Returns(competentAuthority);

            MemberUpload memberUpload = A.Fake<MemberUpload>();
            A.CallTo(() => memberUpload.Scheme)
                .Returns(scheme);
            A.CallTo(() => memberUpload.ComplianceYear)
                .Returns(2015);

            var schemeSubmissionEvent = new SchemeMemberSubmissionEvent(memberUpload);

            var emailService = A.Fake<IWeeeNotificationEmailService>();
            var handler = new SchemeMemberSubmissionEventHandler(emailService);

            // Act
            await handler.HandleAsync(schemeSubmissionEvent);

            // Assert
            A.CallTo(() => emailService.SendSchemeMemberSubmitted(emailAddress, A<string>._, A<int>._, A<int>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_SendsEmailWithSchemeName()
        {
            // Arrange
            var competentAuthority = A.Fake<UKCompetentAuthority>();

            Scheme scheme = A.Fake<Scheme>();
            A.CallTo(() => scheme.CompetentAuthority)
                .Returns(competentAuthority);
            A.CallTo(() => scheme.SchemeName)
                .Returns("Test Scheme Name");

            MemberUpload memberUpload = A.Fake<MemberUpload>();
            A.CallTo(() => memberUpload.Scheme)
                .Returns(scheme);
            A.CallTo(() => memberUpload.ComplianceYear)
                .Returns(2015);

            var schemeSubmissionEvent = new SchemeMemberSubmissionEvent(memberUpload);

            var emailService = A.Fake<IWeeeNotificationEmailService>();
            var handler = new SchemeMemberSubmissionEventHandler(emailService);

            // Act
            await handler.HandleAsync(schemeSubmissionEvent);

            // Assert
            A.CallTo(() => emailService.SendSchemeMemberSubmitted(A<string>._, "Test Scheme Name", A<int>._, A<int>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_SendsEmailWithMemberUploadComplianceYear()
        {
            // Arrange
            var competentAuthority = A.Fake<UKCompetentAuthority>();

            Scheme scheme = A.Fake<Scheme>();
            A.CallTo(() => scheme.CompetentAuthority)
                .Returns(competentAuthority);

            MemberUpload memberUpload = A.Fake<MemberUpload>();
            A.CallTo(() => memberUpload.Scheme)
                .Returns(scheme);
            A.CallTo(() => memberUpload.ComplianceYear)
                .Returns(2015);

            var schemeSubmissionEvent = new SchemeMemberSubmissionEvent(memberUpload);

            var emailService = A.Fake<IWeeeNotificationEmailService>();
            var handler = new SchemeMemberSubmissionEventHandler(emailService);

            // Act
            await handler.HandleAsync(schemeSubmissionEvent);

            // Assert
            A.CallTo(() => emailService.SendSchemeMemberSubmitted(A<string>._, A<string>._, 2015, A<int>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_SendsEmailWithNumberOfMemberUploadWarnings()
        {
            // Arrange
            var competentAuthority = A.Fake<UKCompetentAuthority>();

            Scheme scheme = A.Fake<Scheme>();
            A.CallTo(() => scheme.CompetentAuthority)
                .Returns(competentAuthority);

            MemberUpload memberUpload = A.Fake<MemberUpload>();
            A.CallTo(() => memberUpload.Scheme)
                .Returns(scheme);
            A.CallTo(() => memberUpload.ComplianceYear)
                .Returns(2015);
            A.CallTo(() => memberUpload.GetNumberOfWarnings())
                .Returns(5);

            var schemeSubmissionEvent = new SchemeMemberSubmissionEvent(memberUpload);

            var emailService = A.Fake<IWeeeNotificationEmailService>();
            var handler = new SchemeMemberSubmissionEventHandler(emailService);

            // Act
            await handler.HandleAsync(schemeSubmissionEvent);

            // Assert
            A.CallTo(() => emailService.SendSchemeMemberSubmitted(A<string>._, A<string>._, A<int>._, 5))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
