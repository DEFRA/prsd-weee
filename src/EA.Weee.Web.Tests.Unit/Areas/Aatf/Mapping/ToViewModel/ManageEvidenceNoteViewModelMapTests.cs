namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Mapping.ToViewModel
{
    using AutoFixture;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class ManageEvidenceNoteViewModelMapTests
    {
        private readonly ManageEvidenceNoteViewModelMap map;
        private readonly Fixture fixture;
        private readonly IMapper mapper;

        public ManageEvidenceNoteViewModelMapTests()
        {
            mapper = A.Fake<IMapper>();

            map = new ManageEvidenceNoteViewModelMap();

            fixture = new Fixture();
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNulLExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => map.Map(null));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenSource_ValidModelShouldBeReturned()
        {
            //arrange
            var source = fixture.Create<ManageEvidenceNoteTransfer>();

            //act
            var model = map.Map(source);

            //assert
            model.Should().NotBeNull();
        }

        [Fact]
        public void Map_GivenSourceWithSingleAatf_MapperShouldBeCalled()
        {
            //arrange
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var aatfData = fixture.Create<AatfData>();

            var aatfDataValid = fixture.Build<AatfData>().With(ad => ad.FacilityType, FacilityType.Aatf).With(ad => ad.ComplianceYear, SystemTime.Now.Year).Create();
            var aatfDataInvalid1 = fixture.Build<AatfData>().With(ad => ad.FacilityType, FacilityType.Aatf).With(ad => ad.ComplianceYear, SystemTime.Now.AddYears(-3).Year).Create();
            var aatfDataInvalid2 = fixture.Build<AatfData>().With(ad => ad.FacilityType, FacilityType.Ae).With(ad => ad.ComplianceYear, SystemTime.Now.Year).Create();

            var aatfDataList = new List<AatfData>();
            aatfDataList.Add(aatfDataValid);
            aatfDataList.Add(aatfDataInvalid1);
            aatfDataList.Add(aatfDataInvalid2);

            var source = new ManageEvidenceNoteTransfer(organisationId, aatfId, aatfData, aatfDataList, null, null, null, 2022, DateTime.Now);

            //act
            var model = map.Map(source);

            // assert 
            model.AatfId.Should().Be(source.AatfId);
            model.AatfName.Should().Be(source.AatfData.Name);
            model.OrganisationId.Should().Be(source.OrganisationId);
            model.AatfId.Should().Be(source.AatfId);
            model.SingleAatf.Should().BeTrue();
        }

        [Fact]
        public void Map_GivenSourceWithNullFilterModel_MapperShouldBeCalled()
        {
            //arrange
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var aatfData = fixture.Create<AatfData>();
            var aatfDataList = fixture.CreateMany<AatfData>().ToList();
           
            var source = new ManageEvidenceNoteTransfer(organisationId, aatfId, aatfData, aatfDataList, null, null, null, 2022, DateTime.Now);

            //act
            var model = map.Map(source);

            // assert 
            model.FilterViewModel.Should().BeOfType<FilterViewModel>();
            model.FilterViewModel.Should().BeEquivalentTo(new FilterViewModel());
        }

        [Fact]
        public void Map_GivenSourceWithNullRecipientWasteStatusModel_MapperShouldBeCalled()
        {
            //arrange
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var aatfData = fixture.Create<AatfData>();
            var aatfDataList = fixture.CreateMany<AatfData>().ToList();

            var source = new ManageEvidenceNoteTransfer(organisationId, aatfId, aatfData, aatfDataList, null, null, null, 2022, DateTime.Now);

            //act
            var model = map.Map(source);

            // assert 
            model.RecipientWasteStatusFilterViewModel.Should().BeOfType<RecipientWasteStatusFilterViewModel>();
            model.RecipientWasteStatusFilterViewModel.Should().BeEquivalentTo(new RecipientWasteStatusFilterViewModel());
        }

        [Fact]
        public void Map_GivenSourceWithNullSubmittedDatesModel_MapperShouldBeCalled()
        {
            //arrange
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var aatfData = fixture.Create<AatfData>();
            var aatfDataList = fixture.CreateMany<AatfData>().ToList();

            var source = new ManageEvidenceNoteTransfer(organisationId, aatfId, aatfData, aatfDataList, null, null, null, 2022, DateTime.Now);

            //act
            var model = map.Map(source);

            // assert 
            model.SubmittedDatesFilterViewModel.Should().BeOfType<SubmittedDatesFilterViewModel>();
            model.SubmittedDatesFilterViewModel.Should().BeEquivalentTo(new SubmittedDatesFilterViewModel());
        }

        [Fact]
        public void Map_GivenSourceWithValidFilterModel_MapperShouldBeCalled()
        {
            //arrange
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var aatfData = fixture.Create<AatfData>();
            var aatfDataList = fixture.CreateMany<AatfData>().ToList();
            var filterModel = fixture.Create<FilterViewModel>();

            var source = new ManageEvidenceNoteTransfer(organisationId, aatfId, aatfData, aatfDataList, filterModel, null, null, 2022, DateTime.Now);

            //act
            var model = map.Map(source);

            // assert 
            model.FilterViewModel.Should().Be(source.FilterViewModel);
        }

        [Fact]
        public void Map_GivenSourceWithValidRecipientWasteStatusModel_MapperShouldBeCalled()
        {
            //arrange
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var aatfData = fixture.Create<AatfData>();
            var aatfDataList = fixture.CreateMany<AatfData>().ToList();
            var recipientWasteStatusModel = fixture.Create<RecipientWasteStatusFilterViewModel>();

            var source = new ManageEvidenceNoteTransfer(organisationId, aatfId, aatfData, aatfDataList, null, recipientWasteStatusModel, null, 2022, DateTime.Now);

            //act
            var model = map.Map(source);

            // assert 
            model.RecipientWasteStatusFilterViewModel.Should().Be(source.RecipientWasteStatusFilterViewModel);
        }

        [Fact]
        public void Map_GivenSourceWithValidModel_MapperShouldBeCalled()
        {
            //arrange
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var aatfData = fixture.Create<AatfData>();
            var aatfDataList = fixture.CreateMany<AatfData>().ToList();
            var submittedDateModel = fixture.Create<SubmittedDatesFilterViewModel>();

            var source = new ManageEvidenceNoteTransfer(organisationId, aatfId, aatfData, aatfDataList, null, null, submittedDateModel, 2022, DateTime.Now);

            //act
            var model = map.Map(source);

            // assert 
            model.SubmittedDatesFilterViewModel.Should().Be(source.SubmittedDatesFilterViewModel);
        }
    }
}
