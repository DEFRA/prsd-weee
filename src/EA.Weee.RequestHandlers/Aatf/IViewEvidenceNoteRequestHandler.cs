namespace EA.Weee.RequestHandlers.Aatf
{
    using EA.Prsd.Core.Mediator;
    using System;
    using System.Threading.Tasks;

    public interface IViewEvidenceNoteRequestHandler<Guid id, ViewEvidenceNoteViewModel> : IRequestHandler<Guid, ViewEvidenceNoteViewModel>
    {
        Task<ViewEvidenceNoteViewModel> HandleAsync(Guid noteId);
    }
}