namespace EA.Weee.RequestHandlers.Tests.DataAccess.Admin.GetProducerDetails
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Core.Admin;
    using Domain.Producer;
    using FakeItEasy;
    using Mappings;
    using Prsd.Core.Mapper;
    using RequestHandlers.Admin.GetProducerDetails;
    using Requests.Admin;
    using Security;
    using Weee.DataAccess;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class GetProducerDetailsHandlerTests
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IMapper mapper;

        public GetProducerDetailsHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            mapper = A.Fake<IMapper>();
        }

        [Theory(Skip = "Should only be run manually to check the performance of this handler with a populated database")]
        [InlineData(3000, "WEE/MM0004AA")]
        public async void PerformanceTest(int expectedUnderMilliseconds, string producerRegistrationNumber)
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                // Arrange
                var producerEeeMap = new ProducerEeeMap();

                A.CallTo(
                    () =>
                        mapper.Map<IEnumerable<ProducerEeeByQuarter>, ProducerEeeDetails>(
                            A<IEnumerable<ProducerEeeByQuarter>>._))
                    .ReturnsLazily((IEnumerable<ProducerEeeByQuarter> input) => producerEeeMap.Map(input));

                var stopwatch = Stopwatch.StartNew();
                await Handler(databaseWrapper.WeeeContext)
                    .HandleAsync(new GetProducerDetails
                    {
                        RegistrationNumber = producerRegistrationNumber
                    });
                stopwatch.Stop();

                // Assert
                Assert.InRange(stopwatch.ElapsedMilliseconds, 0, expectedUnderMilliseconds);
            }
        }

        private GetProducerDetailsHandler Handler(WeeeContext context)
        {
            return
                new GetProducerDetailsHandler(new GetProducerDetailsDataAccess(context), authorization, mapper);
        }
    }
}
