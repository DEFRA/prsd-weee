namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Mapping
{
    using AutoFixture;
    using EA.Weee.Core.Admin.Obligation;
    using EA.Weee.Core.Shared;
    using EA.Weee.Web.Areas.Admin.Mappings.ToViewModel;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Weee.Tests.Core;
    using Xunit;

    public class UploadObligationsViewModelMapTests : SimpleUnitTestBase
    {
        private readonly UploadObligationsViewModelMap map;
        
        public UploadObligationsViewModelMapTests()
        {
            map = new UploadObligationsViewModelMap();
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => map.Map(null));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenSourceWithAuthorityAndNoUploadData_UploadObligationsViewModelShouldBeReturned()
        {
            //arrange
            var source = new UploadObligationsViewModelMapTransfer() { CompetentAuthority = TestFixture.Create<CompetentAuthority>() };

            //act
            var model = map.Map(source);

            //assert
            model.Authority.Should().Be(source.CompetentAuthority);
            model.NumberOfDataErrors.Should().Be(0);
            model.DisplayDataError.Should().BeFalse();
            model.DisplayFormatError.Should().BeFalse();
            model.DisplaySelectFileError.Should().BeFalse();
            model.DisplaySuccessMessage.Should().BeFalse();
            model.UploadedMessage.Should().BeNullOrWhiteSpace();
        }

        [Fact]
        public void Map_GivenSourceWithComplianceYearsList_UploadObligationsViewModelShouldBeReturnedWithComplianceYearList()
        {
            //arrange
            var source = new UploadObligationsViewModelMapTransfer() { ComplianceYears = TestFixture.CreateMany<int>().ToList() };

            //act
            var model = map.Map(source);

            //assert
            model.ComplianceYearList.Should().BeEquivalentTo(source.ComplianceYears);
        }

        [Fact]
        public void Map_GivenSourceWithSelectedComplianceYear_UploadObligationsViewModelSelectedComplianceYearShouldBeSet()
        {
            //arrange
            var source = new UploadObligationsViewModelMapTransfer() { SelectedComplianceYear = TestFixture.Create<int>() };

            //act
            var model = map.Map(source);

            //assert
            model.SelectedComplianceYear.Should().Be(source.SelectedComplianceYear);
        }

        [Theory]
        [InlineData(SchemeObligationUploadErrorType.Data)]
        [InlineData(SchemeObligationUploadErrorType.Scheme)]
        public void Map_GivenSourceWithObligationUploadDataErrors_UploadObligationsViewModelShouldBeReturnedWithDisplayDataErrorAsTrue(SchemeObligationUploadErrorType errorType)
        {
            //arrange
            var schemeUploadObligationData = new List<SchemeObligationUploadErrorData>() { new SchemeObligationUploadErrorData(errorType, TestFixture.Create<string>(), TestFixture.Create<string>(), TestFixture.Create<string>(), null) };

            var source = new UploadObligationsViewModelMapTransfer()
            { 
                CompetentAuthority = TestFixture.Create<CompetentAuthority>(),
                ErrorData = schemeUploadObligationData,
                DisplayNotification = true
            };
            
            //act
            var model = map.Map(source);

            //assert
            model.Authority.Should().Be(source.CompetentAuthority);
            model.DisplayDataError.Should().BeTrue();
            model.DisplaySuccessMessage.Should().BeFalse();
        }

        [Theory]
        [InlineData(SchemeObligationUploadErrorType.Data)]
        [InlineData(SchemeObligationUploadErrorType.Scheme)]
        public void Map_GivenSourceWithObligationUploadDataErrorsAndDisplayNotificationIsFalse_UploadObligationsViewModelShouldBeReturnedWithDisplayDataErrorAsFalse(SchemeObligationUploadErrorType errorType)
        {
            //arrange
            var schemeUploadObligationData = new List<SchemeObligationUploadErrorData>() { new SchemeObligationUploadErrorData(errorType, TestFixture.Create<string>(), TestFixture.Create<string>(), TestFixture.Create<string>(), null) };

            var source = new UploadObligationsViewModelMapTransfer()
            {
                CompetentAuthority = TestFixture.Create<CompetentAuthority>(),
                ErrorData = schemeUploadObligationData,
                DisplayNotification = false
            };

            //act
            var model = map.Map(source);

            //assert
            model.Authority.Should().Be(source.CompetentAuthority);
            model.DisplayDataError.Should().BeFalse();
            model.DisplaySuccessMessage.Should().BeFalse();
        }

        [Fact]
        public void Map_GivenSourceWithObligationUploadDataErrors_UploadObligationsViewModelShouldBeReturnedWithNumberOfDataErrorsSet()
        {
            //arrange
            var schemeUploadObligationData = new List<SchemeObligationUploadErrorData>()
            {
                new SchemeObligationUploadErrorData(SchemeObligationUploadErrorType.Data, TestFixture.Create<string>(),
                    TestFixture.Create<string>(), TestFixture.Create<string>(), null),
                new SchemeObligationUploadErrorData(SchemeObligationUploadErrorType.Data, TestFixture.Create<string>(),
                    TestFixture.Create<string>(), TestFixture.Create<string>(), null)
            };

            var source = new UploadObligationsViewModelMapTransfer()
            {
                CompetentAuthority = TestFixture.Create<CompetentAuthority>(),
                ErrorData = schemeUploadObligationData,
                DisplayNotification = true
            };

            //act
            var model = map.Map(source);

            //assert
            model.Authority.Should().Be(source.CompetentAuthority);
            model.DisplayDataError.Should().BeTrue();
            model.NumberOfDataErrors.Should().Be(2);
            model.DisplaySuccessMessage.Should().BeFalse();
        }

        [Fact]
        public void Map_GivenSourceWithObligationUploadDataErrorsAndDisplayNotificationIsFalse_UploadObligationsViewModelShouldBeReturnedWithNumberOfDataErrorsSet()
        {
            //arrange
            var schemeUploadObligationData = new List<SchemeObligationUploadErrorData>()
            {
                new SchemeObligationUploadErrorData(SchemeObligationUploadErrorType.Data, TestFixture.Create<string>(),
                    TestFixture.Create<string>(), TestFixture.Create<string>(), null),
                new SchemeObligationUploadErrorData(SchemeObligationUploadErrorType.Data, TestFixture.Create<string>(),
                    TestFixture.Create<string>(), TestFixture.Create<string>(), null)
            };

            var source = new UploadObligationsViewModelMapTransfer()
            {
                CompetentAuthority = TestFixture.Create<CompetentAuthority>(),
                ErrorData = schemeUploadObligationData,
                DisplayNotification = false
            };

            //act
            var model = map.Map(source);

            //assert
            model.Authority.Should().Be(source.CompetentAuthority);
            model.DisplayDataError.Should().BeFalse();
            model.NumberOfDataErrors.Should().Be(0);
            model.DisplaySuccessMessage.Should().BeFalse();
        }

        [Fact]
        public void Map_GivenSourceWithObligationUploadFileErrors_UploadObligationsViewModelShouldBeReturnedWithDisplayFormatErrorAsTrue()
        {
            //arrange
            var schemeUploadObligationData = new List<SchemeObligationUploadErrorData>()
            {
                new SchemeObligationUploadErrorData(SchemeObligationUploadErrorType.File, TestFixture.Create<string>(),
                    TestFixture.Create<string>(), TestFixture.Create<string>(), null)
            };

            var source = new UploadObligationsViewModelMapTransfer()
            {
                CompetentAuthority = TestFixture.Create<CompetentAuthority>(),
                ErrorData = schemeUploadObligationData,
                DisplayNotification = true
            };

            //act
            var model = map.Map(source);

            //assert
            model.Authority.Should().Be(source.CompetentAuthority);
            model.DisplayFormatError.Should().BeTrue();
            model.DisplaySuccessMessage.Should().BeFalse();
        }

        [Fact]
        public void Map_GivenSourceWithObligationUploadFileErrorsAndDisplayNotificationIsFalse_UploadObligationsViewModelShouldBeReturnedWithDisplayFormatErrorAsFalse()
        {
            //arrange
            var schemeUploadObligationData = new List<SchemeObligationUploadErrorData>()
            {
                new SchemeObligationUploadErrorData(SchemeObligationUploadErrorType.File, TestFixture.Create<string>(),
                    TestFixture.Create<string>(), TestFixture.Create<string>(), null)
            };

            var source = new UploadObligationsViewModelMapTransfer()
            {
                CompetentAuthority = TestFixture.Create<CompetentAuthority>(),
                ErrorData = schemeUploadObligationData,
                DisplayNotification = false
            };

            //act
            var model = map.Map(source);

            //assert
            model.Authority.Should().Be(source.CompetentAuthority);
            model.DisplayFormatError.Should().BeFalse();
            model.DisplaySuccessMessage.Should().BeFalse();
        }

        [Fact]
        public void Map_GivenSourceWithDifferentErrorTypes_UploadObligationsViewModelShouldBeReturnedWithDisplayFormatErrorAsTrueAndDisplayFormatErrorAsFalse()
        {
            //arrange
            var schemeUploadObligationData = new List<SchemeObligationUploadErrorData>()
            {
                new SchemeObligationUploadErrorData(SchemeObligationUploadErrorType.File, TestFixture.Create<string>(),
                    TestFixture.Create<string>(), TestFixture.Create<string>(), null),
                new SchemeObligationUploadErrorData(SchemeObligationUploadErrorType.Data, TestFixture.Create<string>(),
                    TestFixture.Create<string>(), TestFixture.Create<string>(), null)
            };

            var source = new UploadObligationsViewModelMapTransfer()
            {
                CompetentAuthority = TestFixture.Create<CompetentAuthority>(),
                ErrorData = schemeUploadObligationData,
                DisplayNotification = true
            };

            //act
            var model = map.Map(source);

            //assert
            model.Authority.Should().Be(source.CompetentAuthority);
            model.DisplayFormatError.Should().BeTrue();
            model.DisplayDataError.Should().BeFalse();
            model.DisplaySuccessMessage.Should().BeFalse();
        }

        [Fact]
        public void Map_GivenSourceWithDifferentErrorTypesAndDisplayNotificationIsFalse_UploadObligationsViewModelShouldBeReturnedWithDisplayFormatErrorAsFalseAndDisplayFormatErrorAsFalse()
        {
            //arrange
            var schemeUploadObligationData = new List<SchemeObligationUploadErrorData>()
            {
                new SchemeObligationUploadErrorData(SchemeObligationUploadErrorType.File, TestFixture.Create<string>(),
                    TestFixture.Create<string>(), TestFixture.Create<string>(), null),
                new SchemeObligationUploadErrorData(SchemeObligationUploadErrorType.Data, TestFixture.Create<string>(),
                    TestFixture.Create<string>(), TestFixture.Create<string>(), null)
            };

            var source = new UploadObligationsViewModelMapTransfer()
            {
                CompetentAuthority = TestFixture.Create<CompetentAuthority>(),
                ErrorData = schemeUploadObligationData,
                DisplayNotification = false
            };

            //act
            var model = map.Map(source);

            //assert
            model.Authority.Should().Be(source.CompetentAuthority);
            model.DisplayFormatError.Should().BeFalse();
            model.DisplayDataError.Should().BeFalse();
            model.DisplaySuccessMessage.Should().BeFalse();
        }

        [Fact]
        public void Map_GivenSourceWithUploadWithNoError_UploadObligationsViewModelShouldBeReturnedWithDisplaySuccessMessageAsTrue()
        {
            //arrange
            var schemeUploadObligationData = new List<SchemeObligationUploadErrorData>();
                
            var source = new UploadObligationsViewModelMapTransfer()
            {
                CompetentAuthority = TestFixture.Create<CompetentAuthority>(),
                ErrorData = schemeUploadObligationData,
                DisplayNotification = true
            };

            //act
            var model = map.Map(source);

            //assert
            model.Authority.Should().Be(source.CompetentAuthority);
            model.DisplayFormatError.Should().BeFalse();
            model.DisplayDataError.Should().BeFalse();
            model.DisplaySuccessMessage.Should().BeTrue();
        }

        [Fact]
        public void Map_GivenSourceWithUploadWithNoErrorAndDisplayNotificationIsFalse_UploadObligationsViewModelShouldBeReturnedWithDisplaySuccessMessageAsFalse()
        {
            //arrange
            var schemeUploadObligationData = new List<SchemeObligationUploadErrorData>();

            var source = new UploadObligationsViewModelMapTransfer()
            {
                CompetentAuthority = TestFixture.Create<CompetentAuthority>(),
                ErrorData = schemeUploadObligationData,
                DisplayNotification = false
            };

            //act
            var model = map.Map(source);

            //assert
            model.Authority.Should().Be(source.CompetentAuthority);
            model.DisplayFormatError.Should().BeFalse();
            model.DisplayDataError.Should().BeFalse();
            model.DisplaySuccessMessage.Should().BeFalse();
        }

        [Fact]
        public void Map_GivenSourceWithUploadWithNoError_UploadObligationsViewModelShouldBeReturnedWithUploadedMessage()
        {
            //arrange
            var source = new UploadObligationsViewModelMapTransfer()
            {
                SelectedComplianceYear = TestFixture.Create<int>(),
                DisplayNotification = true
            };

            //act
            var model = map.Map(source);

            //assert
            model.UploadedMessage.Should()
                .Be($"You have successfully uploaded the obligations for the compliance year {source.SelectedComplianceYear}");
        }

        [Fact]
        public void Map_GivenSourceWithUploadWithNoErrorAndDisplayNotificationIsFalse_UploadObligationsViewModelShouldBeReturnedWithEmptyUploadedMessage()
        {
            //arrange
            var source = new UploadObligationsViewModelMapTransfer()
            {
                SelectedComplianceYear = TestFixture.Create<int>(),
                DisplayNotification = false
            };

            //act
            var model = map.Map(source);

            //assert
            model.UploadedMessage.Should().BeNullOrWhiteSpace();
        }

        [Fact]
        public void Map_GivenSourceWithSchemeObligations_UploadObligationsViewModelShouldBeReturnedWithSchemeObligations()
        {
            //arrange
            var schemeObligationData = new List<SchemeObligationData>()
            {
                new SchemeObligationData(TestFixture.Create<string>(), TestFixture.Create<DateTime?>(), TestFixture.CreateMany<SchemeObligationAmountData>().ToList()),
                new SchemeObligationData(TestFixture.Create<string>(), TestFixture.Create<DateTime?>(), TestFixture.CreateMany<SchemeObligationAmountData>().ToList())
            };

            var source = new UploadObligationsViewModelMapTransfer()
            {
                CompetentAuthority = TestFixture.Create<CompetentAuthority>(),
                ObligationData = schemeObligationData
            };

            //act
            var model = map.Map(source);

            //assert
            model.SchemeObligations.Should().Contain(s =>
                s.SchemeName == schemeObligationData.ElementAt(0).SchemeName &&
                s.UpdateDate == schemeObligationData.ElementAt(0).UpdatedDate.ToString());
            model.SchemeObligations.Should().Contain(s =>
                s.SchemeName.Equals(schemeObligationData.ElementAt(1).SchemeName) &&
                s.UpdateDate == schemeObligationData.ElementAt(1).UpdatedDate.ToString());
        }

        [Fact]
        public void Map_GivenSourceWithNoSchemeObligations_UploadObligationsViewModelShouldBeReturnedWithEmptySchemeObligations()
        {
            //arrange
            var schemeObligationData = new List<SchemeObligationData>();

            var source = new UploadObligationsViewModelMapTransfer()
            {
                CompetentAuthority = TestFixture.Create<CompetentAuthority>(),
                ObligationData = schemeObligationData
            };

            //act
            var model = map.Map(source);

            //assert
            model.SchemeObligations.Should().BeEmpty();
        }

        [Fact]
        public void Map_GivenSourceWithSchemeObligations_UploadObligationsShouldBeOrderedBySchemeName()
        {
            //arrange
            var schemeObligationData = new List<SchemeObligationData>()
            {
                new SchemeObligationData("Z", TestFixture.Create<DateTime?>(), TestFixture.CreateMany<SchemeObligationAmountData>().ToList()),
                new SchemeObligationData("R", TestFixture.Create<DateTime?>(), TestFixture.CreateMany<SchemeObligationAmountData>().ToList()),
                new SchemeObligationData("A", TestFixture.Create<DateTime?>(), TestFixture.CreateMany<SchemeObligationAmountData>().ToList()),
                new SchemeObligationData("C", TestFixture.Create<DateTime?>(), TestFixture.CreateMany<SchemeObligationAmountData>().ToList()),
                new SchemeObligationData("B", TestFixture.Create<DateTime?>(), TestFixture.CreateMany<SchemeObligationAmountData>().ToList())
            };

            var source = new UploadObligationsViewModelMapTransfer()
            {
                CompetentAuthority = TestFixture.Create<CompetentAuthority>(),
                ObligationData = schemeObligationData
            };

            //act
            var model = map.Map(source);

            //assert
            model.SchemeObligations.Should().BeInAscendingOrder(s => s.SchemeName);
        }

        [Fact]
        public void Map_GivenSourceWithSchemeObligationsWithNoUpdatedDate_ShouldFormatUpdatedDateCorrectly()
        {
            //arrange
            var schemeObligationData = new List<SchemeObligationData>()
            {
                new SchemeObligationData(TestFixture.Create<string>(), null, TestFixture.CreateMany<SchemeObligationAmountData>().ToList()),
            };

            var source = new UploadObligationsViewModelMapTransfer()
            {
                CompetentAuthority = TestFixture.Create<CompetentAuthority>(),
                ObligationData = schemeObligationData
            };

            //act
            var model = map.Map(source);

            //assert
            model.SchemeObligations.Should().Contain(s =>
                s.SchemeName.Equals(schemeObligationData.ElementAt(0).SchemeName) &&
                s.UpdateDate.Equals("-"));
        }
    }
}
