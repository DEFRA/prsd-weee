namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Requests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using AutoFixture;
    using Core.AatfEvidence;
    using FluentAssertions;
    using Web.Areas.Aatf.Requests;
    using Web.Areas.Aatf.ViewModels;
    using Web.Extensions;
    using Web.ViewModels.Shared;
    using Weee.Requests.AatfEvidence;
    using Weee.Tests.Core.SpecimenBuilders;
    using Xunit;

    public class CreateEvidenceNoteRequestCreatorTests
    {
        private readonly CreateEvidenceNoteRequestCreator requestCreator;
        private readonly Fixture fixture;

        public CreateEvidenceNoteRequestCreatorTests()
        {
            fixture = new Fixture();
            fixture.Customizations.Add(new StringDecimalReceivedAndReusedGenerator());

            requestCreator = new CreateEvidenceNoteRequestCreator();
        }

        [Fact]
        public void ViewModelToRequest_GivenNullViewModel_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => requestCreator.ViewModelToRequest(null));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void ViewModelToRequest_GivenEmptyReceivedId_ArgumentExceptionExpected()
        {
            //arrange
            var model = ValidModel();
            model.ReceivedId = Guid.Empty;

            //act
            var exception = Record.Exception(() => requestCreator.ViewModelToRequest(model));

            //assert
            exception.Should().BeOfType<TargetInvocationException>();
            exception.InnerException.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void ViewModelToRequest_GivenEmptyOrganisationId_ArgumentExceptionExpected()
        {
            //arrange
            var model = ValidModel();
            model.OrganisationId = Guid.Empty;

            //act
            var exception = Record.Exception(() => requestCreator.ViewModelToRequest(model));

            //assert
            exception.Should().BeOfType<TargetInvocationException>();
            exception.InnerException.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void ViewModelToRequest_GivenEmptyAatfId_ArgumentExceptionExpected()
        {
            //arrange
            var model = ValidModel();
            model.AatfId = Guid.Empty;

            //act
            var exception = Record.Exception(() => requestCreator.ViewModelToRequest(model));

            //assert
            exception.Should().BeOfType<TargetInvocationException>();
            exception.InnerException.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void ViewModelToRequest_GivenEmptyStartDate_ArgumentExceptionExpected()
        {
            //arrange
            var model = ValidModel();
            model.StartDate = DateTime.MinValue;

            //act
            var exception = Record.Exception(() => requestCreator.ViewModelToRequest(model));

            //assert
            exception.Should().BeOfType<TargetInvocationException>();
            exception.InnerException.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void ViewModelToRequest_GivenEmptyEndDate_ArgumentExceptionExpected()
        {
            //arrange
            var model = ValidModel();
            model.EndDate = DateTime.MinValue;

            //act
            var exception = Record.Exception(() => requestCreator.ViewModelToRequest(model));

            //assert
            exception.Should().BeOfType<TargetInvocationException>();
            exception.InnerException.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void ViewModelToRequest_GivenSaveViewModel_CreateEvidenceNoteRequestShouldBeCreated()
        {
            //arrange
            var aatfId = fixture.Create<Guid>();
            var endDate = fixture.Create<DateTime>();
            var startDate = fixture.Create<DateTime>();
            var organisationId = fixture.Create<Guid>();
            var receivedId = fixture.Create<Guid>();
            var wasteTypeValue = fixture.Create<WasteType>();
            var protocolValue = fixture.Create<Protocol>();
            var tonnageValues = fixture.CreateMany<EvidenceCategoryValue>()
                .ToList();

            var model = ValidModel();
            model.AatfId = aatfId;
            model.EndDate = endDate;
            model.StartDate = startDate;
            model.OrganisationId = organisationId;
            model.ReceivedId = receivedId;
            model.WasteTypeValue = wasteTypeValue;
            model.ProtocolValue = protocolValue;
            model.CategoryValues = tonnageValues;
            model.Action = ActionEnum.Save;

            //act
            var request = requestCreator.ViewModelToRequest(model);

            //assert
            request.AatfId.Should().Be(aatfId);
            request.OrganisationId.Should().Be(organisationId);
            request.StartDate.Should().Be(startDate);
            request.EndDate.Should().Be(endDate);
            request.RecipientId.Should().Be(receivedId);
            request.WasteType.Should().Be(wasteTypeValue);
            request.Protocol.Should().Be(protocolValue);
            request.Status.Should().Be(NoteStatus.Draft);
            ShouldContainTonnage(tonnageValues, request);
        }

        [Fact]
        public void ViewModelToRequest_GivenSubmitViewModel_CreateEvidenceNoteRequestShouldBeCreated()
        {
            //arrange
            var aatfId = fixture.Create<Guid>();
            var endDate = fixture.Create<DateTime>();
            var startDate = fixture.Create<DateTime>();
            var organisationId = fixture.Create<Guid>();
            var receivedId = fixture.Create<Guid>();
            var wasteTypeValue = fixture.Create<WasteType>();
            var protocolValue = fixture.Create<Protocol>();
            var tonnageValues = fixture.CreateMany<EvidenceCategoryValue>()
                .ToList();

            var model = ValidModel();
            model.AatfId = aatfId;
            model.EndDate = endDate;
            model.StartDate = startDate;
            model.OrganisationId = organisationId;
            model.ReceivedId = receivedId;
            model.WasteTypeValue = wasteTypeValue;
            model.ProtocolValue = protocolValue;
            model.CategoryValues = tonnageValues;
            model.Action = ActionEnum.Submit;

            //act
            var request = requestCreator.ViewModelToRequest(model);

            //assert
            request.Id.Should().Be(Guid.Empty);
            request.AatfId.Should().Be(aatfId);
            request.OrganisationId.Should().Be(organisationId);
            request.StartDate.Should().Be(startDate);
            request.EndDate.Should().Be(endDate);
            request.RecipientId.Should().Be(receivedId);
            request.WasteType.Should().Be(wasteTypeValue);
            request.Protocol.Should().Be(protocolValue);
            request.Status.Should().Be(NoteStatus.Submitted);
            ShouldContainTonnage(tonnageValues, request);
        }

        [Fact]
        public void ViewModelToRequest_GivenViewModelWithNullWasteAndProtocolValues_CreateEvidenceNoteRequestShouldBeCreated()
        {
            //arrange
            var aatfId = fixture.Create<Guid>();
            var endDate = fixture.Create<DateTime>();
            var startDate = fixture.Create<DateTime>();
            var organisationId = fixture.Create<Guid>();
            var receivedId = fixture.Create<Guid>();
            var tonnageValues = fixture.CreateMany<EvidenceCategoryValue>()
                .ToList();

            var model = ValidModel();
            model.AatfId = aatfId;
            model.EndDate = endDate;
            model.StartDate = startDate;
            model.OrganisationId = organisationId;
            model.ReceivedId = receivedId;
            model.WasteTypeValue = null;
            model.ProtocolValue = null;
            model.CategoryValues = tonnageValues;
            model.Action = ActionEnum.Save;

            //act
            var request = requestCreator.ViewModelToRequest(model);

            //assert
            request.Id.Should().Be(Guid.Empty);
            request.AatfId.Should().Be(aatfId);
            request.OrganisationId.Should().Be(organisationId);
            request.StartDate.Should().Be(startDate);
            request.EndDate.Should().Be(endDate);
            request.RecipientId.Should().Be(receivedId);
            request.WasteType.Should().Be(null);
            request.Protocol.Should().Be(null);
            request.Status.Should().Be(NoteStatus.Draft);
            ShouldContainTonnage(tonnageValues, request);
        }

        [Fact]
        public void ViewModelToRequest_GivenSaveAndViewModelStatusIsNotSet_CreateEvidenceNoteRequestShouldBeCreatedWithDraftStatus()
        {
            //arrange
            var id = fixture.Create<Guid>();
            var model = ValidModel();
            model.Id = id;
            model.Status = 0;
            model.Action = ActionEnum.Save;

            //act
            var request = requestCreator.ViewModelToRequest(model);

            //assert
            request.Status.Should().Be(NoteStatus.Draft);
        }

        [Fact]
        public void ViewModelToRequest_GivenSaveAndViewModelStatusIsSet_CreateEvidenceNoteRequestShouldBeCreatedWithCorrectStatus()
        {
            //arrange
            var id = fixture.Create<Guid>();
            var model = ValidModel();
            model.Id = id;
            model.Status = NoteStatus.Returned;
            model.Action = ActionEnum.Save;

            //act
            var request = requestCreator.ViewModelToRequest(model);

            //assert
            request.Status.Should().Be(NoteStatus.Returned);
        }

        private EditEvidenceNoteViewModel ValidModel()
        {
            var model = new EditEvidenceNoteViewModel()
            {
                ReceivedId = Guid.NewGuid(),
                Id = Guid.Empty,
                OrganisationId = Guid.NewGuid(),
                AatfId = Guid.NewGuid(),
                StartDate = DateTime.Now,
                EndDate = DateTime.Now
            };

            return model;
        }

        private void ShouldContainTonnage(List<EvidenceCategoryValue> tonnageValues, EvidenceNoteBaseRequest request)
        {
            foreach (var evidenceCategoryValue in tonnageValues)
            {
                request.TonnageValues.Should().Contain(c => c.CategoryId.Equals(evidenceCategoryValue.CategoryId)
                                                            && c.FirstTonnage.Equals(evidenceCategoryValue.Received.ToDecimal())
                                                            && c.SecondTonnage.Equals(evidenceCategoryValue.Reused
                                                                .ToDecimal()));
            }
        }
    }
}
