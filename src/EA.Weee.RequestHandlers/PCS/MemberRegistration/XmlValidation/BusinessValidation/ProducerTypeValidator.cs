﻿namespace EA.Weee.RequestHandlers.PCS.MemberRegistration.XmlValidation.BusinessValidation
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Domain;
    using FluentValidation;

    public class ProducerTypeValidator : AbstractValidator<producerType>
    {
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "It's the UK, silly.")]
        private readonly IEnumerable<countryType> ukCountries = new List<countryType>
        {
            countryType.UKENGLAND, countryType.UKNORTHERNIRELAND, 
            countryType.UKSCOTLAND, countryType.UKWALES
        };

        public ProducerTypeValidator()
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
                                        return false; // TODO: throw exception instead? how will that affect validation?
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
        }
    }
}
