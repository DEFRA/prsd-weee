namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using FakeItEasy;
    using Prsd.Core.Mapper;
    using Services.Caching;
    using Web.Areas.Scheme.Mappings.ToViewModels;

    public class TransferEvidenceNotesViewModelMapTests
    {
        private readonly IWeeeCache cache;
        private readonly IMapper mapper;

        private readonly TransferEvidenceNotesViewModelMap map;

        public TransferEvidenceNotesViewModelMapTests()
        {
            cache = A.Fake<IWeeeCache>();
            mapper = A.Fake<IMapper>();

            map = new TransferEvidenceNotesViewModelMap(mapper, cache);
        }
    }
}
