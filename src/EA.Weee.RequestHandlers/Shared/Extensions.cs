namespace EA.Weee.RequestHandlers.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Helpers;
    using Domain;
    using Domain.DataReturns;
    using Domain.Scheme;
    using Weee.XmlValidation.Errors;

    public static class Extensions
    {
        public static MemberUploadError ToMemberUploadError(this XmlValidationError error)
        {
            return new MemberUploadError(error.ErrorLevel.ToDomainEnumeration<ErrorLevel>(),
                error.ErrorType.ToUploadErrorType(), error.Message,
                error.LineNumber.HasValue ? error.LineNumber.Value : 0);
        }

        public static DataReturnUploadError ToDataReturnsUploadError(this XmlValidationError error)
        {
            return new DataReturnUploadError(error.ErrorLevel.ToDomainEnumeration<ErrorLevel>(),
                error.ErrorType.ToUploadErrorType(), error.Message,
                error.LineNumber.HasValue ? error.LineNumber.Value : 0);
        }

        public static UploadErrorType ToUploadErrorType(this XmlErrorType errorType)
        {
            switch (errorType)
            {
                case XmlErrorType.Business:
                    return UploadErrorType.Business;
                case XmlErrorType.Schema:
                    return UploadErrorType.Schema;
                default:
                    throw new NotImplementedException(string.Format("Unknown error type '{0}'", errorType));
            }
        }

        public static ErrorLevel ToDomainErrorLevel(this Core.Shared.ErrorLevel errorLevel)
        {
            ErrorLevel result = null;

            switch (errorLevel)
            {
                case Core.Shared.ErrorLevel.Trace:
                    result = ErrorLevel.Trace;
                    break;
                case Core.Shared.ErrorLevel.Debug:
                    result = ErrorLevel.Debug;
                    break;
                case Core.Shared.ErrorLevel.Info:
                    result = ErrorLevel.Info;
                    break;
                case Core.Shared.ErrorLevel.Warning:
                    result = ErrorLevel.Warning;
                    break;
                case Core.Shared.ErrorLevel.Error:
                    result = ErrorLevel.Error;
                    break;
                case Core.Shared.ErrorLevel.Fatal:
                    result = ErrorLevel.Fatal;
                    break;
            }

            return result;
        }

        /// <summary>
        /// An implementation of the Fisher-Yates shuffle algorithm.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IList<TSource> Shuffle<TSource>(this IList<TSource> list)
        {
            Random r = new Random();

            List<TSource> results = new List<TSource>();
            List<int> indexes = Enumerable.Range(0, list.Count).ToList();

            while (indexes.Count > 0)
            {
                int n = r.Next(indexes.Count);
                int index = indexes[n];
                indexes.Remove(index);

                results.Add(list[index]);
            }

            return results;
        }

        public static Core.Shared.ObligationType ToCoreObligationType(this Xml.DataReturns.obligationTypeType obligationType)
        {
            switch (obligationType)
            {
                case Xml.DataReturns.obligationTypeType.B2B:
                    return Core.Shared.ObligationType.B2B;
                case Xml.DataReturns.obligationTypeType.B2C:
                    return Core.Shared.ObligationType.B2C;
                default:
                    throw new ArgumentException("Unable to map between obligation types");
            }
        }

        public static ObligationType ToDomainObligationType(this Xml.DataReturns.obligationTypeType obligationType)
        {
            switch (obligationType)
            {
                case Xml.DataReturns.obligationTypeType.B2B:
                    return ObligationType.B2B;
                case Xml.DataReturns.obligationTypeType.B2C:
                    return ObligationType.B2C;
                default:
                    throw new ArgumentException("Unable to map between obligation types");
            }
        }

        public static Domain.Lookup.WeeeCategory ToDomainWeeeCategory(this Xml.DataReturns.categoryNameType category)
        {
            switch (category)
            {
                case Xml.DataReturns.categoryNameType.LargeHouseholdAppliances:
                    return Domain.Lookup.WeeeCategory.LargeHouseholdAppliances;
                case Xml.DataReturns.categoryNameType.SmallHouseholdAppliances:
                    return Domain.Lookup.WeeeCategory.SmallHouseholdAppliances;
                case Xml.DataReturns.categoryNameType.ITandTelecommsEquipment:
                    return Domain.Lookup.WeeeCategory.ITAndTelecommsEquipment;
                case Xml.DataReturns.categoryNameType.ConsumerEquipment:
                    return Domain.Lookup.WeeeCategory.ConsumerEquipment;
                case Xml.DataReturns.categoryNameType.LightingEquipment:
                    return Domain.Lookup.WeeeCategory.LightingEquipment;
                case Xml.DataReturns.categoryNameType.ElectricalandElectronicTools:
                    return Domain.Lookup.WeeeCategory.ElectricalAndElectronicTools;
                case Xml.DataReturns.categoryNameType.ToysLeisureandSports:
                    return Domain.Lookup.WeeeCategory.ToysLeisureAndSports;
                case Xml.DataReturns.categoryNameType.MedicalDevices:
                    return Domain.Lookup.WeeeCategory.MedicalDevices;
                case Xml.DataReturns.categoryNameType.MonitoringandControlInstruments:
                    return Domain.Lookup.WeeeCategory.MonitoringAndControlInstruments;
                case Xml.DataReturns.categoryNameType.AutomaticDispensers:
                    return Domain.Lookup.WeeeCategory.AutomaticDispensers;
                case Xml.DataReturns.categoryNameType.DisplayEquipment:
                    return Domain.Lookup.WeeeCategory.DisplayEquipment;
                case Xml.DataReturns.categoryNameType.CoolingAppliancesContainingRefrigerants:
                    return Domain.Lookup.WeeeCategory.CoolingApplicancesContainingRefrigerants;
                case Xml.DataReturns.categoryNameType.GasDischargeLampsandLEDlightsources:
                    return Domain.Lookup.WeeeCategory.GasDischargeLampsAndLedLightSources;
                case Xml.DataReturns.categoryNameType.PhotovoltaicPanels:
                    return Domain.Lookup.WeeeCategory.PhotovoltaicPanels;
                default:
                    throw new ArgumentException("Unable to map between category types");
            }
        }
    }
}
