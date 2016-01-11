namespace EA.Weee.RequestHandlers.DataReturns.ProcessDataReturnXmlFile
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using BusinessValidation;
    using Core.Shared;
    using Domain.DataReturns;
    using Prsd.Core;
    using ReturnVersionBuilder;
    using Shared;
    using Xml.DataReturns;

    /// <summary>
    /// Builds data return version domain objects for a specified scheme and quarter by processing
    /// a SchemeReturn object representing the contents of a schematically valid XML file.
    /// The scheme and quarter are determined by the IDataReturnVersionBuilder provided.
    /// </summary>
    public class DataReturnVersionFromXmlBuilder : IDataReturnVersionFromXmlBuilder
    {
        private readonly IDataReturnVersionBuilder dataReturnVersionBuilder;
        private readonly IXmlBusinessValidator xmlBusinessValidator;

        public DataReturnVersionFromXmlBuilder(
            IDataReturnVersionBuilder dataReturnVersionBuilder,
            IXmlBusinessValidator xmlBusinessValidator)
        {
            Guard.ArgumentNotNull(() => dataReturnVersionBuilder, dataReturnVersionBuilder);
            this.dataReturnVersionBuilder = dataReturnVersionBuilder;
            this.xmlBusinessValidator = xmlBusinessValidator;
        }

        public async Task<DataReturnVersionBuilderResult> Build(SchemeReturn schemeReturn)
        {
            // Process XML-specific validation first
            var businessValidationResult = await xmlBusinessValidator.Validate(schemeReturn);

            if (businessValidationResult.ErrorData.Any())
            {
                return businessValidationResult;
            }

            if (schemeReturn.ProducerList != null)
            {
                foreach (var producer in schemeReturn.ProducerList)
                {
                    foreach (var tonnageReturn in producer.Return)
                    {
                        await dataReturnVersionBuilder.AddEeeOutputAmount(producer.RegistrationNo,
                              producer.ProducerCompanyName, tonnageReturn.CategoryName.ToDomainWeeeCategory(),
                              tonnageReturn.ObligationType.ToDomainObligationType(), tonnageReturn.TonnesReturnValue);
                    }
                }
            }

            if (schemeReturn.CollectedFromDCF != null)
            {
                foreach (var collectedFromDcf in schemeReturn.CollectedFromDCF)
                {
                    await dataReturnVersionBuilder.AddWeeeCollectedAmount(WeeeCollectedAmountSourceType.Dcf,
                         collectedFromDcf.CategoryName.ToDomainWeeeCategory(), collectedFromDcf.ObligationType.ToDomainObligationType(), collectedFromDcf.TonnesReturnValue);
                }
            }

            if (schemeReturn.B2CWEEEFromDistributors != null)
            {
                foreach (var weeeFromDistributor in schemeReturn.B2CWEEEFromDistributors)
                {
                    await dataReturnVersionBuilder.AddWeeeCollectedAmount(WeeeCollectedAmountSourceType.Distributor,
                            weeeFromDistributor.CategoryName.ToDomainWeeeCategory(), weeeFromDistributor.ObligationType.ToDomainObligationType(), weeeFromDistributor.TonnesReturnValue);
                }
            }

            if (schemeReturn.B2CWEEEFromFinalHolders != null)
            {
                foreach (var weeeFromFinalHolders in schemeReturn.B2CWEEEFromFinalHolders)
                {
                    await dataReturnVersionBuilder.AddWeeeCollectedAmount(WeeeCollectedAmountSourceType.FinalHolder,
                           weeeFromFinalHolders.CategoryName.ToDomainWeeeCategory(), weeeFromFinalHolders.ObligationType.ToDomainObligationType(), weeeFromFinalHolders.TonnesReturnValue);
                }
            }

            if (schemeReturn.DeliveredToATF != null)
            {
                foreach (var deliveredToAatf in schemeReturn.DeliveredToATF)
                {
                    var facility = deliveredToAatf.DeliveredToFacility;

                    foreach (var tonnageReturn in deliveredToAatf.Return)
                    {
                        await dataReturnVersionBuilder.AddAatfDeliveredAmount(facility.AATFApprovalNo, facility.FacilityName, tonnageReturn.CategoryName.ToDomainWeeeCategory(),
                                     tonnageReturn.ObligationType.ToDomainObligationType(), tonnageReturn.TonnesReturnValue);
                    }
                }
            }

            if (schemeReturn.DeliveredToAE != null)
            {
                foreach (var deliveredToAe in schemeReturn.DeliveredToAE)
                {
                    var deliveredToOperator = deliveredToAe.DeliveredToOperator;

                    foreach (var tonnageReturn in deliveredToAe.Return)
                    {
                        await dataReturnVersionBuilder.AddAeDeliveredAmount(deliveredToOperator.AEApprovalNo, deliveredToOperator.OperatorName, tonnageReturn.CategoryName.ToDomainWeeeCategory(),
                                         tonnageReturn.ObligationType.ToDomainObligationType(), tonnageReturn.TonnesReturnValue);
                    }
                }
            }

            return dataReturnVersionBuilder.Build();
        }
    }
}
