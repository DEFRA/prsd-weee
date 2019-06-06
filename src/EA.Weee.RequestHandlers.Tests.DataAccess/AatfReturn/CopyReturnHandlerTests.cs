namespace EA.Weee.RequestHandlers.Tests.DataAccess.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using Core.AatfReturn;
    using Core.Helpers;
    using Domain;
    using Domain.AatfReturn;
    using Domain.DataReturns;
    using Domain.Scheme;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core;
    using Prsd.Core.Domain;
    using RequestHandlers.AatfReturn;
    using Requests.AatfReturn;
    using Weee.DataAccess;
    using Weee.Tests.Core;
    using Weee.Tests.Core.Model;
    using Xunit;
    using AatfSize = Domain.AatfReturn.AatfSize;
    using AatfStatus = Domain.AatfReturn.AatfStatus;
    using Country = Domain.Country;
    using FacilityType = Domain.AatfReturn.FacilityType;
    using NonObligatedWeee = Domain.AatfReturn.NonObligatedWeee;
    using Organisation = Domain.Organisation.Organisation;
    using Return = Domain.AatfReturn.Return;
    using ReturnReportOn = Domain.AatfReturn.ReturnReportOn;
    using ReturnScheme = Domain.AatfReturn.ReturnScheme;
    using ReturnStatus = Domain.AatfReturn.ReturnStatus;
    using Scheme = Domain.Scheme.Scheme;
    using WeeeReceived = Domain.AatfReturn.WeeeReceived;
    using WeeeReceivedAmount = Domain.AatfReturn.WeeeReceivedAmount;
    using WeeeReused = Domain.AatfReturn.WeeeReused;
    using WeeeReusedAmount = Domain.AatfReturn.WeeeReusedAmount;
    using WeeeReusedSite = Domain.AatfReturn.WeeeReusedSite;
    using WeeeSentOn = Domain.AatfReturn.WeeeSentOn;
    using WeeeSentOnAmount = Domain.AatfReturn.WeeeSentOnAmount;

    public class CopyReturnHandlerTests
    {
        private CopyReturnHandler handler;
        private DatabaseWrapper database;
        private Return @return;
        private Return copiedReturn;
        private Country country;
        private EA.Weee.Domain.UKCompetentAuthority competentAuthority;
        private Organisation organisation;

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            handler = new CopyReturnHandler(authorization,
                A.Dummy<WeeeContext>(),
                A.Dummy<IUserContext>());

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<CopyReturn>());

            await action.Should().ThrowAsync<SecurityException>();   
        }

        [Fact]
        public async Task HandleAsync_NoOrganisationAccess_ThrowsSecurityException()
        {
            using (database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);
                var domainHelper = new DomainHelper(database.WeeeContext);

                await CreateReturnToCopy();

                var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

                handler = new CopyReturnHandler(authorization,
                    database.WeeeContext,
                    A.Dummy<IUserContext>());

                Func<Task> action = async () => await handler.HandleAsync(new CopyReturn(@return.Id));

                await action.Should().ThrowAsync<SecurityException>();
            }
        }

        [Fact]
        public async Task HandleAsync_NoReturnFound_ArgumentExceptionExpected()
        {
            using (database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);
                var domainHelper = new DomainHelper(database.WeeeContext);

                var authorization = new AuthorizationBuilder().AllowEverything().Build();

                handler = new CopyReturnHandler(authorization,
                    database.WeeeContext,
                    A.Dummy<IUserContext>());

                Func<Task> assertAction = async () => await handler.HandleAsync(new CopyReturn(Guid.NewGuid()));

                await assertAction.Should().ThrowAsync<ArgumentException>();
            }
        }

        [Fact]
        public async Task HandleAsync_ReturnCopied_CopiedReturnShouldBeAddedToContext()
        {
            void Action(Guid id)
            {   
                copiedReturn.Should().NotBeNull();
                copiedReturn.SubmittedById.Should().BeNull();
                copiedReturn.SubmittedBy.Should().BeNull();
                copiedReturn.CreatedBy.Should().BeNull();
                copiedReturn.CreatedById.ToLower().Should().Be(database.Model.AspNetUsers.First().Id.ToLower());
                copiedReturn.CreatedDate.Should().BeSameDateAs(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day));
                copiedReturn.ReturnStatus.Should().Be(ReturnStatus.Created);
                copiedReturn.ParentId.Should().Be(@return.Id);
            }

            await ActionAndAssert(Action);
        }

        [Fact]
        public async Task HandleAsync_GivenReturnReportsOn_CopiedReturnShouldHaveSameValues()
        {
            void Action(Guid id)
            {
                var copiedReportsOn = database.WeeeContext.ReturnReportOns.Where(r => r.ReturnId == copiedReturn.Id).ToList();

                var originalReports = database.WeeeContext.ReturnReportOns.Where(r => r.ReturnId == @return.Id).ToList();

                originalReports.Count().Should().NotBe(0);
                copiedReportsOn.Count().Should().Be(originalReports.Count());
                copiedReportsOn.Select(r => r.Id).ToList().Should().NotContain(originalReports.Select(r => r.Id).ToList());
                copiedReportsOn.Select(r => r.ReportOnQuestionId).ToList().Should().Contain(originalReports.Select(r => r.ReportOnQuestionId).ToList());
            }

            await ActionAndAssert(Action);
        }

        [Fact]
        public async Task HandleAsync_GivenReturnNonObligated_CopiedReturnShouldHaveSameValues()
        {
            void Action(Guid id)
            {
                var copiedNonObligated = database.WeeeContext.NonObligatedWeee.Where(r => r.ReturnId == copiedReturn.Id).ToList();

                var originalNonObligated = database.WeeeContext.NonObligatedWeee.Where(r => r.ReturnId == @return.Id).ToList();

                originalNonObligated.Count().Should().NotBe(0);
                copiedNonObligated.Count().Should().Be(originalNonObligated.Count());
                copiedNonObligated.Select(r => r.Id).ToList().Should().NotContain(originalNonObligated.Select(r => r.Id).ToList());

                foreach (var copiedNonObligatedWeee in copiedNonObligated)
                {
                    originalNonObligated.Where(n =>
                        n.Tonnage == copiedNonObligatedWeee.Tonnage && n.CategoryId == copiedNonObligatedWeee.CategoryId
                                                                    && n.Dcf == copiedNonObligatedWeee.Dcf).Should().NotBeNull();
                }              
            }

            await ActionAndAssert(Action);
        }

        [Fact]
        public async Task HandleAsync_GivenReturnWeeeReceived_CopiedReturnShouldHaveSameValues()
        {
            void Action(Guid id)
            {
                var copiedWeeeReceived = database.WeeeContext.WeeeReceived.Where(r => r.ReturnId == copiedReturn.Id).Include(w => w.WeeeReceivedAmounts).ToList();

                var originalWeeeReceived = database.WeeeContext.WeeeReceived.Where(r => r.ReturnId == @return.Id).Include(w => w.WeeeReceivedAmounts).ToList();

                originalWeeeReceived.Count().Should().NotBe(0);
                copiedWeeeReceived.Count().Should().Be(originalWeeeReceived.Count());
                copiedWeeeReceived.Select(r => r.Id).ToList().Should().NotContain(originalWeeeReceived.Select(r => r.Id).ToList());

                foreach (var copiedNonObligatedWeee in copiedWeeeReceived)
                {
                    originalWeeeReceived.Where(n => n.AatfId == copiedNonObligatedWeee.AatfId && n.SchemeId == copiedNonObligatedWeee.SchemeId).Should().NotBeNull();
                }

                var originalWeeeReceivedAmounts = originalWeeeReceived.SelectMany(w => w.WeeeReceivedAmounts).ToList();
                var copiedWeeeReceivedAmounts = copiedWeeeReceived.SelectMany(w => w.WeeeReceivedAmounts).ToList();

                originalWeeeReceivedAmounts.Count().Should().NotBe(0);
                originalWeeeReceivedAmounts.Count().Should().Be(copiedWeeeReceivedAmounts.Count());
                originalWeeeReceivedAmounts.Select(w => w.Id).ToList().Should()
                    .NotContain(copiedWeeeReceivedAmounts.Select(r => r.Id).ToList());

                foreach (var copiedWeeeReceivedAmount in copiedWeeeReceivedAmounts)
                {
                    originalWeeeReceivedAmounts.Where(n =>
                        n.CategoryId == copiedWeeeReceivedAmount.CategoryId &&
                        n.HouseholdTonnage == copiedWeeeReceivedAmount.HouseholdTonnage &&
                        n.NonHouseholdTonnage == copiedWeeeReceivedAmount.NonHouseholdTonnage).Should().NotBeNull();
                }
            }

            await ActionAndAssert(Action);
        }

        [Fact]
        public async Task HandleAsync_GivenReturnWeeeSentOn_CopiedReturnShouldHaveSameValues()
        {
            void Action(Guid id)
            {
                var copiedWeeeSentOn = database.WeeeContext.WeeeSentOn.Where(r => r.ReturnId == copiedReturn.Id).Include(w => w.WeeeSentOnAmounts).ToList();

                var originalWeeeSentOn = database.WeeeContext.WeeeSentOn.Where(r => r.ReturnId == @return.Id).Include(w => w.WeeeSentOnAmounts).ToList();

                originalWeeeSentOn.Count().Should().NotBe(0);
                copiedWeeeSentOn.Count().Should().Be(originalWeeeSentOn.Count());
                copiedWeeeSentOn.Select(r => r.Id).ToList().Should().NotContain(originalWeeeSentOn.Select(r => r.Id).ToList());
                copiedWeeeSentOn.Select(r => r.OperatorAddress.Id).ToList().Should().NotContain(originalWeeeSentOn.Select(r => r.OperatorAddress.Id).ToList());
                copiedWeeeSentOn.Select(r => r.SiteAddress.Id).ToList().Should().NotContain(originalWeeeSentOn.Select(r => r.SiteAddress.Id).ToList());
                copiedWeeeSentOn.Select(r => r.OperatorAddress.Id).Should().NotContainNulls();
                copiedWeeeSentOn.Select(r => r.SiteAddress.Id).Should().NotContainNulls();

                foreach (var copiedNonObligatedWeee in copiedWeeeSentOn)
                {
                    originalWeeeSentOn.Where(n => n.AatfId == copiedNonObligatedWeee.AatfId).Should().NotBeNull();
                }

                var originalWeeeSentOnAmounts = originalWeeeSentOn.SelectMany(w => w.WeeeSentOnAmounts).ToList();
                var copiedWeeeSentOnAmounts = copiedWeeeSentOn.SelectMany(w => w.WeeeSentOnAmounts).ToList();

                originalWeeeSentOnAmounts.Count().Should().NotBe(0);
                originalWeeeSentOnAmounts.Count().Should().Be(copiedWeeeSentOnAmounts.Count());
                originalWeeeSentOnAmounts.Select(w => w.Id).ToList().Should()
                    .NotContain(copiedWeeeSentOnAmounts.Select(r => r.Id).ToList());

                foreach (var copiedWeeeReceivedAmount in copiedWeeeSentOnAmounts)
                {
                    originalWeeeSentOnAmounts.Where(n =>
                        n.CategoryId == copiedWeeeReceivedAmount.CategoryId &&
                        n.HouseholdTonnage == copiedWeeeReceivedAmount.HouseholdTonnage &&
                        n.NonHouseholdTonnage == copiedWeeeReceivedAmount.NonHouseholdTonnage).Should().NotBeNull();
                }
            }

            await ActionAndAssert(Action);
        }

        [Fact]
        public async Task HandleAsync_GivenReturnWeeeReused_CopiedReturnShouldHaveSameValues()
        {
            void Action(Guid id)
            {
                var copiedWeeeReused = database.WeeeContext.WeeeReused.Where(r => r.ReturnId == copiedReturn.Id)
                    .Include(w => w.WeeeReusedSites)
                    .Include(w => w.WeeeReusedAmounts).ToList();

                var originalWeeeReused = database.WeeeContext.WeeeReused.Where(r => r.ReturnId == @return.Id)
                    .Include(w => w.WeeeReusedSites)
                    .Include(w => w.WeeeReusedAmounts).ToList();

                originalWeeeReused.Count().Should().NotBe(0);
                copiedWeeeReused.Count().Should().Be(originalWeeeReused.Count());
                copiedWeeeReused.Select(r => r.Id).ToList().Should().NotContain(originalWeeeReused.Select(r => r.Id).ToList());

                foreach (var copiedNonObligatedWeee in copiedWeeeReused)
                {
                    originalWeeeReused.Where(n => n.AatfId == copiedNonObligatedWeee.AatfId).Should().NotBeNull();
                }

                var originalWeeeReusedAmounts = originalWeeeReused.SelectMany(w => w.WeeeReusedAmounts).ToList();
                var copiedWeeeReusedAmounts = copiedWeeeReused.SelectMany(w => w.WeeeReusedAmounts).ToList();

                originalWeeeReusedAmounts.Count().Should().NotBe(0);
                originalWeeeReusedAmounts.Count().Should().Be(copiedWeeeReusedAmounts.Count());
                originalWeeeReusedAmounts.Select(w => w.Id).ToList().Should()
                    .NotContain(copiedWeeeReusedAmounts.Select(r => r.Id).ToList());

                foreach (var copiedWeeeReceivedAmount in copiedWeeeReusedAmounts)
                {
                    originalWeeeReusedAmounts.Where(n =>
                        n.CategoryId == copiedWeeeReceivedAmount.CategoryId &&
                        n.HouseholdTonnage == copiedWeeeReceivedAmount.HouseholdTonnage &&
                        n.NonHouseholdTonnage == copiedWeeeReceivedAmount.NonHouseholdTonnage).Should().NotBeNull();
                }

                var originalWeeeReusedSites = originalWeeeReused.SelectMany(w => w.WeeeReusedSites).ToList();
                var copiedWeeeReusedSites = copiedWeeeReused.SelectMany(w => w.WeeeReusedSites).ToList();

                originalWeeeReusedSites.Count().Should().NotBe(0);
                originalWeeeReusedSites.Count().Should().Be(copiedWeeeReusedSites.Count());
                originalWeeeReusedSites.Select(w => w.Id).ToList().Should()
                    .NotContain(copiedWeeeReusedSites.Select(r => r.Id).ToList());
                originalWeeeReusedSites.Select(w => w.Address.Id).ToList().Should()
                    .NotContain(copiedWeeeReusedSites.Select(r => r.Address.Id).ToList());
            }

            await ActionAndAssert(Action);
        }

        [Fact]
        public async Task HandleAsync_GivenReturnSchemes_CopiedReturnShouldHaveSameValues()
        {
            void Action(Guid id)
            {
                var copiedReturnScheme = database.WeeeContext.ReturnScheme.Where(r => r.ReturnId == copiedReturn.Id).ToList();

                var originalReturnScheme = database.WeeeContext.ReturnScheme.Where(r => r.ReturnId == @return.Id).ToList();

                originalReturnScheme.Count().Should().NotBe(0);
                copiedReturnScheme.Count().Should().Be(originalReturnScheme.Count());
                copiedReturnScheme.Select(r => r.Id).ToList().Should().NotContain(originalReturnScheme.Select(r => r.Id).ToList());
                copiedReturnScheme.Select(r => r.SchemeId).ToList().Should().Contain(originalReturnScheme.Select(r => r.SchemeId).ToList());
            }

            await ActionAndAssert(Action);
        }

        [Fact]

        public async Task HandleAsync_ReturnCopied_CopiedOrganisationShouldBeTheSameAsOriginal()
        {
            void Action(Guid id)
            {
                copiedReturn.Organisation.Should().Be(@return.Organisation);
            }

            await ActionAndAssert(Action);
        }

        [Fact]
        public async Task HandleAsync_ReturnFound_OriginalReturnShouldBeInContext()
        {
            void Action(Guid id)
            {
                var originalReturn = database.WeeeContext.Returns.First(r => r.Id == @return.Id);

                originalReturn.Equals(@return).Should().BeTrue();
            }

            await ActionAndAssert(Action);
        }

        private async Task ActionAndAssert(Action<Guid> action)
        {
            using (database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);
                var domainHelper = new DomainHelper(database.WeeeContext);
                var userContext = A.Fake<IUserContext>();

                A.CallTo(() => userContext.UserId).Returns(Guid.Parse(database.Model.AspNetUsers.First().Id));

                await CreateReturnToCopy();
                
                var message = new CopyReturn(@return.Id);

                var authorization = new AuthorizationBuilder().AllowEverything().Build();
                
                handler = new CopyReturnHandler(authorization,
                    database.WeeeContext,
                    userContext);
                
                var result = await handler.HandleAsync(message);

                @return = database.WeeeContext.Returns.AsNoTracking().First(r => r.Id == message.ReturnId);
                copiedReturn = database.WeeeContext.Returns.First(r => r.Id == result);

                action(result);
            }
        }

        private async Task CreateReturnToCopy()
        {
            country = await database.WeeeContext.Countries.FirstAsync();
            competentAuthority = await database.WeeeContext.UKCompetentAuthorities.FirstAsync(c => c.Name == "Environment Agency");

            organisation = Organisation.CreateSoleTrader("Test Organisation");
            var quarter = new Quarter(2019, QuarterType.Q1);

            @return = new Domain.AatfReturn.Return(organisation, quarter, database.Model.AspNetUsers.First().Id) { ReturnStatus = ReturnStatus.Submitted };

            database.WeeeContext.Returns.Add(@return);

            await database.WeeeContext.SaveChangesAsync();

            await AddReturnReportsOn();

            await AddReturnScheme();

            await AddNonObligated();

            await AddWeeeReceived();

            await AddWeeSentOn();

            await AddWeeeReused();
        }

        private async Task AddWeeeReused()
        {
            var weeeReused = new List<WeeeReused>()
            {
                new WeeeReused(Aatf(), @return.Id),
                new WeeeReused(Aatf(), @return.Id)
            };

            var weeeReusedSites = new List<WeeeReusedSite>()
            {
                new WeeeReusedSite(weeeReused.ElementAt(0), AatfSiteAddress()),
                new WeeeReusedSite(weeeReused.ElementAt(1), AatfSiteAddress())
            };

            var weeeReusedAmounts = new List<WeeeReusedAmount>()
            {
                new WeeeReusedAmount(weeeReused.ElementAt(0), 1, 10, 20),
                new WeeeReusedAmount(weeeReused.ElementAt(0), 2, 30, 40),
                new WeeeReusedAmount(weeeReused.ElementAt(1), 3, null, 0),
                new WeeeReusedAmount(weeeReused.ElementAt(1), 4, 0, null)
            };

            database.WeeeContext.WeeeReusedSite.AddRange(weeeReusedSites);
            database.WeeeContext.WeeeReusedAmount.AddRange(weeeReusedAmounts);
            database.WeeeContext.WeeeReused.AddRange(weeeReused);

            await database.WeeeContext.SaveChangesAsync();
        }

        private async Task AddWeeSentOn()
        {
            var weeeSentOn = new List<WeeeSentOn>()
            {
                new WeeeSentOn(AatfSiteAddress(), AatfSiteAddress(), Aatf(), @return),
                new WeeeSentOn(AatfSiteAddress(), AatfSiteAddress(), Aatf(), @return)
            };

            var weeSentOnAmounts = new List<WeeeSentOnAmount>()
            {
                new WeeeSentOnAmount(weeeSentOn.ElementAt(0), 1, 10, 20),
                new WeeeSentOnAmount(weeeSentOn.ElementAt(0), 2, 30, 40),
                new WeeeSentOnAmount(weeeSentOn.ElementAt(1), 3, 0, null),
                new WeeeSentOnAmount(weeeSentOn.ElementAt(1), 4, null, 0)
            };

            database.WeeeContext.WeeeSentOnAmount.AddRange(weeSentOnAmounts);
            database.WeeeContext.WeeeSentOn.AddRange(weeeSentOn);

            await database.WeeeContext.SaveChangesAsync();
        }

        private async Task AddWeeeReceived()
        {
            var weeReceived = new List<WeeeReceived>()
            {
                new WeeeReceived(new Scheme(organisation), Aatf(), @return.Id),
                new WeeeReceived(new Scheme(organisation), Aatf(), @return.Id)
            };

            var weeeReceivedAmounts = new List<WeeeReceivedAmount>()
            {
                new WeeeReceivedAmount(weeReceived.ElementAt(0), 1, 100, 200),
                new WeeeReceivedAmount(weeReceived.ElementAt(0), 2, 300, 400),
                new WeeeReceivedAmount(weeReceived.ElementAt(1), 3, 0, null),
                new WeeeReceivedAmount(weeReceived.ElementAt(1), 4, null, 0)
            };

            database.WeeeContext.WeeeReceivedAmount.AddRange(weeeReceivedAmounts);
            database.WeeeContext.WeeeReceived.AddRange(weeReceived);

            await database.WeeeContext.SaveChangesAsync();
        }

        private Aatf Aatf()
        {
            var aatf = new Aatf("aatf", competentAuthority, "123", AatfStatus.Approved, organisation,
                AatfSiteAddress(), AatfSize.Large, DateTime.Now,
                new AatfContact("first", "last", "position", "address1", "address2", "town", "county", "postcode", country, "telephone", "email"), FacilityType.Aatf, 2019);
            return aatf;
        }

        private AatfAddress AatfSiteAddress()
        {
            return new AatfAddress("name", "address1", "address2", "town", "county", "postcode", country);
        }

        private async Task AddNonObligated()
        {
            var nonObligated = new List<NonObligatedWeee>()
            {
                new NonObligatedWeee(@return, 1, true, 20),
                new NonObligatedWeee(@return, 2, false, 30)
            };

            database.WeeeContext.NonObligatedWeee.AddRange(nonObligated);

            await database.WeeeContext.SaveChangesAsync();
        }

        private async Task AddReturnScheme()
        {
            var returnScheme = new List<ReturnScheme>()
            {
                new ReturnScheme(new Scheme(organisation), @return)
            };

            database.WeeeContext.ReturnScheme.AddRange(returnScheme);

            await database.WeeeContext.SaveChangesAsync();
        }

        private async Task AddReturnReportsOn()
        {
            var reportsOn = new List<ReturnReportOn>()
            {
                new ReturnReportOn(@return.Id, (int)ReportOnQuestionEnum.WeeeReceived)
            };

            database.WeeeContext.ReturnReportOns.AddRange(reportsOn);

            await database.WeeeContext.SaveChangesAsync();
        }
    }
}
