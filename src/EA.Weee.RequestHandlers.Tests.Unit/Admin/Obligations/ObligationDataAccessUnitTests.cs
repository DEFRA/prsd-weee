namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.Obligations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using DataAccess;
    using DataAccess.DataAccess;
    using Domain;
    using Domain.Error;
    using Domain.Obligation;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core;
    using Prsd.Core.Domain;
    using Xunit;

    public class ObligationDataAccessUnitTests
    {
        private readonly ObligationDataAccess obligationDataAccess;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly WeeeContext context;
        private readonly Fixture fixture;
        private Guid userId;

        public ObligationDataAccessUnitTests()
        {
            genericDataAccess = A.Fake<IGenericDataAccess>();
            context = A.Fake<WeeeContext>();
            var userContext = A.Fake<IUserContext>();
            userId = Guid.NewGuid();
            fixture = new Fixture();

            A.CallTo(() => userContext.UserId).Returns(userId);

            obligationDataAccess = new ObligationDataAccess(context, userContext, genericDataAccess);
        }

        [Fact]
        public async Task AddObligationUpload_GivenUploadDataWithNoErrors_ObligationUploadShouldBeAddedToContext()
        {
            //arrange
            var date = new DateTime();
            SystemTime.Freeze(date);

            var authority = fixture.Create<UKCompetentAuthority>();
            var data = fixture.Create<string>();
            var fileName = fixture.Create<string>();

            //act
            await obligationDataAccess.AddObligationUpload(authority, data, fileName, new List<ObligationUploadError>(), new List<ObligationScheme>());

            //assert
            A.CallTo(() => genericDataAccess.Add(A<ObligationUpload>.That.Matches(o => o.Data.Equals(data) &&
                o.FileName.Equals(fileName) &&
                o.CompetentAuthority.Equals(authority) &&
                o.UploadedDate == date &&
                o.UploadedById.Equals(userId.ToString()) &&
                o.ObligationUploadErrors.Count.Equals(0)))).MustHaveHappenedOnceExactly();

            SystemTime.Unfreeze();
        }

        [Fact]
        public async Task AddObligationUpload_GivenUploadDataWithErrors_ObligationUploadShouldBeAddedToContext()
        {
            //arrange
            var date = new DateTime();
            SystemTime.Freeze(date);

            var authority = fixture.Create<UKCompetentAuthority>();
            var data = fixture.Create<string>();
            var fileName = fixture.Create<string>();
            var errors = new List<ObligationUploadError>()
            {
                new ObligationUploadError(ObligationUploadErrorType.Scheme, "desc"),
                new ObligationUploadError(ObligationUploadErrorType.Data, "desc"),
                new ObligationUploadError(ObligationUploadErrorType.File, "desc")
            };

            //act
            await obligationDataAccess.AddObligationUpload(authority, data, fileName, errors, new List<ObligationScheme>());

            //assert
            A.CallTo(() => genericDataAccess.Add(A<ObligationUpload>.That.Matches(o => o.Data.Equals(data) &&
                o.FileName.Equals(fileName) &&
                o.CompetentAuthority.Equals(authority) &&
                o.UploadedDate == date &&
                o.UploadedById.Equals(userId.ToString()) &&
                o.ObligationUploadErrors.Count.Equals(3)))).MustHaveHappenedOnceExactly();

            foreach (var error in errors)
            {
                A.CallTo(() => genericDataAccess.Add(A<ObligationUpload>.That.Matches(o => o.ObligationUploadErrors.FirstOrDefault(c =>
                    c.ErrorType.Equals(error.ErrorType)) != null))).MustHaveHappenedOnceExactly();
            }

            SystemTime.Unfreeze();
        }

        [Fact]
        public async Task AddObligationUpload_GivenAddedObligationUpload_ObligationUploadIdShouldBeReturned()
        {
            //arrange
            var obligationUpload = A.Fake<ObligationUpload>();
            var id = Guid.NewGuid();
            A.CallTo(() => obligationUpload.Id).Returns(id);
            A.CallTo(() => genericDataAccess.Add(A<ObligationUpload>._)).Returns(obligationUpload);

            //act
            var result = await obligationDataAccess.AddObligationUpload(fixture.Create<UKCompetentAuthority>(), 
                fixture.Create<string>(), fixture.Create<string>(), fixture.CreateMany<ObligationUploadError>().ToList(), fixture.CreateMany<ObligationScheme>().ToList());

            //assert
            result.Should().Be(id);
        }

        [Fact]
        public async Task AddObligationUpload_GivenAddedObligationUpload_ObligationUploadShouldBeAddedAndSaveChangesCalled()
        {
            //act
            var result = await obligationDataAccess.AddObligationUpload(fixture.Create<UKCompetentAuthority>(),
                fixture.Create<string>(), fixture.Create<string>(), fixture.CreateMany<ObligationUploadError>().ToList(),
                fixture.CreateMany<ObligationScheme>().ToList());

            //assert
            A.CallTo(() => genericDataAccess.Add(A<ObligationUpload>._)).MustHaveHappenedOnceExactly().Then(
                A.CallTo(() => context.SaveChangesAsync()).MustHaveHappenedOnceExactly());
        }
    }
}
