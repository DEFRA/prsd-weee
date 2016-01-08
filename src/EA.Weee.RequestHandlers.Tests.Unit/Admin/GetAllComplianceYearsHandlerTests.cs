namespace EA.Weee.RequestHandlers.Tests.Unit.Admin
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using Core.Shared;
    using Domain.DataReturns;
    using Domain.Scheme;
    using FakeItEasy;
    using RequestHandlers.Admin;
    using RequestHandlers.Security;
    using Requests.Admin;
    using Weee.Tests.Core;
    using Xunit;

    public class GetAllComplianceYearsHandlerTests
    {
        private readonly DbContextHelper dbContextHelper = new DbContextHelper();

        [Theory]
        [InlineData(AuthorizationBuilder.UserType.External)]
        [InlineData(AuthorizationBuilder.UserType.Unauthenticated)]
        public async Task GetAllComplianceYearsHandler_NotAdminUser_ThrowsSecurityException(AuthorizationBuilder.UserType userType)
        {
            // Arrange
            Guid pcsId = new Guid("A7905BCD-8EE7-48E5-9E71-2B571F7BBC81");
            IGetAllComplianceYearsDataAccess dataAccess = A.Dummy<IGetAllComplianceYearsDataAccess>();
            IWeeeAuthorization authorization = AuthorizationBuilder.CreateFromUserType(userType);
                
            GetAllComplianceYearsHandler handler = new GetAllComplianceYearsHandler(authorization, dataAccess);

            GetAllComplianceYears request = new GetAllComplianceYears();

            // Act
            Func<Task> action = async () => await handler.HandleAsync(request);

            // Asert
            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task GetAllComplianceYearsHandler_ReturnsYearsInDescendingOrder()
        {
            // Arrange
            var memberUpload1 = A.Fake<MemberUpload>();
            A.CallTo(() => memberUpload1.ComplianceYear).Returns(2015); 

            var memberUpload2 = A.Fake<MemberUpload>();
            A.CallTo(() => memberUpload2.ComplianceYear).Returns(2017);

            var memberUpload3 = A.Fake<MemberUpload>();
            A.CallTo(() => memberUpload3.ComplianceYear).Returns(2016);

            IGetAllComplianceYearsDataAccess dataAccess = A.Fake<IGetAllComplianceYearsDataAccess>();
            A.CallTo(() => dataAccess.GetAllComplianceYears(ComplianceYearFor.MemberRegistrations))
                .Returns(new List<int>()
                {
                    memberUpload2.ComplianceYear.Value, 
                    memberUpload3.ComplianceYear.Value, 
                    memberUpload1.ComplianceYear.Value
                });

            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();

            GetAllComplianceYearsHandler handler = new GetAllComplianceYearsHandler(authorization, dataAccess);

            GetAllComplianceYears request = new GetAllComplianceYears();

            // Act
            var yearsList = await handler.HandleAsync(request);
            Assert.NotNull(yearsList);
            Assert.Equal(3, yearsList.Count);
            Assert.Collection(yearsList,
                r1 => Assert.Equal("2017", r1.ToString()),
                r2 => Assert.Equal("2016", r2.ToString()),
                r3 => Assert.Equal("2015", r3.ToString()));
        }
    }
}
