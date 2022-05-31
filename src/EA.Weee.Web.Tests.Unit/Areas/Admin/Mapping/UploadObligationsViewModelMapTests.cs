namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Mapping
{
    using AutoFixture;
    using EA.Weee.Core.Admin.Obligation;
    using EA.Weee.Core.Shared;
    using EA.Weee.Web.Areas.Admin.Mappings.ToViewModel;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class UploadObligationsViewModelMapTests
    {
        private readonly UploadObligationsViewModelMap map;
        private readonly Fixture fixture;
        
        public UploadObligationsViewModelMapTests()
        {
            fixture = new Fixture();

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
            var source = new UploadObligationsViewModelMapTransfer() { CompetentAuthority = fixture.Create<CompetentAuthority>() };

            //act
            var model = map.Map(source);

            //assert
            model.Authority.Should().Be(source.CompetentAuthority);
            model.NumberOfDataErrors.Should().Be(0);
            model.DisplayDataError.Should().BeFalse();
            model.DisplayFormatError.Should().BeFalse();
            model.DisplaySelectFileError.Should().BeFalse();
            model.DisplaySuccessMessage.Should().BeFalse();
        }

        [Theory]
        [InlineData(SchemeObligationUploadErrorType.Data)]
        [InlineData(SchemeObligationUploadErrorType.Scheme)]
        public void Map_GivenSourceWithObligationUploadDataErrors_UploadObligationsViewModelShouldBeReturnedWithDisplayDataErrorAsTrue(SchemeObligationUploadErrorType errorType)
        {
            //arrange
            var schemeUploadObligationData = fixture.Build<SchemeObligationUploadData>()
                .With(s => s.ErrorData, new List<SchemeObligationUploadErrorData>() { new SchemeObligationUploadErrorData(errorType, fixture.Create<string>(), fixture.Create<string>(), fixture.Create<string>(), null) })
                .Create();
            var source = new UploadObligationsViewModelMapTransfer()
            { 
                CompetentAuthority = fixture.Create<CompetentAuthority>(),
                UploadData = schemeUploadObligationData
            };
            
            //act
            var model = map.Map(source);

            //assert
            model.Authority.Should().Be(source.CompetentAuthority);
            model.DisplayDataError.Should().BeTrue();
            model.DisplaySuccessMessage.Should().BeFalse();
        }

        [Fact]
        public void Map_GivenSourceWithObligationUploadDataErrors_UploadObligationsViewModelShouldBeReturnedWithNumberOfDataErrorsSet()
        {
            //arrange
            var schemeUploadObligationData = fixture.Build<SchemeObligationUploadData>()
                .With(s => s.ErrorData, 
                    new List<SchemeObligationUploadErrorData>()
                    {
                        new SchemeObligationUploadErrorData(SchemeObligationUploadErrorType.Data, fixture.Create<string>(), fixture.Create<string>(), fixture.Create<string>(), null),
                        new SchemeObligationUploadErrorData(SchemeObligationUploadErrorType.Data, fixture.Create<string>(), fixture.Create<string>(), fixture.Create<string>(), null)
                    })
                .Create();

            var source = new UploadObligationsViewModelMapTransfer()
            {
                CompetentAuthority = fixture.Create<CompetentAuthority>(),
                UploadData = schemeUploadObligationData
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
        public void
            Map_GivenSourceWithObligationUploadFileErrors_UploadObligationsViewModelShouldBeReturnedWithDisplayFormatErrorAsTrue()
        {
            //arrange
            var schemeUploadObligationData = fixture.Build<SchemeObligationUploadData>()
                .With(s => s.ErrorData, new List<SchemeObligationUploadErrorData>() { new SchemeObligationUploadErrorData(SchemeObligationUploadErrorType.File, fixture.Create<string>(), fixture.Create<string>(), fixture.Create<string>(), null) })
                .Create();
            var source = new UploadObligationsViewModelMapTransfer()
            {
                CompetentAuthority = fixture.Create<CompetentAuthority>(),
                UploadData = schemeUploadObligationData
            };

            //act
            var model = map.Map(source);

            //assert
            model.Authority.Should().Be(source.CompetentAuthority);
            model.DisplayFormatError.Should().BeTrue();
            model.DisplaySuccessMessage.Should().BeFalse();
        }

        [Fact]
        public void
            Map_GivenSourceWithDifferentErrorTypes_UploadObligationsViewModelShouldBeReturnedWithDisplayFormatErrorAsTrueAndDisplayFormatErrorAsFalse()
        {
            //arrange
            var schemeUploadObligationData = fixture.Build<SchemeObligationUploadData>()
                .With(s => s.ErrorData, 
                    new List<SchemeObligationUploadErrorData>()
                    {
                        new SchemeObligationUploadErrorData(SchemeObligationUploadErrorType.File, fixture.Create<string>(), fixture.Create<string>(), fixture.Create<string>(), null),
                        new SchemeObligationUploadErrorData(SchemeObligationUploadErrorType.Data, fixture.Create<string>(), fixture.Create<string>(), fixture.Create<string>(), null)
                    })
                .Create();
            var source = new UploadObligationsViewModelMapTransfer()
            {
                CompetentAuthority = fixture.Create<CompetentAuthority>(),
                UploadData = schemeUploadObligationData
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
        public void
            Map_GivenSourceWithUploadWithNoError_UploadObligationsViewModelShouldBeReturnedWithDisplaySuccessMessageAsTrue()
        {
            //arrange
            var schemeUploadObligationData = fixture.Build<SchemeObligationUploadData>()
                .With(s => s.ErrorData, 
                    new List<SchemeObligationUploadErrorData>())
                .Create();
            var source = new UploadObligationsViewModelMapTransfer()
            {
                CompetentAuthority = fixture.Create<CompetentAuthority>(),
                UploadData = schemeUploadObligationData
            };

            //act
            var model = map.Map(source);

            //assert
            model.Authority.Should().Be(source.CompetentAuthority);
            model.DisplayFormatError.Should().BeFalse();
            model.DisplayDataError.Should().BeFalse();
            model.DisplaySuccessMessage.Should().BeTrue();
        }
    }
}
