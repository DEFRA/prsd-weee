﻿namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Mapping
{
    using System;
    using System.Web;
    using AutoFixture;
    using Core.Shared;
    using FakeItEasy;
    using FluentAssertions;
    using Services;
    using Web.Areas.Admin.Mappings.FromViewModel;
    using Web.Areas.Admin.ViewModels.Obligations;
    using Xunit;

    public class SubmitSchemeObligationMapTests
    {
        private readonly IFileConverterService fileConverterService;
        private readonly SubmitSchemeObligationMap map;
        private readonly Fixture fixture;

        public SubmitSchemeObligationMapTests()
        {
            fileConverterService = A.Fake<IFileConverterService>();

            fixture = new Fixture();

            map = new SubmitSchemeObligationMap(fileConverterService);
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
        public void Map_GivenModelSource_FileConverterServiceShouldBeCalled()
        {
            //arrange
            var model = new UploadObligationsViewModel()
            {
                File = A.Fake<HttpPostedFileBase>()
            };

            //act
            map.Map(model);

            //assert
            A.CallTo(() => fileConverterService.ConvertCsv(model.File)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Map_GivenModelSource_SubmitSchemeObligationMapShouldBeReturned()
        {
            //arrange
            var model = new UploadObligationsViewModel()
            {
                File = A.Fake<HttpPostedFileBase>(),
                Authority = fixture.Create<CompetentAuthority>()
            };
            var fileData = fixture.Create<byte[]>();

            A.CallTo(() => fileConverterService.ConvertCsv(A<HttpPostedFileBase>._)).Returns(fileData);

            //act
            var result = map.Map(model);

            //assert
            result.Authority.Should().Be(model.Authority);
            result.FileInfo.Data.Should().BeEquivalentTo(fileData);
            result.FileInfo.FileName.Should().Be(model.File.FileName);
        }
    }
}
