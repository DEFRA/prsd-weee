﻿namespace EA.Weee.RequestHandlers.Admin.GetProducerDetails
{
    using Core.Admin;
    using Domain.DataReturns;
    using Domain.Producer;
    using Prsd.Core.Mediator;
    using Security;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Weee.Security;

    public class GetProducerDetailsByRegisteredProducerIdHandler : IRequestHandler<Requests.Admin.GetProducerDetailsByRegisteredProducerId, ProducerDetailsScheme>
    {
        private readonly IGetProducerDetailsByRegisteredProducerIdDataAccess dataAccess;
        private readonly IWeeeAuthorization authorization;

        public GetProducerDetailsByRegisteredProducerIdHandler(IGetProducerDetailsByRegisteredProducerIdDataAccess dataAccess, IWeeeAuthorization authorization)
        {
            this.dataAccess = dataAccess;
            this.authorization = authorization;
        }

        public async Task<ProducerDetailsScheme> HandleAsync(Requests.Admin.GetProducerDetailsByRegisteredProducerId request)
        {
            authorization.EnsureCanAccessInternalArea();

            RegisteredProducer registeredProducer = await dataAccess.Fetch(request.RegisteredProducerId);

            if (registeredProducer == null)
            {
                string message = string.Format(
                    "No producer has been registered with the id \"{0}\".",
                    request.RegisteredProducerId);

                throw new Exception(message);
            }

            bool hasSubmittedEee = false;
            IEnumerable<DataReturn> dataReturns = await dataAccess.FetchDataReturns(registeredProducer.Scheme, registeredProducer.ComplianceYear);
            foreach (DataReturn dataReturn in dataReturns)
            {
                if (dataReturn.CurrentVersion != null &&
                    dataReturn.CurrentVersion.EeeOutputReturnVersion != null)
                {
                    ICollection<EeeOutputAmount> eeeOutputAmounts = dataReturn.CurrentVersion.EeeOutputReturnVersion.EeeOutputAmounts;
                    if (eeeOutputAmounts.Any(a => a.RegisteredProducer == registeredProducer))
                    {
                        hasSubmittedEee = true;
                        break;
                    }
                }
            }

            var canRemoveProducer = authorization.CheckUserInRole(Roles.InternalAdmin);

            return new ProducerDetailsScheme
            {
                SchemeName = registeredProducer.Scheme.SchemeName,
                ComplianceYear = registeredProducer.ComplianceYear,
                ProducerName = registeredProducer.CurrentSubmission.OrganisationName,
                RegistrationNumber = registeredProducer.ProducerRegistrationNumber,
                HasSubmittedEEE = hasSubmittedEee,
                CanRemoveProducer = canRemoveProducer
            };
        }
    }
}
