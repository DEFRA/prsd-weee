namespace EA.Weee.Integration.Tests.Builders
{
    using Base;
    using EA.Weee.Domain.DataReturns;
    using EA.Weee.Domain.Producer;
    using EA.Weee.Domain.Producer.Classfication;
    using EA.Weee.Tests.Core;

    public class DirectRegistrantSubmissionHistoryDbSetup : DbTestDataBuilder<DirectProducerSubmissionHistory, DirectRegistrantSubmissionHistoryDbSetup>
    {
        protected override DirectProducerSubmissionHistory Instantiate()
        {
            instance = new DirectProducerSubmissionHistory();

            return instance;
        }

        public DirectRegistrantSubmissionHistoryDbSetup WithDirectProducerSubmission(DirectProducerSubmission submission)
        {
            ObjectInstantiator<DirectProducerSubmissionHistory>.SetProperty(o => o.DirectProducerSubmissionId, submission.Id, instance);

            return this;
        }

        public DirectRegistrantSubmissionHistoryDbSetup WithSellingTechnique(SellingTechniqueType sellingTechnique)
        {
            ObjectInstantiator<DirectProducerSubmissionHistory>.SetProperty(o => o.SellingTechniqueType, sellingTechnique.Value, instance);

            return this;
        }

        public DirectRegistrantSubmissionHistoryDbSetup WithEeeData(EeeOutputReturnVersion outputReturnVersion)
        {
            ObjectInstantiator<DirectProducerSubmissionHistory>.SetProperty(o => o.EeeOutputReturnVersion, outputReturnVersion, instance);

            return this;
        }
    }
}
