namespace EA.Prsd.Core.Autofac
{
    public interface IEnvironmentResolver
    {
        HostEnvironmentType HostEnvironment { get; set; }
        IocApplication IocApplication { get; set; }

        bool IsTestRun { get; set; }
    }
}
