namespace EA.Prsd.Core.DataAccess.Tests.Integration.Model.Domain
{
    using EA.Prsd.Core.Domain;

    public class SimpleEntity : Entity
    {
        public string Data { get; set; }

        public SimpleEntity()
        {
            Data = "Default entity";
        }

        public SimpleEntity(string data)
        {
            Data = data;
        }
    }
}
