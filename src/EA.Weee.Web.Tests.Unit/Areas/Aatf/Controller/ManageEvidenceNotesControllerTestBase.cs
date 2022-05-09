﻿namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Controller
{
    using System;
    using System.Collections.Generic;
    using Api.Client;
    using AutoFixture;
    using Core.AatfEvidence;
    using FakeItEasy;
    using Prsd.Core.Mapper;
    using Services;
    using Services.Caching;
    using Web.Areas.Aatf.Controllers;
    using Web.Areas.Aatf.ViewModels;
    using Web.Requests.Base;
    using Web.ViewModels.Shared;
    using Weee.Requests.Aatf;
    using Weee.Requests.AatfEvidence;

    public class ManageEvidenceNotesControllerTestsBase
    {
        protected readonly IWeeeClient WeeeClient;
        protected readonly IMapper Mapper;
        protected readonly ManageEvidenceNotesController ManageEvidenceController;
        protected readonly BreadcrumbService Breadcrumb;
        protected readonly IWeeeCache Cache;
        protected readonly ISessionService SessionService;
        protected readonly IRequestCreator<EvidenceNoteViewModel, CreateEvidenceNoteRequest> CreateRequestCreator;
        protected readonly IRequestCreator<EvidenceNoteViewModel, EditEvidenceNoteRequest> EditRequestCreator;
        protected readonly Fixture Fixture;
        protected readonly Guid OrganisationId;
        protected readonly Guid AatfId;
        protected readonly Guid EvidenceNoteId;

        public ManageEvidenceNotesControllerTestsBase()
        {
            WeeeClient = A.Fake<IWeeeClient>();
            Breadcrumb = A.Fake<BreadcrumbService>();
            Cache = A.Fake<IWeeeCache>();
            Mapper = A.Fake<IMapper>();
            SessionService = A.Fake<ISessionService>();
            CreateRequestCreator = A.Fake<IRequestCreator<EvidenceNoteViewModel, CreateEvidenceNoteRequest>>();
            EditRequestCreator = A.Fake<IRequestCreator<EvidenceNoteViewModel, EditEvidenceNoteRequest>>();
            Fixture = new Fixture();
            OrganisationId = Guid.NewGuid();
            AatfId = Guid.NewGuid();
            EvidenceNoteId = Guid.NewGuid();
            ManageEvidenceController = new ManageEvidenceNotesController(Mapper, Breadcrumb, Cache, () => WeeeClient, CreateRequestCreator, EditRequestCreator, SessionService);
        }

        protected void AddModelError()
        {
            ManageEvidenceController.ModelState.AddModelError("error", "error");
        }

        protected CreateEvidenceNoteRequest Request(NoteStatus status = NoteStatus.Draft)
        {
            return new CreateEvidenceNoteRequest(Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                DateTime.Now,
                DateTime.Now,
                null,
                null,
                new List<TonnageValues>(),
                status,
                Guid.Empty);
        }

        protected EditEvidenceNoteRequest EditRequest(NoteStatus status = NoteStatus.Draft)
        {
            return new EditEvidenceNoteRequest(Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                DateTime.Now,
                DateTime.Now,
                null,
                null,
                new List<TonnageValues>(),
                status,
                Guid.NewGuid());
        }

        protected EditEvidenceNoteViewModel ValidModel()
        {
            var model = new EditEvidenceNoteViewModel()
            {
                EndDate = DateTime.Now,
                StartDate = DateTime.Now,
                ReceivedId = Guid.NewGuid(),
                AatfId = Guid.NewGuid(),
                OrganisationId = Guid.NewGuid()
            };
            return model;
        }
    }
}
