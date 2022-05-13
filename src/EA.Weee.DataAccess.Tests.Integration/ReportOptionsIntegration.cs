namespace EA.Weee.DataAccess.Tests.Integration
{
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.NonObligated;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedReceived;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedReused;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedSentOn;
    using EA.Weee.RequestHandlers.AatfReturn.Specification;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Tests.Core.Model;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Weee.DataAccess.DataAccess;
    using Weee.Tests.Core;
    using Xunit;
    using NonObligatedWeee = Domain.AatfReturn.NonObligatedWeee;
    using Organisation = Domain.Organisation.Organisation;
    using Return = Domain.AatfReturn.Return;
    using ReturnReportOn = Domain.AatfReturn.ReturnReportOn;
    using ReturnScheme = Domain.AatfReturn.ReturnScheme;
    using Scheme = Domain.Scheme.Scheme;
    using WeeeReceived = Domain.AatfReturn.WeeeReceived;
    using WeeeReceivedAmount = Domain.AatfReturn.WeeeReceivedAmount;
    using WeeeReused = Domain.AatfReturn.WeeeReused;
    using WeeeReusedAmount = Domain.AatfReturn.WeeeReusedAmount;
    using WeeeReusedSite = Domain.AatfReturn.WeeeReusedSite;
    using WeeeSentOn = Domain.AatfReturn.WeeeSentOn;
    using WeeeSentOnAmount = Domain.AatfReturn.WeeeSentOnAmount;

    public class ReportOptionsIntegration
    {
        private readonly Fixture fixture;

        public ReportOptionsIntegration()
        {
            fixture = new Fixture();
        }

        [Fact]
        public async Task CanCreateReturnReportOnEntry()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new GenericDataAccess(context);

                var organisation = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var @return = await CreateReturn(context, database, organisation);

                var questions = context.ReportOnQuestions.ToList();

                var returnReportOn = new List<ReturnReportOn>();

                foreach (var question in questions)
                {
                    returnReportOn.Add(new ReturnReportOn(@return.Id, question.Id));
                }

                await dataAccess.AddMany<ReturnReportOn>(returnReportOn);

                var result = context.ReturnReportOns.Where(r => r.ReturnId == @return.Id).ToList();

                result.Count.Should().Be(questions.Count);
                foreach (var question in questions)
                {
                    result.Where(r => r.ReportOnQuestionId == question.Id).ToList().Count.Should().Be(1);
                }
            }
        }

        [Fact]
        public async Task DeselectedOptionAndDataIsDeleted()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new GenericDataAccess(context);

                var organisation = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var @return = await CreateReturn(context, database, organisation);
                var country = await context.Countries.SingleAsync(c => c.Name == "France");

                var questions = context.ReportOnQuestions.ToList();

                var returnReportOn = new List<ReturnReportOn>();

                foreach (var question in questions)
                {
                    returnReportOn.Add(new ReturnReportOn(@return.Id, question.Id));
                }

                var aatf = await CreateAatf(database, @return);
                var scheme = await CreateScheme(context, organisation);
                var sentOnSiteAddress = await CreateAddress(database);
                var sentOnSOperatorAddress = await CreateAddress(database);
                var reusedSiteAddress = await CreateAddress(database);

                await dataAccess.AddMany<ReturnReportOn>(returnReportOn);

                await CreateWeeeReusedAmounts(new ObligatedReusedDataAccess(context), new AatfSiteDataAccess(context, dataAccess), @return, aatf, reusedSiteAddress);
                await CreateWeeeReceivedAmounts(new ObligatedReceivedDataAccess(context), new ReturnSchemeDataAccess(context), @return, aatf, scheme);
                await CreateWeeeSentOnAmounts(new ObligatedSentOnDataAccess(context), @return, aatf, sentOnSiteAddress, sentOnSOperatorAddress);
                await CreateNonObligatedWeee(new NonObligatedDataAccess(context), @return);

                var submittedReturnOptions = context.ReturnReportOns.Where(r => r.ReturnId == @return.Id).ToList();

                var(submittedWeeeReused, submittedWeeeReusedAddresses, submittedWeeeReusedAmounts, submittedWeeeReusedSites) = await RetrieveSubmittedWeeeReusedData(context, @return, dataAccess);

                var(submittedWeeeReceived, submittedWeeeReturnScheme, submittedWeeeReceivedAmounts) = await RetrieveSubmittedWeeeReceivedData(context, @return, dataAccess);

                var(submittedWeeeSentOn, submittedWeeeSentOnAddresses, submittedWeeeSentOnAmounts) = await RetrieveSubmittedWeeeSentOnData(context, @return, dataAccess);

                var submittedNonObligatedWeee = context.NonObligatedWeee.Where(w => w.ReturnId == @return.Id).ToList();

                var handler = new AddReturnReportOnHandler(A.Fake<IWeeeAuthorization>(), dataAccess, context);

                var deselectOptions = new List<int>() { 1, 2, 3, 4, 5 };
                var coreReportOnQuestions = CreateReportQuestions();

                var request = new AddReturnReportOn() { DeselectedOptions = deselectOptions, Options = coreReportOnQuestions, ReturnId = @return.Id, SelectedOptions = new List<int>() };

                await handler.HandleAsync(request);

                AssertWeeeReceivedDeletion(context, submittedWeeeReceived, submittedWeeeReturnScheme, submittedWeeeReceivedAmounts);
                AssertWeeeReusedDeletion(context, submittedWeeeReused, submittedWeeeReusedAddresses, submittedWeeeReusedAmounts, submittedWeeeReusedSites);
                AssertWeeeSentOnDeletion(context, submittedWeeeSentOn, submittedWeeeSentOnAddresses, submittedWeeeSentOnAmounts);
                AssertNonObligatedWeeeDeletion(context, submittedNonObligatedWeee);
            }
        }

        [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1009:ClosingParenthesisMustBeSpacedCorrectly", Justification = "C# 7 Tuple")]
        private static async Task<(List<WeeeSentOn> submittedWeeeSentOn, List<AatfAddress> submittedWeeeSentOnAddresses, List<WeeeSentOnAmount> submittedWeeeSentOnAmounts)> RetrieveSubmittedWeeeSentOnData(WeeeContext context, Return @return,
            GenericDataAccess dataAccess)
        {
            var submittedWeeeSentOn = context.WeeeSentOn.Where(w => w.ReturnId == @return.Id).ToList();
            var submittedWeeeSentOnAddresses = new List<AatfAddress>();
            var submittedWeeeSentOnAmounts = new List<WeeeSentOnAmount>();
            foreach (var weeeSentOn in submittedWeeeSentOn)
            {
                submittedWeeeSentOnAmounts.AddRange(
                    await dataAccess.GetManyByExpression(new WeeeSentOnAmountByWeeeSentOnIdSpecification(weeeSentOn.Id)));
                if (weeeSentOn.OperatorAddress != null)
                {
                    submittedWeeeSentOnAddresses.Add(weeeSentOn.OperatorAddress);
                }

                submittedWeeeSentOnAddresses.Add(weeeSentOn.SiteAddress);
            }

            return (submittedWeeeSentOn, submittedWeeeSentOnAddresses, submittedWeeeSentOnAmounts);
        }

        [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1009:ClosingParenthesisMustBeSpacedCorrectly", Justification = "C# 7 Tuple")]
        private static async Task<(List<WeeeReceived> submittedWeeeReceived, List<ReturnScheme> submittedWeeeReturnScheme, List<WeeeReceivedAmount> submittedWeeeReceivedAmounts)> RetrieveSubmittedWeeeReceivedData(WeeeContext context, Return @return,
            GenericDataAccess dataAccess)
        {
            var submittedWeeeReceived = context.WeeeReceived.Where(w => w.ReturnId == @return.Id).ToList();
            var submittedWeeeReturnScheme = context.ReturnScheme.Where(w => w.Return.Id == @return.Id).ToList();
            var submittedWeeeReceivedAmounts = new List<WeeeReceivedAmount>();
            foreach (var weeeReceived in submittedWeeeReceived)
            {
                submittedWeeeReceivedAmounts.AddRange(
                    await dataAccess.GetManyByExpression(new WeeeReceivedAmountByWeeeReceivedIdSpecification(weeeReceived.Id)));
            }

            return (submittedWeeeReceived, submittedWeeeReturnScheme, submittedWeeeReceivedAmounts);
        }

        [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1009:ClosingParenthesisMustBeSpacedCorrectly", Justification = "C# 7 Tuple")]
        private static async Task<(List<WeeeReused> submittedWeeeReused, List<AatfAddress> submittedWeeeReusedAddresses, List<WeeeReusedAmount> submittedWeeeReusedAmounts, List<WeeeReusedSite> submittedWeeeReusedSites)> RetrieveSubmittedWeeeReusedData(WeeeContext context, Return @return, GenericDataAccess dataAccess)
        {
            var submittedWeeeReused = context.WeeeReused.Where(w => w.ReturnId == @return.Id).ToList();
            var submittedWeeeReusedAddresses = new List<AatfAddress>();
            var submittedWeeeReusedAmounts = new List<WeeeReusedAmount>();
            var submittedWeeeReusedSites = new List<WeeeReusedSite>();
            foreach (var weeeReused in submittedWeeeReused)
            {
                submittedWeeeReusedAmounts.AddRange(
                    await dataAccess.GetManyByExpression(new WeeeReusedAmountByWeeeReusedIdSpecification(weeeReused.Id)));
                submittedWeeeReusedSites.AddRange(
                    await dataAccess.GetManyByExpression(new WeeeReusedSiteByWeeeReusedIdSpecification(weeeReused.Id)));
            }

            foreach (var site in submittedWeeeReusedSites)
            {
                submittedWeeeReusedAddresses.Add(site.Address);
            }

            return (submittedWeeeReused, submittedWeeeReusedAddresses, submittedWeeeReusedAmounts, submittedWeeeReusedSites);
        }

        private static void AssertNonObligatedWeeeDeletion(WeeeContext context, List<NonObligatedWeee> submittedNonObligatedWeee)
        {
            foreach (var item in submittedNonObligatedWeee)
            {
                context.NonObligatedWeee.Count(n => n.Id == item.Id).Should().Be(0);
            }
        }

        private static void AssertWeeeSentOnDeletion(WeeeContext context, List<WeeeSentOn> submittedWeeeSentOn, List<AatfAddress> submittedWeeeSentOnAddresses, List<WeeeSentOnAmount> submittedWeeeSentOnAmounts)
        {
            foreach (var item in submittedWeeeSentOn)
            {
                context.WeeeSentOn.Count(w => w.Id == item.Id).Should().Be(0);
            }

            foreach (var item in submittedWeeeSentOnAmounts)
            {
                context.WeeeSentOnAmount.Count(w => w.Id == item.Id).Should().Be(0);
            }

            foreach (var item in submittedWeeeSentOnAddresses)
            {
                context.AatfAddress.Count(a => a.Id == item.Id).Should().Be(0);
            }
        }

        private static void AssertWeeeReusedDeletion(WeeeContext context, List<WeeeReused> submittedWeeeReused, List<AatfAddress> submittedWeeeReusedAddresses, List<WeeeReusedAmount> submittedWeeeReusedAmounts, List<WeeeReusedSite> submittedWeeeReusedSites)
        {
            foreach (var item in submittedWeeeReused)
            {
                context.WeeeReused.Count(w => w.Id == item.Id).Should().Be(0);
            }

            foreach (var item in submittedWeeeReusedAmounts)
            {
                context.WeeeReusedAmount.Count(w => w.Id == item.Id).Should().Be(0);
            }

            foreach (var item in submittedWeeeReusedSites)
            {
                context.WeeeReusedSite.Count(w => w.Id == item.Id).Should().Be(0);
            }

            foreach (var item in submittedWeeeReusedAddresses)
            {
                context.AatfAddress.Count(a => a.Id == item.Id).Should().Be(0);
            }
        }

        private static void AssertWeeeReceivedDeletion(WeeeContext context, List<WeeeReceived> submittedWeeeReceived, List<Domain.AatfReturn.ReturnScheme> submittedWeeeReturnScheme, List<WeeeReceivedAmount> submittedWeeeReceivedAmounts)
        {
            foreach (var item in submittedWeeeReceived)
            {
                context.WeeeReceived.Count(w => w.Id == item.Id).Should().Be(0);
            }

            foreach (var item in submittedWeeeReturnScheme)
            {
                context.ReturnScheme.Count(w => w.Id == item.Id).Should().Be(0);
            }

            foreach (var item in submittedWeeeReceivedAmounts)
            {
                context.WeeeReceivedAmount.Count(w => w.Id == item.Id).Should().Be(0);
            }
        }

        private static async Task<AatfAddress> CreateAddress(DatabaseWrapper database)
        {
            var address = ObligatedWeeeIntegrationCommon.CreateAatfAddress(database);

            database.WeeeContext.AatfAddress.Add(address);

            await database.WeeeContext.SaveChangesAsync();

            return address;
        }

        private static async Task<Scheme> CreateScheme(WeeeContext context, Organisation organisation)
        {
            var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);

            context.Schemes.Add(scheme);

            await context.SaveChangesAsync();

            return scheme;
        }

        private static async Task<Aatf> CreateAatf(DatabaseWrapper database, Domain.AatfReturn.Return @return)
        {
            var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(database, @return.Organisation);

            database.WeeeContext.Aatfs.Add(aatf);

            await database.WeeeContext.SaveChangesAsync();

            return aatf;
        }

        private static async Task<Return> CreateReturn(WeeeContext context, DatabaseWrapper database, Organisation organisation)
        {
            var @return = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, database.Model.AspNetUsers.First().Id);

            context.Organisations.Add(organisation);
            context.Returns.Add(@return);

            await context.SaveChangesAsync();
            return @return;
        }

        private static async Task CreateWeeeReceivedAmounts(
            ObligatedReceivedDataAccess dataAccess,
            ReturnSchemeDataAccess returnSchemeDataAccess,
            Return @return,
            Aatf aatf,
            Scheme scheme)
        {
            var returnScheme = new ReturnScheme(scheme, @return);

            var weeeReceived = new WeeeReceived(scheme, aatf, @return);

            var weeeReceivedAmount = new List<WeeeReceivedAmount>();

            foreach (var category in Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>())
            {
                weeeReceivedAmount.Add(new WeeeReceivedAmount(weeeReceived, (int)category, (int)category, (int)category + 1));
            }

            await returnSchemeDataAccess.Submit(new List<ReturnScheme> { returnScheme });
            await dataAccess.Submit(weeeReceivedAmount);
        }

        private static async Task CreateWeeeReusedAmounts(
            ObligatedReusedDataAccess obligatedReusedDataAccess,
            AatfSiteDataAccess aatfSiteDataAccess,
            Return @return,
            Aatf aatf,
            AatfAddress address)
        {
            var weeeReused = new WeeeReused(aatf.Id, @return.Id);

            var weeeReusedAmount = new List<WeeeReusedAmount>();

            foreach (var category in Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>())
            {
                weeeReusedAmount.Add(new WeeeReusedAmount(weeeReused, (int)category, (int)category, (int)category + 1));
            }

            var weeeReusedSite = new WeeeReusedSite(weeeReused, address);

            await obligatedReusedDataAccess.Submit(weeeReusedAmount);
            await aatfSiteDataAccess.Submit(weeeReusedSite);
        }

        private static async Task CreateWeeeSentOnAmounts(
            ObligatedSentOnDataAccess dataAccess,
            Return @return,
            Aatf aatf,
            AatfAddress siteAddress,
            AatfAddress operatorAddress)
        {
            var weeeSentOn = new WeeeSentOn(operatorAddress, siteAddress, aatf, @return);

            var weeeSentOnAmount = new List<WeeeSentOnAmount>();

            foreach (var category in Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>())
            {
                weeeSentOnAmount.Add(new WeeeSentOnAmount(weeeSentOn, (int)category, (decimal?)category, (decimal?)category + 1));
            }

            await dataAccess.Submit(weeeSentOnAmount);
        }

        private static async Task CreateNonObligatedWeee(NonObligatedDataAccess dataAccess, Return @return)
        {
            var nonObligatedWee = new List<NonObligatedWeee>();

            foreach (var category in Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>())
            {
                nonObligatedWee.Add(new NonObligatedWeee(@return, (int)category, false, (decimal?)category));
            }

            foreach (var category in Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>())
            {
                nonObligatedWee.Add(new NonObligatedWeee(@return, (int)category, true, (decimal?)category));
            }

            await dataAccess.InsertNonObligatedWeee(@return.Id, nonObligatedWee);
        }

        private List<Core.AatfReturn.ReportOnQuestion> CreateReportQuestions()
        {
            var output = new List<Core.AatfReturn.ReportOnQuestion>();
            for (var i = 1; i <= 5; i++)
            {
                output.Add(fixture.Build<Core.AatfReturn.ReportOnQuestion>().With(r => r.Id, i).Create());
            }

            output[4].ParentId = 4;
            return output;
        }
    }
}