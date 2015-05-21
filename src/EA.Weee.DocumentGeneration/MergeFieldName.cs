namespace EA.Weee.DocumentGeneration
{
    using System;

    public class MergeFieldName
    {
        public string OuterTypeName { get; private set; }

        public string InnerTypeName { get; private set; }

        public MergeFieldName(string name)
        {
            if (name.Contains("[")
            && name.Contains("]"))
            {
                int subTypeStart = name.IndexOf("[", StringComparison.InvariantCultureIgnoreCase);

                this.OuterTypeName = name.Substring(0, subTypeStart);
                this.InnerTypeName = name.Substring(subTypeStart + 1, name.Length - subTypeStart - 2);
            }
            else
            {
                this.InnerTypeName = name;
            }
        }

        public override string ToString()
        {
            return OuterTypeName + "[" + InnerTypeName + "]";
        }
    }
}
