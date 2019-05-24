namespace EA.Weee.Migration.Organisation.Tests.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using FakeItEasy;
    using FluentAssertions;
    using Model;
    using Organisation.Validation;
    using Xunit;

    public class OrganisationDataListValidatorTests
    {
        [Fact]
        public void CheckHasErrors_AddressDataHasErrors_FalseReturned()
        {
            A.CallTo(() => A<OrganisationDataValidator>_.Validate(A<Organisation>._)).Returns(true);
            var result = OrganisationDataListValidator.CheckHasErrors(A.CollectionOfFake<Organisation>(2));

            result.Should().BeFalse();
        }
    }
}
