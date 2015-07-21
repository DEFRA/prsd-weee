namespace EA.Weee.Web.Tests.Unit.Areas.PCS.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using Api.Client;
    using Core.PCS;
    using Core.Shared;
    using EA.Weee.Web.Tests.Unit.TestHelpers;
    using FakeItEasy;
    using Prsd.Core.Mediator;
    using Services;
    using Web.Areas.PCS.Controllers;
    using Web.Areas.PCS.ViewModels;
    using Weee.Requests.Organisations;
    using Weee.Requests.PCS.MemberRegistration;
    using Weee.Requests.Shared;
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
        public async void PostAddOrAmendMembers_ModelIsInvalid_ReturnsView()
        {
            var controller = MemberRegistrationController();
            controller.ModelState.AddModelError("ErrorKey", "Some kind of error goes here");

            var result = await controller.AddOrAmendMembers(A<Guid>._, new AddOrAmendMembersViewModel());

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void PostAddOrAmendMembers_ConvertsFileToString()
        {
            try
            {
                await MemberRegistrationController().AddOrAmendMembers(A<Guid>._, new AddOrAmendMembersViewModel());
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

            A.CallTo(() => fileConverter.Convert(A<HttpPostedFileBase>._))
                .Returns(fileData);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<ValidateXmlFile>._))
                .Invokes((string token, IRequest<Guid> req) => request = (ValidateXmlFile)req);

            try
            {
                await MemberRegistrationController().AddOrAmendMembers(organisationId, new AddOrAmendMembersViewModel());
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

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<ValidateXmlFile>._))
                .Returns(validationId);

            var result = await MemberRegistrationController().AddOrAmendMembers(A<Guid>._, new AddOrAmendMembersViewModel());
            var redirect = (RedirectToRouteResult)result;

            Assert.Equal("ViewErrorsAndWarnings", redirect.RouteValues["action"]);
            Assert.Equal("MemberRegistration", redirect.RouteValues["controller"]);
            Assert.Equal(validationId, redirect.RouteValues["memberUploadId"]);
        }

        private const string XmlHasErrorsViewName = "ViewErrorsAndWarnings";
        private const string XmlHasNoErrorsViewName = "XmlHasNoErrors";

        [Fact]
        public async Task GetViewErrorsOrWarnings_NoErrors_ShowsAcceptedPage()
        {
            Assert.Equal(XmlHasNoErrorsViewName, await ViewAfterClientReturns(new List<MemberUploadErrorData> { }));
        }

        [Fact]
        public async Task GetViewErrorsOrWarnings_ErrorsPresent_ShowsErrorPage()
        {
            Assert.Equal(XmlHasErrorsViewName, await ViewAfterClientReturns(new List<MemberUploadErrorData> { new MemberUploadErrorData { ErrorLevel = ErrorLevel.Error } }));
        }

        [Fact]
        public async Task GetViewErrorsOrWarnings_WarningPresent_ShowsAcceptedPage()
        {
            Assert.Equal(XmlHasNoErrorsViewName, await ViewAfterClientReturns(new List<MemberUploadErrorData> { new MemberUploadErrorData { ErrorLevel = ErrorLevel.Warning } }));
        }

        [Fact]
        public async Task GetViewErrorsOrWarnings_NoErrors_HasProvidedErrorData()
        {
            var errors = new List<MemberUploadErrorData>();

            var providedErrors = await ErrorsAfterClientReturns(errors);

            Assert.Equal(errors, providedErrors);
        }

        [Fact]
        public async Task GetViewErrorsOrWarnings_ErrorsPresent_HasProvidedErrorData()
        {
            var errors = new List<MemberUploadErrorData>
            {
                new MemberUploadErrorData
                {
                    ErrorLevel = ErrorLevel.Error
                }
            };

            var providedErrors = await ErrorsAfterClientReturns(errors);

            Assert.Equal(errors, providedErrors);
        }

        [Fact]
        public async void GetProducerCSV_ValidMemberUploadId_ReturnsCSVFile()
        {
            var testCSVData = new ProducerCSVFileData { FileContent = "Test, Test, Test", FileName = "test.csv" };
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetProducerCSVByMemberUploadId>._))
                .Returns(testCSVData);

            var result = await MemberRegistrationController().GetProducerCSV(A<Guid>._);

            Assert.IsType<FileContentResult>(result);
        }

        [Fact]
        public async void PostSubmitXml_ValidMemberUploadId_ReturnsSuccessfulSubmissionView()
        {
            var memberUploadId = Guid.NewGuid();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<MemberUploadSubmission>._))
                .Returns(memberUploadId);

            var result = await MemberRegistrationController().SubmitXml(A<Guid>._, new MemberUploadResultViewModel { ErrorData = new List<MemberUploadErrorData>(), MemberUploadId = memberUploadId });

            var redirect = (RedirectToRouteResult)result;

            Assert.Equal("SuccessfulSubmission", redirect.RouteValues["action"]);
            Assert.Equal(memberUploadId, redirect.RouteValues["memberUploadId"]);
        }

        private async Task<List<MemberUploadErrorData>> ErrorsAfterClientReturns(List<MemberUploadErrorData> memberUploadErrorDatas)
        {
            var result = await GetViewResult(memberUploadErrorDatas);

            return ((MemberUploadResultViewModel)result.Model).ErrorData;
        }

        private async Task<string> ViewAfterClientReturns(List<MemberUploadErrorData> list)
        {
            return (await GetViewResult(list)).ViewName;
        }

        private async Task<ViewResult> GetViewResult(List<MemberUploadErrorData> memberUploadErrorDatas)
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetMemberUploadData>._))
                .Returns(memberUploadErrorDatas);

            return await MemberRegistrationController().ViewErrorsAndWarnings(A<Guid>._, A<Guid>._);
        }
    }
}
