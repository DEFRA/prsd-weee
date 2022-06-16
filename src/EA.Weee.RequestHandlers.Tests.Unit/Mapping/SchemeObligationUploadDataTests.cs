namespace EA.Weee.RequestHandlers.Tests.Unit.Mapping
{
    using System;
    using System.Collections.Generic;
    using Core.Admin.Obligation;
    using Core.DataReturns;
    using Domain.Error;
    using Domain.Obligation;
    using FakeItEasy;
    using FluentAssertions;
    using Mappings;
    using Xunit;

    public class SchemeObligationUploadDataTests
    {
        private readonly SchemeObligationUploadDataMap map;

        public SchemeObligationUploadDataTests()
        {
            map = new SchemeObligationUploadDataMap();
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
        public void Map_GivenObligationUpload_SchemeObligationUploadDataShouldBeMapped()
        {
            //arrange
            var obligationUploadErrors = new List<ObligationUploadError>()
            {
                new ObligationUploadError(ObligationUploadErrorType.Scheme, "scheme 1", "scheme 1 identifier",
                    "error 1"),
                new ObligationUploadError(ObligationUploadErrorType.File, "error 2"),
                new ObligationUploadError(ObligationUploadErrorType.Data, EA.Weee.Domain.Lookup.WeeeCategory.ConsumerEquipment, "scheme 2",
                    "scheme 2 identifier", "error 3")
            };

            var obligationUpload = A.Fake<ObligationUpload>();
            A.CallTo(() => obligationUpload.ObligationUploadErrors).Returns(obligationUploadErrors);

            //act
            var result = map.Map(obligationUpload);

            //assert
            result.Count.Should().Be(3);
            result.Should().Contain(e =>
                e.Scheme == "scheme 1" && 
                e.SchemeIdentifier == "scheme 1 identifier" &&
                e.ErrorType == SchemeObligationUploadErrorType.Scheme &&
                e.Category == null &&
                e.Description == "error 1");
            result.Should().Contain(e =>
                e.Scheme == "scheme 2" && 
                e.SchemeIdentifier == "scheme 2 identifier" &&
                e.ErrorType == SchemeObligationUploadErrorType.Data &&
                e.Category == WeeeCategory.ConsumerEquipment &&
                e.Description == "error 3");
            result.Should().Contain(e =>
                e.Scheme == null &&
                e.SchemeIdentifier == null &&
                e.ErrorType == SchemeObligationUploadErrorType.File &&
                e.Category == null &&
                e.Description == "error 2");
        }
    }
}
