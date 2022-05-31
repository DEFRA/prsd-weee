﻿namespace EA.Weee.DataAccess.Tests.Integration.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Validation;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.Error;
    using Domain.Lookup;
    using Domain.Obligation;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core;
    using Prsd.Core.Domain;
    using RequestHandlers.Shared;
    using Weee.DataAccess.DataAccess;
    using Weee.Tests.Core.Model;
    using Xunit;
    using CompetentAuthority = Core.Shared.CompetentAuthority;

    public class ObligationDataAccessIntegration
    {
        [Fact]
        public async Task AddObligationUpload_WithNoErrors_ShouldAddObligationUpload()
        {
            using (var database = new DatabaseWrapper())
            {
                //arrange
                var context = database.WeeeContext;
                var userContext = A.Fake<IUserContext>();
                A.CallTo(() => userContext.UserId).Returns(Guid.Parse(context.GetCurrentUser()));

                var dataAccess = new ObligationDataAccess(database.WeeeContext, userContext, new GenericDataAccess(database.WeeeContext));
                var commonDataAccess = new CommonDataAccess(database.WeeeContext);
                var genericDataAccess = new GenericDataAccess(database.WeeeContext);

                var authority = await commonDataAccess.FetchCompetentAuthority(CompetentAuthority.England);

                var fileData = Faker.Lorem.Paragraph();
                var fileName = Faker.Lorem.GetFirstWord();

                //act
                var id = await dataAccess.AddObligationUpload(authority, fileData, fileName, new List<ObligationUploadError>());
                
                await context.SaveChangesAsync();

                var obligation = await genericDataAccess.GetById<ObligationUpload>(id);

                //assert
                obligation.Should().NotBeNull();
                obligation.Data.Should().Be(fileData);
                obligation.FileName.Should().Be(fileName);
                obligation.CompetentAuthority.Should().Be(authority);
                obligation.UploadedById.Should().Be(context.GetCurrentUser());
                obligation.UploadedDate.Should().BeCloseTo(SystemTime.UtcNow, TimeSpan.FromSeconds(10));
                obligation.ObligationUploadErrors.Should().BeEmpty();
            }
        }

        [Fact]
        public async Task AddObligationUpload_WithErrors_ShouldAddObligationUpload()
        {
            using (var database = new DatabaseWrapper())
            {
                //arrange
                var context = database.WeeeContext;
                var userContext = A.Fake<IUserContext>();
                A.CallTo(() => userContext.UserId).Returns(Guid.Parse(context.GetCurrentUser()));

                var dataAccess = new ObligationDataAccess(database.WeeeContext, userContext, new GenericDataAccess(database.WeeeContext));
                var commonDataAccess = new CommonDataAccess(database.WeeeContext);
                var genericDataAccess = new GenericDataAccess(database.WeeeContext);

                var authority = await commonDataAccess.FetchCompetentAuthority(CompetentAuthority.England);

                var fileData = Faker.Lorem.Paragraph();
                var fileName = Faker.Lorem.GetFirstWord();

                var errors = new List<ObligationUploadError>();
                const string description1 = "description1";
                const string scheme1 = "scheme1";
                const string schemeIdentifier1 = "Identifier1";

                errors.Add(new ObligationUploadError(ObligationUploadErrorType.Scheme, scheme1, schemeIdentifier1, description1));

                const string description2 = "description2";
                const string scheme2 = "scheme2";
                const string schemeIdentifier2 = "Identifier2";

                errors.Add(new ObligationUploadError(ObligationUploadErrorType.Data, WeeeCategory.ConsumerEquipment, scheme2, schemeIdentifier2, description2));

                const string description3 = "description3";

                errors.Add(new ObligationUploadError(ObligationUploadErrorType.File, description3));

                //act
                var id = await dataAccess.AddObligationUpload(authority, fileData, fileName, errors);

                var obligation = await genericDataAccess.GetById<ObligationUpload>(id);

                //assert
                obligation.Should().NotBeNull();
                obligation.Data.Should().Be(fileData);
                obligation.FileName.Should().Be(fileName);
                obligation.CompetentAuthority.Should().Be(authority);
                obligation.UploadedById.Should().Be(context.GetCurrentUser());
                obligation.UploadedDate.Should().BeCloseTo(SystemTime.UtcNow, TimeSpan.FromSeconds(10));
                obligation.ObligationUploadErrors.Count.Should().Be(3);
                obligation.ObligationUploadErrors.FirstOrDefault(e => 
                                                                e.SchemeIdentifier.Equals(schemeIdentifier1) &&
                                                                e.SchemeName.Equals(scheme1) &&
                                                                e.ErrorType.Value == ObligationUploadErrorType.Scheme.Value &&
                                                                e.Description.Equals(description1)).Should().NotBeNull();
                obligation.ObligationUploadErrors.FirstOrDefault(e => e.SchemeIdentifier.Equals(schemeIdentifier2) &&
                                                                      e.SchemeName.Equals(scheme2) &&
                                                                      e.ErrorType == ObligationUploadErrorType.Data &&
                                                                      e.Description.Equals(description2) &&
                                                                      e.Category == WeeeCategory.ConsumerEquipment).Should().NotBeNull();
                obligation.ObligationUploadErrors.FirstOrDefault(e => e.SchemeIdentifier == null &&
                                                                      e.SchemeName == null &&
                                                                      e.ErrorType == ObligationUploadErrorType.File &&
                                                                      e.Description.Equals(description3) &&
                                                                      e.Category == null).Should().NotBeNull();
            }
        }
    }
}