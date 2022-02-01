namespace EA.Weee.Integration.Tests.Base
{
    using System;
    using System.Configuration;
    using System.Data.Entity;
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
    using Domain.Organisation;
    using IoC;
    using Microsoft.AspNet.Identity;
    using NUnit.Framework;
    using Prsd.Core.Autofac;


    /// <summary>
    /// Contains all shared code for integration tests
    /// </summary>
    [TestFixture(Category = "IntegrationTest")]
    [Category("IntegrationTest")]
    public class IntegrationTestBase
    {
        public static HostEnvironmentType CurrentHostEnvironment = HostEnvironmentType.Console;
        public static IocApplication CurrentAppUnderTest = IocApplication.Unknown;
        protected static IntegrationTestSetupBuilder Test;
        private static IContainer _requestHandlerContainer;

        public static Organisation DefaultIntegrationOrganisation { get; set; }

        public static ClaimsPrincipal Principal { get; set; }
        public static ApplicationUser User { get; set; }

        public static Guid UserId => User != null ? Guid.Parse(User.Id) : Guid.Empty;

        public static IntegrationTestSetupBuilder SetupTest(IocApplication app)
        {
            return Test = new IntegrationTestSetupBuilder(app);
        }

        public static IContainer Container
        {
            get
            {
                switch (CurrentAppUnderTest)
                {
                    case IocApplication.RequestHandler:
                        return _requestHandlerContainer;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(CurrentAppUnderTest), CurrentAppUnderTest, null);
                }
            }
            set
            {
                switch (CurrentAppUnderTest)
                {
                    case IocApplication.RequestHandler:
                        _requestHandlerContainer = value;
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

            public IntegrationTestSetupBuilder WithDefaultSettings(bool resetDb = false)
            {
                return WithIoC()
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

            protected static IContainer Init(IocApplication app, bool resetIoC = false, bool installIoC = true, bool reseedDb = false)
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
                    RunSql("USE master; SELECT * FROM sys.databases WHERE name = 'EA.Weee.Integration'");

                    const string company = "Integration Test Company";
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
                    testUserContextUpdater(new TestUserContext(Guid.Parse(User.Id)));

                    OrganisationDbSetup.Init().With(o =>
                        o.UpdateRegisteredCompanyDetails(company,
                            Faker.RandomNumber.Next(10000000, 99999999).ToString(), "trading")).Create();

                    var weeContext = Container.Resolve<WeeeContext>();
                    DefaultIntegrationOrganisation = weeContext.Organisations.First(o => o.Name.Equals(company));

                    if (DefaultIntegrationOrganisation == null)
                    {
                        DefaultIntegrationOrganisation = OrganisationDbSetup.Init().With(o =>
                            o.UpdateRegisteredCompanyDetails(company,
                                Faker.RandomNumber.Next(10000000, 99999999).ToString(), "trading")).Create();
                    }

                    Console.WriteLine("Test database reseeded");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to recreate database {ex.Message}");
                    TestingStatus.IsDbSeedingFaulted = true;
                }
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

                // Default user Id // can be overridden

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
                    if (ex.Message.Contains("No process is on the other end of the pipe"))
                    {
                        // rebuild the db
                        new DatabaseSeeder().RebuildDatabase();
                    }
                }
                catch (Exception ex)
                {
                    Console.Out.WriteLine($"Exception running SQL during a test run. SQL was:\n{sql}\nException: {ex}");
                    throw;
                }
            }
        }
    }
}
