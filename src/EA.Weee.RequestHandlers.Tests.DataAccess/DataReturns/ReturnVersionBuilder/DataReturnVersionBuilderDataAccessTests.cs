namespace EA.Weee.RequestHandlers.Tests.DataAccess.DataReturns.ReturnVersionBuilder
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.DataReturns;
    using RequestHandlers.DataReturns.ReturnVersionBuilder;
    using Weee.DataAccess;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class DataReturnVersionBuilderDataAccessTests
    {
        // TODO: Write data access tests for data return method

        [Fact]
        public async Task GetRegisteredProducer_ReturnsProducerForSpecifiedSchemeOnly()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(database.Model);

                var scheme1 = helper.CreateScheme();
                helper.GerOrCreateRegisteredProducer(scheme1, 2016, "AAAA");

                var scheme2 = helper.CreateScheme();
                helper.GerOrCreateRegisteredProducer(scheme2, 2016, "AAAA");

                database.Model.SaveChanges();

                var dataAccess = new DataReturnVersionBuilderDataAccess(GetScheme(database.WeeeContext, scheme1.Id), new Quarter(2016, QuarterType.Q1), database.WeeeContext);

                // Act
                var result = await dataAccess.GetRegisteredProducer("AAAA");

                // Assert
                Assert.Equal("AAAA", result.ProducerRegistrationNumber);
                Assert.Equal(scheme1.Id, result.Scheme.Id);
            }
        }

        [Fact]
        public async Task GetRegisteredProducer_ReturnsProducerForSpecifiedComplianceYearOnly()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(database.Model);

                var scheme = helper.CreateScheme();
                helper.GerOrCreateRegisteredProducer(scheme, 2016, "AAAA");
                var producer = helper.GerOrCreateRegisteredProducer(scheme, 2017, "AAAA");

                database.Model.SaveChanges();

                var dataAccess = new DataReturnVersionBuilderDataAccess(GetScheme(database.WeeeContext, scheme.Id), new Quarter(2017, QuarterType.Q1), database.WeeeContext);

                // Act
                var result = await dataAccess.GetRegisteredProducer("AAAA");

                // Assert
                Assert.Equal("AAAA", result.ProducerRegistrationNumber);
                Assert.Equal(producer.Id, result.Id);
            }
        }

        [Fact]
        public async Task GetRegisteredProducer_ReturnsProducerForMatchingRegistrationNumberOnly()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(database.Model);

                var scheme = helper.CreateScheme();
                helper.GerOrCreateRegisteredProducer(scheme, 2016, "AAAA");
                var producer = helper.GerOrCreateRegisteredProducer(scheme, 2016, "BBBB");

                database.Model.SaveChanges();

                var dataAccess = new DataReturnVersionBuilderDataAccess(GetScheme(database.WeeeContext, scheme.Id), new Quarter(2016, QuarterType.Q1), database.WeeeContext);

                // Act
                var result = await dataAccess.GetRegisteredProducer("BBBB");

                // Assert
                Assert.Equal("BBBB", result.ProducerRegistrationNumber);
                Assert.Equal(producer.Id, result.Id);
            }
        }

        private static Domain.Scheme.Scheme GetScheme(WeeeContext context, Guid schemeId)
        {
            return context.Schemes.Single(s => s.Id == schemeId);
        }
    }
}
