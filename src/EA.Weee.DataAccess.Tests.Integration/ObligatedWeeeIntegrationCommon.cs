namespace EA.Weee.DataAccess.Tests.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Domain.DataReturns;
    using EA.Weee.Domain.Lookup;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using EA.Weee.Tests.Core.Model;
    using FluentAssertions;
    using RequestHandlers.AatfReturn.ObligatedGeneric;
    using Xunit;
    using Aatf = Domain.AatfReturn.Aatf;
    using Operator = Domain.AatfReturn.Operator;
    using Organisation = Domain.Organisation.Organisation;
    using Return = Domain.AatfReturn.Return;
    using Scheme = Domain.Scheme.Scheme;
    using WeeeReceived = Domain.AatfReturn.WeeeReceived;
    using WeeeReceivedAmount = Domain.AatfReturn.WeeeReceivedAmount;

    public static class ObligatedWeeeIntegrationCommon
    {
        public static Return CreateReturn(Operator operatorTest)
        {
            return new Return(operatorTest, new Quarter(2019, QuarterType.Q1), ReturnStatus.Created);
        }

        public static Aatf CreateAatf(UKCompetentAuthority competentAuthority, Operator @operator)
        {
            var aatf = new Aatf("aatfname", competentAuthority, "number", AatfStatus.Approved, @operator);
            return aatf;
        }

        public static Operator CreateOperator(Organisation organisation)
        {
            return new Operator(organisation);
        }

        public static Scheme CreateScheme(Organisation organisation)
        {
            return new Scheme(organisation);
        }

        public static Organisation CreateOrganisation()
        {
            var companyName = "Test Name" + Guid.NewGuid();
            var tradingName = "Test Trading Name" + Guid.NewGuid();
            const string companyRegistrationNumber = "ABC12345";

            var organisation = Organisation.CreateRegisteredCompany(companyName, companyRegistrationNumber, tradingName);
            return organisation;
        }
    }
}