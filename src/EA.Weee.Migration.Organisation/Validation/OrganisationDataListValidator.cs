namespace EA.Weee.Migration.Organisation.Validation
{
    using System.Collections.Generic;
    using FluentValidation;
    using Model;
    using NLog;

    public class OrganisationDataListValidator
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly OrganisationDataValidator OrganisationValidator = new OrganisationDataValidator();
        private static readonly AddressDataValidator AddressValidator = new AddressDataValidator();

        public static bool HasErrors(IList<Organisation> organisations)
        {
            var hasErrors = false;

            foreach (var organisation in organisations)
            {
                var addressResult = AddressValidator.Validate(organisation.AddressData, ruleSet: "Address");
                if (!addressResult.IsValid)
                {
                    Logger.Error("Address validation errors for organisation {0}", organisation.Name);
                    foreach (var error in addressResult.Errors)
                    {
                        Logger.Error(error.ErrorMessage);
                    }

                    hasErrors = true;
                }

                var organisationResult = OrganisationValidator.Validate(organisation, ruleSet: "Organisation");
                if (!organisationResult.IsValid)
                {
                    Logger.Error("Other organisation validation errors for organisation {0}", organisation.Name);
                    foreach (var error in organisationResult.Errors)
                    {
                        Logger.Error(error.ErrorMessage);
                    }

                    hasErrors = true;
                }
            }

            return hasErrors;
        }
    }
}
