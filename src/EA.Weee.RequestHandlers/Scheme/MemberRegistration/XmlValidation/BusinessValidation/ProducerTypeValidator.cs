namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using DataAccess;
    using Domain;
    using Extensions;
    using FluentValidation;

    public class ProducerTypeValidator : AbstractValidator<producerType>
    {
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "It's the UK, silly.")]
        private readonly IEnumerable<countryType> ukCountries = new List<countryType>
        {
            countryType.UKENGLAND, countryType.UKNORTHERNIRELAND, 
            countryType.UKSCOTLAND, countryType.UKWALES
        };

        public ProducerTypeValidator(WeeeContext context)
        {
            RuleSet(
                BusinessValidator.RegistrationNoRuleSet,
                () =>
                    {
                        RuleFor(pt => pt.registrationNo)
                            .NotEmpty()
                            .When(pt => pt.status == statusType.A)
                            .WithState(pt => ErrorLevel.Error)
                            .WithMessage(
                                "The producer registration number for '{0}' has been left out of the xml file but the xml file is amending existing producer details. Check this producer's details. To amend this producer add the producer registration number or to add as a brand new producer use status \"I\" not \"A\".",
                                pt => pt.tradingName);

                        RuleFor(pt => pt.registrationNo)
                            .Must((pt, registrationNo) => string.IsNullOrEmpty(registrationNo))
                            .When(pt => pt.status == statusType.I)
                            .WithState(pt => ErrorLevel.Error)
                            .WithMessage(
                                "A producer registration number for: '{0}' has been entered in the xml file but you are trying to register this producer for the very first time. Check this producer's details. To add this as a new producer remove the producer registration number or to amend  an existing producer details use status \"A\" not \"I\".",
                                pt => pt.tradingName);
                    });

            RuleSet(
                BusinessValidator.AuthorisedRepresentativeMustBeInUkRuleset,
                () =>
                    {
                        RuleFor(pt => pt.producerBusiness.Item).Must(
                            (pt, producerBusinessItem) =>
                                {
                                    contactDetailsContainerType officeContactDetails;

                                    if (producerBusinessItem.GetType() == typeof(companyType))
                                    {
                                        officeContactDetails = ((companyType)producerBusinessItem).registeredOffice;
                                    }
                                    else if (producerBusinessItem.GetType() == typeof(partnershipType))
                                    {
                                        officeContactDetails =
                                            ((partnershipType)producerBusinessItem).principalPlaceOfBusiness;
                                    }
                                    else
                                    {
                                        throw new ArgumentException(string.Format("{0}: producerBusinessItem must be of type companyType or partnershipType", pt.tradingName));
                                    }

                                    // abusing law of demeter here, but schema requires all these fields to be present and correct
                                    return ukCountries.Contains(officeContactDetails.contactDetails.address.country);
                                })
                            .When(pt => pt.authorisedRepresentative.overseasProducer != null)
                            .WithState(pt => ErrorLevel.Error)
                            .WithMessage(
                                "{0} is an authorised representative but has a country in their address which is outside of the UK. An authorised representative must be based in the UK. In order to register or amend this producer please check they are an authorised representative and are based in the UK.",
                                (pt, item) => pt.tradingName);
                    });

            RuleSet(BusinessValidator.DataValidationRuleSet,
                () =>
                {
                    var validProducerRegistrationNumbers = context.Producers
                        .Select(p => p.RegistrationNumber)
                        .Concat(context.MigratedProducers.Select(mp => mp.ProducerRegistrationNumber))
                        .ToList() // Cannot perform string operations until EntityFramework enumerable is transfered to list
                        .Select(prn => prn.ToLowerInvariant());

                    RuleFor(p => p.registrationNo)
                        .Must((p, prn) => validProducerRegistrationNumbers.Contains(prn.ToLowerInvariant()))
                        .When(p => p.status == statusType.A && !string.IsNullOrEmpty(p.registrationNo))
                        .WithState(p => ErrorLevel.Error)
                        .WithMessage(
                            "{0} {1} has a producer registration number in the xml which is not recognised. In order to register or amend this producer please enter the correct producer registration number for the producer.",
                            (p, prn) => p.GetProducerName(),
                            (p, prn) => prn);
                });
        }
    }
}
