namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.GetDataReturnSubmissionChanges
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.DataReturns;
    using FakeItEasy;
    using RequestHandlers.Admin.GetDataReturnSubmissionChanges;
    using Weee.Tests.Core;
    using Xunit;

    public class GetDataReturnSubmissionEeeChangesCsvDataAccessTests
    {
        [Fact]
        public async Task GetChanges_WithSameValuesForCurrentAndPreviousArguments_ThrowsArgumentException()
        {
            var dataAccess =
                new GetDataReturnSubmissionEeeChangesCsvDataAccess(A.Dummy<WeeeContext>());

            var id = Guid.NewGuid();

            await Assert.ThrowsAsync<ArgumentException>(() => dataAccess.GetChanges(id, id));
        }
    }
}
