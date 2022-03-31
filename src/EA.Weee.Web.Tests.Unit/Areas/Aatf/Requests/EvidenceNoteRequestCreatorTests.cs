namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Requests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using Core.AatfEvidence;
    using EA.Weee.Core.Tests.Unit.SpecimenBuilders;
    using FluentAssertions;
    using Web.Areas.Aatf.Requests;
    using Web.Areas.Aatf.ViewModels;
    using Web.Extensions;
    using Weee.Requests.AatfEvidence;
    using Xunit;

    public class EvidenceNoteRequestCreatorTests
    {
        private readonly EvidenceNoteRequestCreator requestCreator;
        private readonly Fixture fixture;

        public EvidenceNoteRequestCreatorTests()
        {
            fixture = new Fixture();
            fixture.Customizations.Add(new StringDecimalGenerator());

            requestCreator = new EvidenceNoteRequestCreator();
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
        public void ViewModelToRequest_GivenNullReceivedId_InvalidOperationExceptionExpected()
        {
            //arrange
            var model = new EvidenceNoteViewModel()
            {
                ReceivedId = null
            };

            //act
            var exception = Record.Exception(() => requestCreator.ViewModelToRequest(model));

            //assert
            exception.Should().BeOfType<InvalidOperationException>();
        }

        [Fact]
        public void ViewModelToRequest_GivenSaveViewModel_CreateEvidenceNoteRequestShouldBeCreated()
        {
            //arrange
            var aatfId = Guid.NewGuid();
            var endDate = DateTime.Now;
            var startDate = DateTime.Now;
            var organisationId = Guid.NewGuid();
            var receivedId = Guid.NewGuid();
            var wasteTypeValue = fixture.Create<WasteType>();
            var protocolValue = fixture.Create<Protocol>();
            var tonnageValues = fixture.CreateMany<EvidenceCategoryValue>()
                .ToList();
            
            var model = new EvidenceNoteViewModel()
            {
                AatfId = aatfId,
                EndDate = endDate,
                StartDate = startDate,
                OrganisationId = organisationId,
                ReceivedId = receivedId,
                WasteTypeValue = wasteTypeValue,
                ProtocolValue = protocolValue,
                CategoryValues = tonnageValues,
                Action = ActionEnum.Save
            };

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
            var aatfId = Guid.NewGuid();
            var endDate = DateTime.Now;
            var startDate = DateTime.Now;
            var organisationId = Guid.NewGuid();
            var receivedId = Guid.NewGuid();
            var wasteTypeValue = fixture.Create<WasteType>();
            var protocolValue = fixture.Create<Protocol>();
            var tonnageValues = fixture.CreateMany<EvidenceCategoryValue>()
                .ToList();

            var model = new EvidenceNoteViewModel()
            {
                AatfId = aatfId,
                EndDate = endDate,
                StartDate = startDate,
                OrganisationId = organisationId,
                ReceivedId = receivedId,
                WasteTypeValue = wasteTypeValue,
                ProtocolValue = protocolValue,
                CategoryValues = tonnageValues,
                Action = ActionEnum.Submit
            };

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
            request.Status.Should().Be(NoteStatus.Submitted);
            ShouldContainTonnage(tonnageValues, request);
        }

        [Fact]
        public void ViewModelToRequest_GivenViewModelWithNullValues_CreateEvidenceNoteRequestShouldBeCreated()
        {
            //arrange
            var aatfId = Guid.NewGuid();
            var endDate = DateTime.Now;
            var startDate = DateTime.Now;
            var organisationId = Guid.NewGuid();
            var receivedId = Guid.NewGuid();
            var tonnageValues = fixture.CreateMany<EvidenceCategoryValue>()
                .ToList();

            var model = new EvidenceNoteViewModel()
            {
                AatfId = aatfId,
                EndDate = endDate,
                StartDate = startDate,
                OrganisationId = organisationId,
                ReceivedId = receivedId,
                WasteTypeValue = null,
                ProtocolValue = null,
                CategoryValues = tonnageValues
            };

            //act
            var request = requestCreator.ViewModelToRequest(model);

            //assert
            request.AatfId.Should().Be(aatfId);
            request.OrganisationId.Should().Be(organisationId);
            request.StartDate.Should().Be(startDate);
            request.EndDate.Should().Be(endDate);
            request.RecipientId.Should().Be(receivedId);
            request.WasteType.Should().BeNull();
            request.Protocol.Should().BeNull();
            ShouldContainTonnage(tonnageValues, request);
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
