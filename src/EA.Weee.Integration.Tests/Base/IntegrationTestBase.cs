namespace EA.Weee.Integration.Tests.Base
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Net;
    using System.Security.Claims;
    using Api.Identity;
    using Autofac;
    using Builders;
    using Dapper;
    using DataAccess;
    using DataAccess.Identity;
    using Domain.AatfReturn;
    using Domain.Organisation;
    using Domain.Scheme;
    using IoC;
    using Microsoft.AspNet.Identity;
    using netDumbster.smtp;
    using NUnit.Framework;
    using Prsd.Core.Autofac;
    using Prsd.Core.Domain;

    /// <summary>
    /// Contains all shared code for integration tests
    /// </summary>
    [TestFixture(Category = "IntegrationTest")]
    [Category("IntegrationTest")]
    public class IntegrationTestBase : TestBase
    {
        public static HostEnvironmentType CurrentHostEnvironment = HostEnvironmentType.Console;
        public static IocApplication CurrentAppUnderTest = IocApplication.Unknown;
        protected static IntegrationTestSetupBuilder test;
        private static IContainer requestHandlerContainer;
        public static CommonTestQueryProcessor Query => commonTestQueryProcessor.Value;
        protected static Lazy<CommonTestQueryProcessor> commonTestQueryProcessor = new Lazy<CommonTestQueryProcessor>(() => new CommonTestQueryProcessor());
        private static SimpleSmtpServer smtpServer;
        private static SmtpMessage emailSent;
        private static readonly List<SmtpMessage> EmailsSent = new List<SmtpMessage>();

        public static Organisation DefaultIntegrationOrganisation { get; set; }

        public static Scheme DefaultScheme { get; set; }

        public static Aatf DefaultAatf { get; set; }

        public static ClaimsPrincipal Principal { get; set; }
        public static ApplicationUser User { get; set; }

        public static Guid UserId => User != null ? Guid.Parse(User.Id) : Guid.Empty;

        public static IntegrationTestSetupBuilder SetupTest(IocApplication app)
        {
            return test = new IntegrationTestSetupBuilder(app);
        }

        public static IContainer Container
        {
            get
            {
                switch (CurrentAppUnderTest)
                {
                    case IocApplication.RequestHandler:
                        return requestHandlerContainer;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(CurrentAppUnderTest), CurrentAppUnderTest, null);
                }
            }
            set
            {
                switch (CurrentAppUnderTest)
                {
                    case IocApplication.RequestHandler:
                        requestHandlerContainer = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(CurrentAppUnderTest), CurrentAppUnderTest, null);
                }
            }
        }

        public class IntegrationTestSetupBuilder
        {
            public IntegrationTestSetupBuilder(IocApplication application)
            {
                CurrentAppUnderTest = application;
                CurrentHostEnvironment = HostEnvironmentType.Console;
            }

            public IntegrationTestSetupBuilder WithExternalUserAccess()
            {
                if (Principal == null)
                {
                    throw new InvalidOperationException("Principal must have been set to login as particular user type");
                }
                
                var userContext = new TestUserContext(Guid.Parse(User.Id), true);
                InitIocWithUser(userContext);

                return this;
            }

            public IntegrationTestSetupBuilder WithInternalAdminUserAccess()
            {
                if (Principal == null)
                {
                    throw new InvalidOperationException("Principal must have been set to login as particular user type");
                }

                var userContext = new TestUserContext(Guid.Parse(User.Id), false, true);
                InitIocWithUser(userContext);

                return this;
            }

            public IntegrationTestSetupBuilder WithDefaultSettings(bool resetDb = false)
            {
                return WithIoC(true)
                    .WithTestData(resetDb);
            }

            public IntegrationTestSetupBuilder WithTestData(bool reset = false)
            {
                ResetTestDatabaseData(reset);
                return this;
            }

            public IntegrationTestSetupBuilder WithIoC(bool resetIoC = false, bool installIoC = true)
            {
                InitIoC(resetIoC, installIoC);
                return this;
            }

            protected static IContainer Init(IocApplication app, bool resetIoC = false, bool installIoC = true)
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                CurrentAppUnderTest = app;
                CurrentHostEnvironment = HostEnvironmentType.Console;

                InitIoC(resetIoC, installIoC);

                return Container;
            }

            protected static void ResetTestDatabaseData(bool hardReset = false)
            {
                if (TestingStatus.IsDbSeedingFaulted)
                {
                    throw new DatabaseSeedingFailureException();
                }

                try
                {
                    // check if DB exists before anything
                    RunSql($"USE master; SELECT * FROM sys.databases WHERE name = '{ConfigurationManager.AppSettings["aliaSqlConnectionDatabase"]}'");

                    if (hardReset)
                    {
                        new DatabaseSeeder().RebuildDatabase();
                    }

                    var userManager = Container.Resolve<ApplicationUserManager>();
                    const string user = "integration-email@email.com";
                    User = userManager.FindByEmail(user);

                    if (User == null)
                    {
                        var userId = Guid.NewGuid();
                        User = new ApplicationUser()
                        {
                            Id = userId.ToString(), Email = user, EmailConfirmed = true, PasswordHash = "password",
                            FirstName = Faker.Name.First(), Surname = Faker.Name.Last(), UserName = user
                        };

                        var created = userManager.Create(User);

                        if (created.Succeeded)
                        {
                            Console.WriteLine($"Created default user with id { userId }");
                        }
                        else
                        {
                            Console.WriteLine($"Failed to create default user { string.Join(",", created.Errors)} ");
                        }
                    }

                    Console.WriteLine($"Using default user with id { User.Id } ");

                    var testUserContextUpdater = Container.Resolve<Action<TestUserContext>>();
                    testUserContextUpdater(new TestUserContext(Guid.Parse(User.Id), false));

                    var userc = Container.Resolve<IUserContext>();
                    
                    Principal = userc.Principal;
                    var weeContext = Container.Resolve<WeeeContext>();
                    DefaultIntegrationOrganisation = weeContext.Organisations.FirstOrDefault(o => o.Name.Equals(TestingConstants.TestCompanyName));
                    DefaultScheme = weeContext.Schemes.FirstOrDefault(s => s.SchemeName.Equals(TestingConstants.TestCompanyName));
                    DefaultAatf = weeContext.Aatfs.FirstOrDefault(a => a.Name.Equals(TestingConstants.TestCompanyName));

                    if (DefaultIntegrationOrganisation == null)
                    {
                        DefaultIntegrationOrganisation = OrganisationDbSetup.Init().With(o =>
                            o.UpdateRegisteredCompanyDetails(TestingConstants.TestCompanyName,
                                Faker.RandomNumber.Next(10000000, 99999999).ToString(), "trading")).Create();
                    }

                    if (DefaultScheme == null)
                    {
                        DefaultScheme = SchemeDbSetup.Init()
                            .With(s => s.UpdateScheme(TestingConstants.TestCompanyName, s.ApprovalNumber, s.IbisCustomerReference, s.ObligationType, s.CompetentAuthority))
                            .Create();
                    }

                    if (DefaultAatf == null)
                    {
                        DefaultAatf = AatfDbSetup.Init().With(a => a.UpdateDetails(TestingConstants.TestCompanyName, a.CompetentAuthority, a.ApprovalNumber, a.AatfStatus,
                                a.Organisation, a.Size, a.ApprovalDate, a.LocalArea, a.PanArea)).Create();
                    }

                    Console.WriteLine("Test database reseeded");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to recreate database {ex.Message}");
                    TestingStatus.IsDbSeedingFaulted = true;

                    throw new DatabaseSeedingFailureException();
                }

                TestingStatus.IsDbReseeded = true;
            }

            protected static IContainer InitIoC(bool resetIoC = false, bool installIoC = true)
            {
                var environment = new EnvironmentResolver
                {
                    IocApplication = CurrentAppUnderTest,
                    HostEnvironment = CurrentHostEnvironment,
                    IsTestRun = true
                };

                var iocFactory = new IoCFactory();

                if (resetIoC)
                {
                    iocFactory.RemoveContainer(environment);
                }

                if (installIoC)
                {
                    ServiceLocator.Container = Container = iocFactory.GetOrCreateContainer(environment);
                }

                return Container;
            }

            protected static IContainer InitIocWithUser(TestUserContext userContext)
            {
                var environment = new EnvironmentResolver
                {
                    IocApplication = CurrentAppUnderTest,
                    HostEnvironment = CurrentHostEnvironment,
                    IsTestRun = true
                };

                var iocFactory = new IoCFactory();
                iocFactory.RemoveContainer(environment);
                ServiceLocator.Container = Container = iocFactory.GetOrCreateContainer(environment, userContext);
                
                return Container;
            }
        }

        public static void RunSql(string sql)
        {
            using (var db = new SqlConnection(ConfigurationManager.ConnectionStrings["Weee.DefaultConnection"].ToString()))
            {
                try
                {
                    db.Open();

                    db.Execute(sql);
                }
                catch (SqlException ex)
                {
                    Console.Write($"SQL: {ex.Message}");

                    if (ex.Message.Contains("No process is on the other end of the pipe") ||
                        ex.Message.Contains("Cannot open database"))
                    {
                        // rebuild the db
                        new DatabaseSeeder().RebuildDatabase();
                    }
                }
                catch (Exception ex)
                {
                    Console.Write($"Exception running SQL during a test run. SQL was:\n{sql}\nException: {ex}");
                    throw;
                }
                finally
                {
                    new DatabaseSeeder().UpdateDatabase();
                }
            }
        }

        public static SmtpMessage GetEmailFromMailCatcher()
        {
            return emailSent;
        }

        public static SmtpMessage[] GetEmailsFromMailCatcher()
        {
            return EmailsSent.ToArray();
        }

        public static void UseMailCatcher()
        {
            smtpServer?.Stop();
            smtpServer = SimpleSmtpServer.Start(5124);
            smtpServer.MessageReceived += SmtpServer_MessageReceived;
            smtpServer.ClearReceivedEmail();
            emailSent = null;
            EmailsSent.Clear();
        }

        private static void SmtpServer_MessageReceived(object sender, MessageReceivedArgs e)
        {
            emailSent = e.Message;
            EmailsSent.Add(e.Message);
        }

        protected static void Act(Action action)
        {
            emailSent = null;
            EmailsSent.Clear();

            smtpServer?.ClearReceivedEmail();

            CatchException(action);

            if (emailSent != null)
            {
                Console.WriteLine("Email content: " + GetEmailFromMailCatcher().MessageParts.FirstOrDefault()?.BodyData);
            }
        }

        public override void Dispose()
        {
        }
    }
}
