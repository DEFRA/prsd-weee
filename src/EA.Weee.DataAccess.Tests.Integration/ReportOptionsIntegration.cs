namespace EA.Weee.DataAccess.Tests.Integration
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.Tests.Core.Model;
    using FluentAssertions;
    using Xunit;

    public class ReportOptionsIntegration
    {
        [Fact]
        public async Task CanCreateReturnReportOnEntry()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new GenericDataAccess(context);

                var @return = await CreateReturn(context, database);

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

        private static async Task<Domain.AatfReturn.Return> CreateReturn(WeeeContext context, DatabaseWrapper database)
        {
            var organisation = ObligatedWeeeIntegrationCommon.CreateOrganisation();
            var @operator = ObligatedWeeeIntegrationCommon.CreateOperator(organisation);
            var @return = ObligatedWeeeIntegrationCommon.CreateReturn(@operator, database.Model.AspNetUsers.First().Id);

            context.Organisations.Add(organisation);
            context.Operators.Add(@operator);
            context.Returns.Add(@return);

            await context.SaveChangesAsync();
            return @return;
        }
    }
}
