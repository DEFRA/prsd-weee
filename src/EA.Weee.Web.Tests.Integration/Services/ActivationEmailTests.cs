namespace EA.Weee.Web.Tests.Integration.Services
{
    using System;
    using System.IO;
    using System.Net.Mail;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Api.Client;
    using Api.Client.Actions;
    using Areas.Admin.Controllers;
    using Areas.Admin.ViewModels;
    using FakeItEasy;
    using Microsoft.Owin.Security;
    using Prsd.Core.Web.OAuth;
    using Prsd.Core.Web.OpenId;
    using Web.Services;
    using Web.Services.EmailRules;
    using Xunit;

    public class ActivationEmailTests
    {
        private readonly Func<IWeeeClient> fakeApiClient;
        private readonly IAuthenticationManager fakeAuthenticationManager;
        private readonly ISmtpClient fakeSmtpClient;
        private readonly Func<IOAuthClient> fakeOauthClient;
        private readonly IRuleChecker fakeRuleChecker;
        private readonly Func<IUserInfoClient> fakeUserInfoClient;

        private readonly IEmailService realEmailService;

        private const string FakeDomain = "https://www.example.com";
        private const string MailFrom = "sent_from@example.com";
        private const string MailTo = "sent_to@example.com";
        private const string ActivationToken = "fake_activation_token";

        public ActivationEmailTests()
        {
            fakeApiClient = SetupFakeApiClient(ActivationToken);
            fakeAuthenticationManager = A.Fake<IAuthenticationManager>();
            fakeSmtpClient = A.Fake<ISmtpClient>();
            fakeOauthClient = A.Fake<Func<IOAuthClient>>();
            fakeRuleChecker = A.Fake<IRuleChecker>();
            fakeUserInfoClient = A.Fake<Func<IUserInfoClient>>();
            A.CallTo(() => fakeRuleChecker.CheckEmailAddress(A<string>._)).Returns(RuleAction.Allow);

            realEmailService = new EmailService(SetupFakeConfigurationService(), new EmailTemplateService(), fakeSmtpClient, fakeRuleChecker);
        }

        private Func<IWeeeClient> SetupFakeApiClient(string activationTokenToUse)
        {
            var newUser = A.Fake<INewUser>();
            var weeeClient = A.Fake<IWeeeClient>();
            var apiClientFunction = A.Fake<Func<IWeeeClient>>();
            A.CallTo(() => newUser.GetUserAccountActivationTokenAsync(A<string>._)).Returns(activationTokenToUse);
            A.CallTo(() => weeeClient.NewUser).Returns(newUser);
            A.CallTo(() => apiClientFunction()).Returns(weeeClient);
            return apiClientFunction;
        }

        private ConfigurationService SetupFakeConfigurationService()
        {
            var configurationService = A.Fake<ConfigurationService>();
            var currentConfiguration = A.Fake<IAppConfiguration>();
            A.CallTo(() => currentConfiguration.MailFrom).Returns(MailFrom);
            A.CallTo(() => currentConfiguration.SendEmail).Returns("true");
            A.CallTo(() => configurationService.CurrentConfiguration).Returns(currentConfiguration);
            return configurationService;
        }

        [Fact]
        public void Internal_GenerateUserAccountActivationMessage_ValidInput_ReturnsCorrectContent()
        {
            // arrange

            MailMessage resultMessage = null;
            A.CallTo(() => fakeSmtpClient.SendMailAsync(A<MailMessage>._)).Invokes((MailMessage m) => resultMessage = m);

            var internalAccountController = SetupInternalAccountControllerAndContext();

            // act

            var actionResult = internalAccountController.Create(new InternalUserCreationViewModel
            {
                Email = MailTo,
            });

            // assert

            Assert.NotNull(actionResult);
            Assert.NotNull(resultMessage);

            string expectedHtmlBody = GetExpectedHtmlBody(ActivationToken);

            var htmlContentStream = resultMessage.AlternateViews[1].ContentStream;
            string actualHtmlBody = new StreamReader(htmlContentStream).ReadToEnd();

            Assert.Equal(expectedHtmlBody, actualHtmlBody);

            Assert.Equal(MailTo, resultMessage.To.ToString());
            Assert.Equal(MailFrom, resultMessage.From.ToString());
        }

        private AccountController SetupInternalAccountControllerAndContext()
        {
            HttpContext.Current = new HttpContext(
                new HttpRequest(string.Empty, FakeDomain, string.Empty),
                new HttpResponse(new StringWriter()));

            var httpContext = A.Fake<HttpContextBase>();
            var request = GetFakeRequest();
            A.CallTo(() => httpContext.Request).Returns(request);

            var controllerContext = new ControllerContext(httpContext, new RouteData(), A.Fake<ControllerBase>());

            var urlHelper = A.Fake<UrlHelper>();
            A.CallTo(() => urlHelper.Action(A<string>._, A<string>._, A<object>._, A<string>._))
                .Returns(FakeDomain);

            var internalAccountController = new AccountController(
                fakeApiClient,
                fakeAuthenticationManager,
                realEmailService,
                fakeOauthClient,
                fakeUserInfoClient)
            {
                ControllerContext = controllerContext,
                Url = urlHelper
            };

            return internalAccountController;
        }

        private HttpRequestBase GetFakeRequest()
        {
            var request = A.Fake<HttpRequestBase>();
            var url = new Uri(FakeDomain);

            A.CallTo(() => request.Url).Returns(url);

            return request;
        }

        private string GetExpectedHtmlBody(string expectedActivationToken)
        {
            return string.Format(
@"<html>
<head>
    <meta content=""text/html;charset=utf-8"" http-equiv=""Content-Type"">
    <meta content=""utf-8"" http-equiv=""encoding"">
    <title>Activate your user account</title>
</head>

<body style=""font-family: Helvetica, Arial, sans-serif; font-size: 16px; margin: 0; color: #0b0c0c"">
    <table width=""100%"" cellpadding=""15"" cellspacing=""0"" border=""0"">
        <tr>
            <td width=""100%"" height=""55px"" bgcolor=""#0b0c0c"">
                <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"" align=""center"">
                    <tr>
                        <td width=""100%"" bgcolor=""#0b0c0c"" valign=""middle"">
                            <img src=cid:logo alt=""GOV.UK"">
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <h1 class=""email"" style=""font-weight: 700; font-size: 36px; line-height: 1.041666667; margin: 30px 0;"">
        Activate your user account
    </h1>

    <p>Your user account has been created on the Waste Electrical and Electronic Equipment service.</p>
    <p>
        To use this service you need to <a href=""https://www.example.com//?code={0}"">activate your account.</a>
    </p>
    <p>If you didn’t create a user account you don’t need to take any action.</p>
    <p>WEEE Team</p>
</body>
</html>", expectedActivationToken);
        }
    }
}
