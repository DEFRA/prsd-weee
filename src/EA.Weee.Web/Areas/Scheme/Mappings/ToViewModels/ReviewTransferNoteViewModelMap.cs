﻿namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using EA.Weee.Web.ViewModels.Shared;
    using Prsd.Core;
    using Prsd.Core.Mapper;
    using ViewModels;

    public class ReviewTransferNoteViewModelMap : IMap<ViewTransferNoteViewModelMapTransfer, ReviewTransferNoteViewModel>
    {
        private readonly IMapper mapper;

        public ReviewTransferNoteViewModelMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public ReviewTransferNoteViewModel Map(ViewTransferNoteViewModelMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var model = new ReviewTransferNoteViewModel()
            {
                ViewTransferNoteViewModel = mapper.Map<ViewTransferNoteViewModel>(source),
                OrganisationId = source.OrganisationId,
                RedirectTabName = source.RedirectTab,
                QueryString = source.QueryString
            };

            return model;
        }
    }
}