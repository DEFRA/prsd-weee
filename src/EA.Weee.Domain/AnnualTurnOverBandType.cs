namespace EA.Weee.Domain
{
    using EA.Prsd.Core.Domain;

    public class AnnualTurnOverBandType : Enumeration
    {
        public static readonly AnnualTurnOverBandType Lessthanorequaltoonemillionpounds = new AnnualTurnOverBandType(0, "Less than or equal to one million pounds");
        public static readonly AnnualTurnOverBandType Greaterthanonemillionpounds = new AnnualTurnOverBandType(1, "Greater than one million pounds");
     
        protected AnnualTurnOverBandType()
        {
        }

        private AnnualTurnOverBandType(int value, string displayName)
            : base(value, displayName)
        {
        }
    }
}