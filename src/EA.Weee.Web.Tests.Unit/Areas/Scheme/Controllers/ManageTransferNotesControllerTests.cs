namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Controllers
{
    using FluentAssertions;
    using Web.Areas.Scheme.Controllers;
    using Xunit;

    public class ManageTransferNotesControllerTests
    {
        [Fact]
        public void CheckManageTransferNotesControllerInheritsBalancingSchemeEvidenceBaseController()
        {
            typeof(ManageTransferNotesController).BaseType.Name.Should().Be(nameof(BalancingSchemeEvidenceBaseController));
        }
    }
}
