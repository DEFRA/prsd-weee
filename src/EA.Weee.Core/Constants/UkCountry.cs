namespace EA.Weee.Core.Constants
{
    using System;
    using System.Collections.Generic;

    public static class UkCountry
    {
        public static class Ids
        {
            public static readonly Guid England = Guid.Parse("184E1785-26B4-4AE4-80D3-AE319B103ACB");
            public static readonly Guid Wales = Guid.Parse("DB83F5AB-E745-49CF-B2CA-23FE391B67A8");
            public static readonly Guid Scotland = Guid.Parse("4209EE95-0882-42F2-9A5D-355B4D89EF30");
            public static readonly Guid NorthernIreland = Guid.Parse("7BFB8717-4226-40F3-BC51-B16FDF42550C");
        }

        public static readonly HashSet<Guid> ValidIds = new HashSet<Guid>
        {
            Ids.England,
            Ids.Wales,
            Ids.Scotland,
            Ids.NorthernIreland
        };

        private static readonly Dictionary<string, Guid> NameToIdMapping = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase)
        {
            { "England", Ids.England },
            { "Wales", Ids.Wales },
            { "Scotland", Ids.Scotland },
            { "Northern Ireland", Ids.NorthernIreland },
            { "Great Britain", Ids.England },
            { "United Kingdom", Ids.England },
            { "Not specified", Guid.Empty }
        };

        public static Guid GetIdByName(string countryName)
        {
            if (string.IsNullOrWhiteSpace(countryName))
            {
                return Guid.Empty;
            }

            if (NameToIdMapping.TryGetValue(countryName, out var guid))
            {
                return guid;
            }

            throw new ArgumentException($"No matching GUID found for country: {countryName}");
        }

        public static bool IsValidId(Guid id)
        {
            return ValidIds.Contains(id);
        }
    }
}
