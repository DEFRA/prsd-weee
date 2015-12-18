namespace EA.Weee.RequestHandlers.Tests.Unit.DataReturns.BusinessValidation
{
    using System;
    using System.Threading.Tasks;
    using Domain.DataReturns;
    using FakeItEasy;
    using RequestHandlers.DataReturns;
    using RequestHandlers.DataReturns.BusinessValidation;
    using RequestHandlers.DataReturns.ReturnVersionBuilder;
    using Xunit;
    using Quarter = Domain.DataReturns.Quarter;
    using Scheme = Domain.Scheme.Scheme;
    public class EeeeValidatorTests
    {
        [Fact]
        public async Task Validate_WithValidData_ReturnsEmptyErrorDataList()
        {
            // Arrange
            Quarter quarter = new Quarter(2016, Domain.DataReturns.QuarterType.Q1);
            Scheme scheme = A.Fake<Scheme>();
            IDataReturnVersionBuilderDataAccess dataAccess = A.Fake<IDataReturnVersionBuilderDataAccess>();
            Func<Domain.Scheme.Scheme, Quarter, IDataReturnVersionBuilderDataAccess> dataAccessDelegate;
            dataAccessDelegate = (s, q) => dataAccess;
            EeeValidator eeeValidator = new EeeValidator(scheme, quarter, dataAccessDelegate);
            throw new NotImplementedException();
        }
    }
}
