namespace EA.Weee.Web.Tests.Unit.Areas.PCS.Controllers
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using Api.Client;
    using EA.Weee.Web.Tests.Unit.TestHelpers;
    using FakeItEasy;
    using Prsd.Core.Mediator;
    using Services;
    using Web.Areas.PCS.Controllers;
    using Weee.Requests.Organisations;
    using Weee.Requests.PCS.MemberRegistration;
    using Xunit;

    public class MemberRegistrationControllerTests
    {
         private readonly IWeeeClient weeeClient;
        private readonly IFileConverterService fileConverter;

        public MemberRegistrationControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            fileConverter = A.Fake<IFileConverterService>();
        }

        private MemberRegistrationController MemberRegistrationController()
        {
            var controller = new MemberRegistrationController(() => weeeClient, fileConverter);
            new HttpContextMocker().AttachToController(controller);

            return controller;
        }

        [Fact]
        public async void GetAddOrAmendMembers_ChecksForValidityOfOrganisation()
        {
            try
            {
                await MemberRegistrationController().AddOrAmendMembers(A<Guid>._);
            }
            catch (Exception)
            {
            }

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void GetAddOrAmendMembers_IdDoesNotBelongToAnExistingOrganisation_ThrowsException()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .Returns(false);

            await Assert.ThrowsAnyAsync<Exception>(() => MemberRegistrationController().AddOrAmendMembers(A<Guid>._));
        }

        [Fact]
        public async void GetAddOrAmendMembers_IdDoesBelongToAnExistingOrganisation_ReturnsView()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .Returns(true);

            var result = await MemberRegistrationController().AddOrAmendMembers(A<Guid>._);

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void PostAddOrAmendMembers_ConvertsFileToString()
        {
            var postedFile = A.Fake<HttpPostedFileBase>();
            try
            {
                await MemberRegistrationController().AddOrAmendMembers(A<Guid>._, postedFile);
            }
            catch (Exception)
            {
            }

            A.CallTo(() => fileConverter.Convert(A<HttpPostedFileBase>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void PostAddOrAmendMembers_FileIsConvertedSuccessfully_ValidateRequestSentWithConvertedFileDataAndOrganisationId()
        {
            const string fileData = "myFileContent";
            var organisationId = Guid.NewGuid();
            var request = new ValidateXmlFile(A<Guid>._, A<string>._);
            var postedFile = A.Fake<HttpPostedFileBase>();

            A.CallTo(() => fileConverter.Convert(A<HttpPostedFileBase>._))
                .Returns(fileData);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<ValidateXmlFile>._))
                .Invokes((string token, IRequest<Guid> req) => request = (ValidateXmlFile)req);

            try
            {
                await MemberRegistrationController().AddOrAmendMembers(organisationId, postedFile);
            }
            catch (Exception)
            {
            }

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<ValidateXmlFile>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            Assert.NotNull(request);
            Assert.Equal(fileData, request.Data);
            Assert.Equal(organisationId, request.OrganisationId);
        }

        [Fact]
        public async void PostAddOrAmendMembers_ValidateRequestIsProcessedSuccessfully_RedirectsToResults()
        {
            var validationId = Guid.NewGuid();
            var postedFile = A.Fake<HttpPostedFileBase>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<ValidateXmlFile>._))
                .Returns(validationId);

            var result = await MemberRegistrationController().AddOrAmendMembers(A<Guid>._, postedFile);
            var redirect = (RedirectToRouteResult)result;

            Assert.Equal("ViewErrorsAndWarnings", redirect.RouteValues["action"]);
            Assert.Equal(validationId, redirect.RouteValues["memberUploadId"]);
        }
    }
}
