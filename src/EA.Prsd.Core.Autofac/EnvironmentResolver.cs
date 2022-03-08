namespace EA.Prsd.Core.Autofac
{
    public class EnvironmentResolver : IEnvironmentResolver
    {
        public HostEnvironmentType HostEnvironment { get; set; }

        public IocApplication IocApplication { get; set; }

        public bool IsTestRun { get; set; }

        public override string ToString()
        {
            return $"HostEnvironment: {HostEnvironment}, IocApplication: {IocApplication}, IsTestRun: {IsTestRun}";
        }
    }
}
