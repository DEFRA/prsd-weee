namespace EA.Weee.Core.Scheme.MemberUploadTesting
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A producer that has been created for the purposes of writing to
    /// an XML file that will be used for testing the PCS member upload
    /// functionality.
    /// </summary>
    public class Producer
    {
        public ProducerStatus Status { get; set; }
        public string RegistrationNumber { get; set; }
        public string TradingName { get; set; }
        public List<string> SICCodes { get; private set; }
        public bool VATRegistered { get; set; }
        public decimal AnnualTurnover { get; set; }
        public AnnualTurnoverBand AnnualTurnoverBand { get; set; }
        public EEEPlacedOnMarketBand EEEPlacedOnMarketBand { get; set; }
        public ObligationType ObligationType { get; set; }
        public List<string> BrandNames { get; private set; }
        public ProducerBusiness ProducerBusiness { get; private set; }
        public AuthorizedRepresentative AuthorizedRepresentative { get; private set; }
        public DateTime? CeasedToExistDate { get; set; }
        public SellingTechnique SellingTechnique { get; set; }

        public Producer()
        {
            SICCodes = new List<string>();
            BrandNames = new List<string>();
            ProducerBusiness = new ProducerBusiness();
        }
        
        public static Producer Create(ProducerSettings settings, bool noCompany)
        {
            Producer producer = new Producer();

            if (settings.IsNew)
            {
                producer.Status = ProducerStatus.Insert;
            }
            else
            {
                producer.Status = ProducerStatus.Amend;
                producer.RegistrationNumber = settings.RegistrationNumber;
            }

            if (!settings.IgnoreStringLengthConditions)
            {
                producer.TradingName = RandomHelper.CreateRandomString("Trading Name ", 1, 255);
            }
            else
            {
                producer.TradingName = RandomHelper.CreateRandomString("Trading Name ", 0, 1000);
            }

            int numberOfSICCodes = RandomHelper.R.Next(1, 5);
            for (int index = 0; index < numberOfSICCodes; ++index)
            {
                if (!settings.IgnoreStringLengthConditions)
                {
                    producer.SICCodes.Add(RandomHelper.CreateRandomString("SIC", 8, 8, false));
                }
                else
                {
                    producer.SICCodes.Add(RandomHelper.CreateRandomString("SIC", 0, 1000, false));
                }
            }

            producer.VATRegistered = RandomHelper.OneIn(2);

            decimal annualTurnover = (decimal)RandomHelper.R.NextDouble() * 2000000;
            producer.AnnualTurnover = annualTurnover;

            producer.AnnualTurnoverBand = (annualTurnover > 1000000)
                ? AnnualTurnoverBand.GreaterThanOneMillionPounds
                : AnnualTurnoverBand.LessThanOrEqualToOneMillionPounds;

            producer.EEEPlacedOnMarketBand = RandomHelper.ChooseEnum<EEEPlacedOnMarketBand>();

            producer.ObligationType = RandomHelper.ChooseEnum<ObligationType>();

            int numberOfBrandNames = RandomHelper.R.Next(1, 5);
            for (int index = 0; index < numberOfBrandNames; ++index)
            {
                if (!settings.IgnoreStringLengthConditions)
                {
                    producer.BrandNames.Add(RandomHelper.CreateRandomString("Brand ", 1, 10)); //255?
                }
                else
                {
                    producer.BrandNames.Add(RandomHelper.CreateRandomString("Brand ", 0, 1000)); //255?
                }
            }

            producer.ProducerBusiness = ProducerBusiness.Create(settings, noCompany);

            if (settings.SchemaVersion < SchemaVersion.Version_3_07 || RandomHelper.OneIn(2))
            {
                producer.AuthorizedRepresentative = AuthorizedRepresentative.Create(settings);
            }

            if (RandomHelper.OneIn(2))
            {
                producer.CeasedToExistDate = DateTime.Now.AddDays(RandomHelper.R.Next(1000));
            }

            producer.SellingTechnique = RandomHelper.ChooseEnum<SellingTechnique>();

            return producer;
        }
    }
}
