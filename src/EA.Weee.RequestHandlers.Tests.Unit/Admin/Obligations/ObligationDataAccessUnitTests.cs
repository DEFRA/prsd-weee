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
    using Domain.Scheme;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core;
    using Prsd.Core.Domain;
    using Xunit;

    public class ObligationDataAccessUnitTests
    {
        private readonly ObligationDataAccess obligationDataAccess;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IUserContext userContext;
        private readonly WeeeContext context;
        private readonly Fixture fixture;

        public ObligationDataAccessUnitTests()
        {
            genericDataAccess = A.Fake<IGenericDataAccess>();
            context = A.Fake<WeeeContext>();
            userContext = A.Fake<IUserContext>();
            fixture = new Fixture();

            obligationDataAccess = new ObligationDataAccess(userContext, genericDataAccess, context);
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
            var user = fixture.Create<Guid>();
            var obligationScheme = fixture.CreateMany<ObligationScheme>().ToList();

            A.CallTo(() => userContext.UserId).Returns(user);

            //act
            await obligationDataAccess.AddObligationUpload(authority, data, fileName, new List<ObligationUploadError>(),
                obligationScheme);

            //assert
            A.CallTo(() => genericDataAccess.Add(A<ObligationUpload>.That.Matches(o => o.Data.Equals(data) &&
                o.FileName.Equals(fileName) &&
                o.CompetentAuthority.Equals(authority) &&
                o.UploadedDate == date &&
                o.UploadedById.Equals(user.ToString()) &&
                o.ObligationUploadErrors.Count == 0 &&
                o.ObligationSchemes.SequenceEqual(obligationScheme)))).MustHaveHappenedOnceExactly();

            SystemTime.Unfreeze();
        }

        [Fact]
        public async Task AddObligationUpload_GivenUploadDataWithNoData_ObligationUploadShouldBeAddedToContext()
        {
            //arrange
            var date = new DateTime();
            SystemTime.Freeze(date);

            var authority = fixture.Create<UKCompetentAuthority>();
            var data = fixture.Create<string>();
            var fileName = fixture.Create<string>();
            var user = fixture.Create<Guid>();

            A.CallTo(() => userContext.UserId).Returns(user);

            //act
            await obligationDataAccess.AddObligationUpload(authority, data, fileName, new List<ObligationUploadError>(),
                new List<ObligationScheme>());

            //assert
            A.CallTo(() => genericDataAccess.Add(A<ObligationUpload>.That.Matches(o => o.Data.Equals(data) &&
                o.FileName.Equals(fileName) &&
                o.CompetentAuthority.Equals(authority) &&
                o.UploadedDate == date &&
                o.UploadedById.Equals(user.ToString()) &&
                o.ObligationUploadErrors.Count == 0 &&
                o.ObligationSchemes.Count == 0))).MustHaveHappenedOnceExactly();

            SystemTime.Unfreeze();
        }

        [Fact]
        public async Task AddObligationUpload_GivenUploadDataWithObligationSchemeUpdatesNoErrors_ObligationSchemeShouldBeUpdated()
        {
            //arrange
            var date = new DateTime();
            SystemTime.Freeze(date);

            var authority = fixture.Create<UKCompetentAuthority>();
            var data = fixture.Create<string>();
            var fileName = fixture.Create<string>();
            var user = fixture.Create<Guid>();
            var complianceYear = 2022;
            var obligationScheme = A.Fake<ObligationScheme>();
            A.CallTo(() => obligationScheme.ComplianceYear).Returns(complianceYear);
            var scheme = A.Fake<Scheme>();
            var innerObligationSchemeInComplianceYear = A.Fake<ObligationScheme>();
            var innerObligationSchemes = new List<ObligationScheme>() { innerObligationSchemeInComplianceYear };
            A.CallTo(() => innerObligationSchemeInComplianceYear.ComplianceYear).Returns(complianceYear);
            A.CallTo(() => scheme.ObligationSchemes).Returns(innerObligationSchemes);
            A.CallTo(() => obligationScheme.Scheme).Returns(scheme);
            var obligationSchemes = new List<ObligationScheme>() { obligationScheme };
            A.CallTo(() => userContext.UserId).Returns(user);
            var updatedObligationAmounts = fixture.CreateMany<ObligationSchemeAmount>().ToList();
            A.CallTo(() => obligationScheme.ObligationSchemeAmounts).Returns(updatedObligationAmounts);

            //act
            await obligationDataAccess.AddObligationUpload(authority, data, fileName, new List<ObligationUploadError>(),
                obligationSchemes);

            //assert
            A.CallTo(() => genericDataAccess.Add(A<ObligationUpload>.That.Matches(o => o.Data.Equals(data) &&
                o.FileName.Equals(fileName) &&
                o.CompetentAuthority.Equals(authority) &&
                o.UploadedDate == date &&
                o.UploadedById.Equals(user.ToString()) &&
                o.ObligationUploadErrors.Count == 0 &&
                !o.ObligationSchemes.Contains(obligationScheme)))).MustHaveHappenedOnceExactly();

            A.CallTo(() => innerObligationSchemeInComplianceYear.UpdateObligationUpload(A<ObligationUpload>.That.Matches(o =>
                o.Data.Equals(data) &&
                o.FileName.Equals(fileName) &&
                o.CompetentAuthority.Equals(authority) &&
                o.UploadedDate == date &&
                o.UploadedById.Equals(user.ToString())))).MustHaveHappenedOnceExactly();

            A.CallTo(() => innerObligationSchemeInComplianceYear.UpdateObligationSchemeAmounts(A<List<ObligationSchemeAmount>>.That.Matches(o => o.SequenceEqual(updatedObligationAmounts)))).MustHaveHappened();

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
            var user = fixture.Create<Guid>();
            A.CallTo(() => userContext.UserId).Returns(user);

            //act
            await obligationDataAccess.AddObligationUpload(authority, data, fileName, errors, new List<ObligationScheme>());

            //assert
            A.CallTo(() => genericDataAccess.Add(A<ObligationUpload>.That.Matches(o => o.Data.Equals(data) &&
                o.FileName.Equals(fileName) &&
                o.CompetentAuthority.Equals(authority) &&
                o.UploadedDate == date &&
                o.UploadedById.Equals(user.ToString()) &&
                o.ObligationUploadErrors.Count == 3 &&
                o.ObligationSchemes.Count == 0 &&
                o.ObligationUploadErrors.SequenceEqual(errors)))).MustHaveHappenedOnceExactly();

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
    }
}
