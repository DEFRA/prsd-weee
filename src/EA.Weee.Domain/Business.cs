namespace EA.Weee.Domain
{
    using Prsd.Core;

    public class Business
    {
        public string Name { get; private set; }

        public string Type { get; private set; }

        public string RegistrationNumber { get; private set; }

        public string AdditionalRegistrationNumber { get; private set; }

        public Business(string name, string type, string registrationNumber, string additionalRegistrationNumber)
        {
            Guard.ArgumentNotNull(name);
            Guard.ArgumentNotNull(type);

            Name = name;
            Type = type;
            RegistrationNumber = registrationNumber;
            AdditionalRegistrationNumber = additionalRegistrationNumber;
        }

        private Business()
        {
        }
    }
}
