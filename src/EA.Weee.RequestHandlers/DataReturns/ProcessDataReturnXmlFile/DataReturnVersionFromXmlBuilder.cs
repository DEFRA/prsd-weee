namespace EA.Weee.RequestHandlers.DataReturns.ProcessDataReturnXmlFile
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using BusinessValidation;
    using BusinessValidation.Rules;
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
        private readonly ISchemeApprovalNumberMismatch schemeApprovalNumberMismatch;

        public DataReturnVersionFromXmlBuilder(
            IDataReturnVersionBuilder dataReturnVersionBuilder,
            ISchemeApprovalNumberMismatch schemeApprovalNumberMismatch)
        {
            Guard.ArgumentNotNull(() => dataReturnVersionBuilder, dataReturnVersionBuilder);
            this.dataReturnVersionBuilder = dataReturnVersionBuilder;
            this.schemeApprovalNumberMismatch = schemeApprovalNumberMismatch;
        }

        public async Task<DataReturnVersionBuilderResult> Build(SchemeReturn schemeReturn)
        {
            // PreValidate (any validation before business validation)
            var preValidationErrors = await dataReturnVersionBuilder.PreValidate();

            // And process XML-specific validation
            var schemeApprovalNumberMismatchResult = schemeApprovalNumberMismatch.Validate(schemeReturn.ApprovalNo,
                dataReturnVersionBuilder.Scheme);

            var preBusinessValidationResult = new DataReturnVersionBuilderResult();
            preBusinessValidationResult.ErrorData.AddRange(preValidationErrors);
            preBusinessValidationResult.ErrorData.AddRange(schemeApprovalNumberMismatchResult);

            // If there are any pre-business validation errors return them
            if (preBusinessValidationResult.ErrorData.Any())
            {
                return preBusinessValidationResult;
            }

            // Then build the Data Return Version
            if (schemeReturn.ProducerList != null)
            {
                foreach (var producer in schemeReturn.ProducerList)
                {
                    foreach (var tonnageReturn in producer.Returns)
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

            if (schemeReturn.DeliveredToAATF != null)
            {
                foreach (var deliveredToAatf in schemeReturn.DeliveredToAATF)
                {
                    foreach (var tonnageReturn in deliveredToAatf.Returns)
                    {
                        await dataReturnVersionBuilder.AddAatfDeliveredAmount(deliveredToAatf.AATFApprovalNo, deliveredToAatf.FacilityName, tonnageReturn.CategoryName.ToDomainWeeeCategory(),
                                     tonnageReturn.ObligationType.ToDomainObligationType(), tonnageReturn.TonnesReturnValue);
                    }
                }
            }

            if (schemeReturn.DeliveredToAE != null)
            {
                foreach (var deliveredToAe in schemeReturn.DeliveredToAE)
                {
                    foreach (var tonnageReturn in deliveredToAe.Returns)
                    {
                        await dataReturnVersionBuilder.AddAeDeliveredAmount(deliveredToAe.AEApprovalNo, deliveredToAe.OperatorName, tonnageReturn.CategoryName.ToDomainWeeeCategory(),
                                         tonnageReturn.ObligationType.ToDomainObligationType(), tonnageReturn.TonnesReturnValue);
                    }
                }
            }

            return await dataReturnVersionBuilder.Build();
        }
    }
}
