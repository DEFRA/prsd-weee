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
    using Web.Areas.Aatf.Helpers;
    using Xunit;

    public class ManageEvidenceNotesViewModelMapTests
    {
        private readonly ManageEvidenceNoteViewModelMap map;
        private readonly Fixture fixture;
        private readonly Guid organisationId;
        private readonly Guid aatfId;
        private readonly AatfData aatfData;
        private readonly List<AatfData> aatfDataList;
        private readonly IAatfEvidenceHelper aatfEvidenceHelper;

        public ManageEvidenceNotesViewModelMapTests()
        {
            aatfEvidenceHelper = A.Fake<IAatfEvidenceHelper>();

            map = new ManageEvidenceNoteViewModelMap(aatfEvidenceHelper);
            fixture = new Fixture();
            organisationId = Guid.NewGuid();
            aatfId = Guid.NewGuid();
            aatfData = fixture.Create<AatfData>();
            aatfDataList = fixture.CreateMany<AatfData>().ToList();
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
            var aatfDataValid = fixture.Build<AatfData>().With(ad => ad.FacilityType, FacilityType.Aatf).With(ad => ad.ComplianceYear, SystemTime.Now.Year).Create();
            var aatfDataInvalid1 = fixture.Build<AatfData>().With(ad => ad.FacilityType, FacilityType.Aatf).With(ad => ad.ComplianceYear, SystemTime.Now.AddYears(-3).Year).Create();
            var aatfDataInvalid2 = fixture.Build<AatfData>().With(ad => ad.FacilityType, FacilityType.Ae).With(ad => ad.ComplianceYear, SystemTime.Now.Year).Create();

            var aatfDataList = new List<AatfData>();
            aatfDataList.Add(aatfDataValid);
            aatfDataList.Add(aatfDataInvalid1);
            aatfDataList.Add(aatfDataInvalid2);

            var source = new ManageEvidenceNoteTransfer(organisationId, aatfId, aatfData, aatfDataList, null, null, null, SystemTime.UtcNow.Year, SystemTime.UtcNow);

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
            var source = new ManageEvidenceNoteTransfer(organisationId, aatfId, aatfData, aatfDataList, null, null, null, SystemTime.UtcNow.Year, SystemTime.UtcNow);

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
            var source = new ManageEvidenceNoteTransfer(organisationId, aatfId, aatfData, aatfDataList, null, null, null, SystemTime.UtcNow.Year, SystemTime.UtcNow);

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
            var source = new ManageEvidenceNoteTransfer(organisationId, aatfId, aatfData, aatfDataList, null, null, null, SystemTime.UtcNow.Year, SystemTime.UtcNow);

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
            var aatfDataList = fixture.CreateMany<AatfData>().ToList();
            var filterModel = fixture.Create<FilterViewModel>();
            var source = new ManageEvidenceNoteTransfer(organisationId, aatfId, aatfData, aatfDataList, filterModel, null, null, SystemTime.UtcNow.Year, SystemTime.UtcNow);

            //act
            var model = map.Map(source);

            // assert 
            model.FilterViewModel.Should().Be(source.FilterViewModel);
        }

        [Fact]
        public void Map_GivenSourceWithValidRecipientWasteStatusModel_MapperShouldBeCalled()
        {
            //arrange
            var aatfDataList = fixture.CreateMany<AatfData>().ToList();
            var recipientWasteStatusModel = fixture.Create<RecipientWasteStatusFilterViewModel>();
            var source = new ManageEvidenceNoteTransfer(organisationId, aatfId, aatfData, aatfDataList, null, recipientWasteStatusModel, null, SystemTime.UtcNow.Year, SystemTime.UtcNow);

            //act
            var model = map.Map(source);

            // assert 
            model.RecipientWasteStatusFilterViewModel.Should().Be(source.RecipientWasteStatusFilterViewModel);
        }

        [Fact]
        public void Map_GivenSourceWithValidModel_MapperShouldBeCalled()
        {
            //arrange
            var aatfDataList = fixture.CreateMany<AatfData>().ToList();
            var submittedDateModel = fixture.Create<SubmittedDatesFilterViewModel>();
            var source = new ManageEvidenceNoteTransfer(organisationId, aatfId, aatfData, aatfDataList, null, null, submittedDateModel, SystemTime.UtcNow.Year, SystemTime.UtcNow);

            //act
            var model = map.Map(source);

            // assert 
            model.SubmittedDatesFilterViewModel.Should().Be(source.SubmittedDatesFilterViewModel);
        }

        [Fact]
        public void Map_GivenSourceToMapper_ComplianceYearShouldBeMapped()
        {
            //arrange
            var expectedComplianceYear = fixture.Create<int>();
            var aatfDataList = new List<AatfData>
            {
                fixture.Build<AatfData>().With(ad => ad.FacilityType, FacilityType.Aatf).With(ad => ad.ComplianceYear, expectedComplianceYear).Create()
            };
            var source = new ManageEvidenceNoteTransfer(organisationId, aatfId, aatfData, aatfDataList, null, null, null, expectedComplianceYear, SystemTime.UtcNow);

            //act
            var model = map.Map(source);

            // assert 
            model.SelectedComplianceYear.Should().Be(source.ComplianceYear);
        }

        [Fact]
        public void Map_GivenSourceWithCurrentDate_ComplianceYearListShouldBeMapped()
        {
            //arrange
            var currentDate = new DateTime(2019, 1, 1);
            var source = new ManageEvidenceNoteTransfer(organisationId, aatfId, aatfData, fixture.CreateMany<AatfData>().ToList(), null, null, null, fixture.Create<int>(), currentDate);

            //act
            var model = map.Map(source);

            // assert 
            model.ComplianceYearList.Should().BeEquivalentTo(new List<int>() { 2019, 2018, 2017 });
        }
    }
}
