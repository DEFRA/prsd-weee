namespace EA.Weee.Email.Tests.Unit.EventHandlers.SchemeDataReturnSubmission
{
    using System;
    using System.Threading.Tasks;
    using Domain;
    using Domain.DataReturns;
    using Domain.Events;
    using Domain.Scheme;
    using Email.EventHandlers.SchemeDataReturnSubmission;
    using FakeItEasy;
    using Xunit;

    public class SchemeDataReturnSubmissionEventHandlerTests
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

            Quarter quarter = new Quarter(2016, QuarterType.Q1);

            DataReturn dataReturn = new DataReturn(scheme, quarter);
            DataReturnVersion dataReturnVersion = new DataReturnVersion(dataReturn);
            dataReturnVersion.Submit("testUserId");

            var schemeSubmissionEvent = new SchemeDataReturnSubmissionEvent(dataReturnVersion);

            var emailService = A.Fake<IWeeeEmailService>();
            var dataAccess = A.Dummy<ISchemeDataReturnSubmissionEventHandlerDataAccess>();
            var handler = new SchemeDataReturnSubmissionEventHandler(emailService, dataAccess);

            // Act
            await handler.HandleAsync(schemeSubmissionEvent);

            // Assert
            A.CallTo(() => emailService.SendSchemeDataReturnSubmitted(emailAddress, A<string>._, A<int>._, A<int>._, A<bool>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_SendsEmailWithSchemeName()
        {
            // Arrange
            string emailAddress = "test@sfwltd.co.uk";
            var competentAuthority = new UKCompetentAuthority(Guid.NewGuid(), "Name", "Abbreviation", A.Dummy<Country>(), emailAddress);

            Scheme scheme = A.Fake<Scheme>();
            A.CallTo(() => scheme.CompetentAuthority)
                .Returns(competentAuthority);
            A.CallTo(() => scheme.SchemeName)
                .Returns("Test Scheme Name");

            Quarter quarter = new Quarter(2016, QuarterType.Q1);

            DataReturn dataReturn = new DataReturn(scheme, quarter);
            DataReturnVersion dataReturnVersion = new DataReturnVersion(dataReturn);
            dataReturnVersion.Submit("testUserId");

            var schemeSubmissionEvent = new SchemeDataReturnSubmissionEvent(dataReturnVersion);

            var emailService = A.Fake<IWeeeEmailService>();
            var dataAccess = A.Dummy<ISchemeDataReturnSubmissionEventHandlerDataAccess>();
            var handler = new SchemeDataReturnSubmissionEventHandler(emailService, dataAccess);

            // Act
            await handler.HandleAsync(schemeSubmissionEvent);

            // Assert
            A.CallTo(() => emailService.SendSchemeDataReturnSubmitted(A<string>._, "Test Scheme Name", A<int>._, A<int>._, A<bool>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_SendsEmailWithDataReturnYear()
        {
            // Arrange
            string emailAddress = "test@sfwltd.co.uk";
            var competentAuthority = new UKCompetentAuthority(Guid.NewGuid(), "Name", "Abbreviation", A.Dummy<Country>(), emailAddress);

            Scheme scheme = A.Fake<Scheme>();
            A.CallTo(() => scheme.CompetentAuthority)
                .Returns(competentAuthority);

            Quarter quarter = new Quarter(2015, QuarterType.Q1);

            DataReturn dataReturn = new DataReturn(scheme, quarter);
            DataReturnVersion dataReturnVersion = new DataReturnVersion(dataReturn);
            dataReturnVersion.Submit("testUserId");

            var schemeSubmissionEvent = new SchemeDataReturnSubmissionEvent(dataReturnVersion);

            var emailService = A.Fake<IWeeeEmailService>();
            var dataAccess = A.Dummy<ISchemeDataReturnSubmissionEventHandlerDataAccess>();
            var handler = new SchemeDataReturnSubmissionEventHandler(emailService, dataAccess);

            // Act
            await handler.HandleAsync(schemeSubmissionEvent);

            // Assert
            A.CallTo(() => emailService.SendSchemeDataReturnSubmitted(A<string>._, A<string>._, 2015, A<int>._, A<bool>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_SendsEmailWithDataReturnQuarter()
        {
            // Arrange
            string emailAddress = "test@sfwltd.co.uk";
            var competentAuthority = new UKCompetentAuthority(Guid.NewGuid(), "Name", "Abbreviation", A.Dummy<Country>(), emailAddress);

            Scheme scheme = A.Fake<Scheme>();
            A.CallTo(() => scheme.CompetentAuthority)
                .Returns(competentAuthority);

            Quarter quarter = new Quarter(2016, QuarterType.Q3);

            DataReturn dataReturn = new DataReturn(scheme, quarter);
            DataReturnVersion dataReturnVersion = new DataReturnVersion(dataReturn);
            dataReturnVersion.Submit("testUserId");

            var schemeSubmissionEvent = new SchemeDataReturnSubmissionEvent(dataReturnVersion);

            var emailService = A.Fake<IWeeeEmailService>();
            var dataAccess = A.Dummy<ISchemeDataReturnSubmissionEventHandlerDataAccess>();
            var handler = new SchemeDataReturnSubmissionEventHandler(emailService, dataAccess);

            // Act
            await handler.HandleAsync(schemeSubmissionEvent);

            // Assert
            A.CallTo(() => emailService.SendSchemeDataReturnSubmitted(A<string>._, A<string>._, A<int>._, 3, A<bool>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_InvokesDataAccess_GetNumberOfDataReturnSubmissionsAsync_WithCorrectParameters()
        {
            // Arrange
            string emailAddress = "test@sfwltd.co.uk";
            var competentAuthority = new UKCompetentAuthority(Guid.NewGuid(), "Name", "Abbreviation", A.Dummy<Country>(), emailAddress);

            Scheme scheme = A.Fake<Scheme>();
            A.CallTo(() => scheme.CompetentAuthority)
                .Returns(competentAuthority);

            Quarter quarter = new Quarter(2016, QuarterType.Q2);

            DataReturn dataReturn = new DataReturn(scheme, quarter);
            DataReturnVersion dataReturnVersion = new DataReturnVersion(dataReturn);
            dataReturnVersion.Submit("testUserId");

            var dataAccess = A.Fake<ISchemeDataReturnSubmissionEventHandlerDataAccess>();
            var emailService = A.Fake<IWeeeEmailService>();
            var handler = new SchemeDataReturnSubmissionEventHandler(emailService, dataAccess);

            var schemeSubmissionEvent = new SchemeDataReturnSubmissionEvent(dataReturnVersion);

            // Act
            await handler.HandleAsync(schemeSubmissionEvent);

            // Assert
            A.CallTo(() => dataAccess.GetNumberOfDataReturnSubmissionsAsync(scheme, 2016, QuarterType.Q2))
                .MustHaveHappened();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public async Task HandleAsync_SendsEmailWithIsResubmissionAsFalse_WhenNumberOfDataReturnSubmissionsIsZeroOrOne(int numberOfDataReturnSubmissions)
        {
            // Arrange
            string emailAddress = "test@sfwltd.co.uk";
            var competentAuthority = new UKCompetentAuthority(Guid.NewGuid(), "Name", "Abbreviation", A.Dummy<Country>(), emailAddress);

            Scheme scheme = A.Fake<Scheme>();
            A.CallTo(() => scheme.CompetentAuthority)
                .Returns(competentAuthority);

            Quarter quarter = new Quarter(2016, QuarterType.Q1);

            DataReturn dataReturn = new DataReturn(scheme, quarter);
            DataReturnVersion dataReturnVersion = new DataReturnVersion(dataReturn);
            dataReturnVersion.Submit("testUserId");

            var dataAccess = A.Fake<ISchemeDataReturnSubmissionEventHandlerDataAccess>();
            A.CallTo(() => dataAccess.GetNumberOfDataReturnSubmissionsAsync(scheme, 2016, QuarterType.Q1))
                .Returns(numberOfDataReturnSubmissions);

            var schemeSubmissionEvent = new SchemeDataReturnSubmissionEvent(dataReturnVersion);

            var emailService = A.Fake<IWeeeEmailService>();
            var handler = new SchemeDataReturnSubmissionEventHandler(emailService, dataAccess);

            // Act
            await handler.HandleAsync(schemeSubmissionEvent);

            // Assert
            A.CallTo(() => emailService.SendSchemeDataReturnSubmitted(A<string>._, A<string>._, A<int>._, A<int>._, false))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_SendsEmailWithIsResubmissionAsTrue_WhenNumberOfDataReturnSubmissionsIsGreaterThanOne()
        {
            // Arrange
            string emailAddress = "test@sfwltd.co.uk";
            var competentAuthority = new UKCompetentAuthority(Guid.NewGuid(), "Name", "Abbreviation", A.Dummy<Country>(), emailAddress);

            Scheme scheme = A.Fake<Scheme>();
            A.CallTo(() => scheme.CompetentAuthority)
                .Returns(competentAuthority);

            Quarter quarter = new Quarter(2016, QuarterType.Q1);

            DataReturn dataReturn = new DataReturn(scheme, quarter);
            DataReturnVersion dataReturnVersion = new DataReturnVersion(dataReturn);
            dataReturnVersion.Submit("testUserId");

            var dataAccess = A.Fake<ISchemeDataReturnSubmissionEventHandlerDataAccess>();
            A.CallTo(() => dataAccess.GetNumberOfDataReturnSubmissionsAsync(scheme, 2016, QuarterType.Q1))
                .Returns(2);

            var schemeSubmissionEvent = new SchemeDataReturnSubmissionEvent(dataReturnVersion);

            var emailService = A.Fake<IWeeeEmailService>();
            var handler = new SchemeDataReturnSubmissionEventHandler(emailService, dataAccess);

            // Act
            await handler.HandleAsync(schemeSubmissionEvent);

            // Assert
            A.CallTo(() => emailService.SendSchemeDataReturnSubmitted(A<string>._, A<string>._, A<int>._, A<int>._, true))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
