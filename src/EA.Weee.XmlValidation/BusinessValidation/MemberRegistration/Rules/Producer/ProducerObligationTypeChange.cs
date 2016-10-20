namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.Rules.Producer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Core.Shared;
    using QuerySets;
    using Xml.MemberRegistration;
    using ObligationType = Domain.Obligation.ObligationType;

    public class ProducerObligationTypeChange : IProducerObligationTypeChange
    {
        private readonly Guid organisationId;
        private readonly string complianceYear;
        private readonly ISchemeEeeDataQuerySet schemeEeeDataQuerySet;
        private readonly IProducerQuerySet producerQuerySet;

        public ProducerObligationTypeChange(
            Guid organisationId,
            string complianceYear,
            Func<Guid, string, ISchemeEeeDataQuerySet> schemeEeeDataQuerySetDelegate,
            IProducerQuerySet producerQuerySet)
        {
            this.organisationId = organisationId;
            this.complianceYear = complianceYear;
            this.producerQuerySet = producerQuerySet;

            schemeEeeDataQuerySet = schemeEeeDataQuerySetDelegate(organisationId, complianceYear);
        }

        public async Task<RuleResult> Evaluate(producerType producer)
        {
            var result = RuleResult.Pass();

            if (producer.status == statusType.A &&
                producer.obligationType != obligationTypeType.Both)
            {
                var existingProducerDetails =
                    producerQuerySet.GetLatestProducerForComplianceYearAndScheme(producer.registrationNo, complianceYear, organisationId);

                if (existingProducerDetails != null)
                {
                    var newObligationType = producer.obligationType.ToDomainObligationType();

                    if (existingProducerDetails.ObligationType != newObligationType)
                    {
                        var producerEeeData = await schemeEeeDataQuerySet.GetLatestProducerEeeData(producer.registrationNo);

                        if (producerEeeData != null &&
                            !producerEeeData.All(e => newObligationType.HasFlag(e.ObligationType)))
                        {
                            var eeeDataObligationTypes = producerEeeData
                                .Select(e => e.ObligationType)
                                .Distinct();

                            string obligationTypeMessage = eeeDataObligationTypes.Count() == 1 ?
                                string.Format("{0} EEE data has", eeeDataObligationTypes.Single())
                                : "both B2B and B2C EEE data have";

                            var errorMessage =
                                string.Format("You are attempting to change the obligation type of {0} {1} from {2} to {3}. You cannot make this change because {4} already been submitted for this producer.",
                                producer.GetProducerName(),
                                producer.registrationNo,
                                existingProducerDetails.ObligationType,
                                newObligationType,
                                obligationTypeMessage);

                            result = RuleResult.Fail(errorMessage, ErrorLevel.Error);
                        }
                    }
                }
            }

            return result;
        }
    }
}
