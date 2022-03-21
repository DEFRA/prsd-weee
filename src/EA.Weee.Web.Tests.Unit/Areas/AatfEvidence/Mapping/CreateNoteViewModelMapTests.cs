namespace EA.Weee.Web.Tests.Unit.Areas.AatfEvidence.Mapping
{
    using System;
    using System.Linq;
    using AutoFixture;
    using Core.AatfEvidence;
    using Core.Scheme;
    using FluentAssertions;
    using Prsd.Core.Domain;
    using Web.Areas.AatfEvidence.Mappings.ToViewModel;
    using Xunit;

    public class CreateNoteViewModelMapTests
    {
        private readonly CreateNoteViewModelMap map;
        private readonly Fixture fixture;
        public CreateNoteViewModelMapTests()
        {
            map = new CreateNoteViewModelMap();

            fixture = new Fixture();
        }

        [Fact]
        public void Map_GiveSchemesIsNull_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => new CreateNoteMapTransfer(null));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenTransfer_CreateNoteViewModelShouldBeReturned()
        {
            //arrange
            var schemes = fixture.CreateMany<SchemeData>().ToList();
            var transfer = new CreateNoteMapTransfer(schemes);

            //act
            var result = map.Map(transfer);

            //assert
            result.Should().NotBeNull();
            result.SchemeList.Should().BeEquivalentTo(schemes);
            result.ProtocolList.Should().NotBeNullOrEmpty();
            result.ProtocolList.Should().BeEquivalentTo(Enumeration.GetAll<Protocol>());
            result.WasteTypeList.Should().NotBeNullOrEmpty();
            result.WasteTypeList.Should().BeEquivalentTo(Enumeration.GetAll<WasteType>());
        }
    }
}
