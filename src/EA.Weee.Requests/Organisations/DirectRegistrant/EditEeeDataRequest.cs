namespace EA.Weee.Requests.Organisations.DirectRegistrant
{
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Core.DirectRegistrant;
    using System;
    using System.Collections.Generic;

    public class EditEeeDataRequest : IRequest<bool>
    {
        public Guid DirectRegistrantId { get; private set; }

        public IList<Eee> TonnageData { get; private set; }

        public SellingTechniqueType SellingTechniqueType { get; private set; }

        public EditEeeDataRequest(Guid directRegistrantId, List<Eee> tonnageData, SellingTechniqueType sellingTechniqueType)
        {
            Condition.Requires(directRegistrantId).IsNotEqualTo(Guid.Empty);
            Condition.Requires(tonnageData).IsNotNull();
            Condition.Requires(tonnageData).IsNotEmpty();

            DirectRegistrantId = directRegistrantId;
            SellingTechniqueType = sellingTechniqueType;
            TonnageData = tonnageData;
        }
    }
}
