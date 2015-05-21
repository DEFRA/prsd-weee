namespace EA.Weee.DocumentGeneration
{
    using System;
    using DocumentFormat.OpenXml.Wordprocessing;

    public class MergeField
    {
        public Run Run { get; private set; }

        public MergeFieldName FieldName { get; private set; }

        public MergeFieldType FieldType { get; set; }

        public MergeField(Run run, string fieldName)
        {
            this.Run = run;
            this.FieldName = new MergeFieldName(fieldName);

            if (this.FieldName.InnerTypeName.StartsWith("Is"))
            {
                this.FieldType = MergeFieldType.Checkbox;
            }
            else
            {
                this.FieldType = MergeFieldType.Text;
            }
        }

        public void SetText(string text, int numberOfLineBreaks = 1)
        {
            string[] lines = { text };

            if (text.Contains(Environment.NewLine))
            {
                lines = text.Split(new[] { Environment.NewLine },
                    StringSplitOptions.RemoveEmptyEntries);
            }

            for (int i = 0; i < lines.Length; i++)
            {
                this.Run.AppendChild(new Text(lines[i]));

                if (i < lines.Length - 1)
                {
                    for (int j = 0; j < numberOfLineBreaks; j++)
                    {
                        this.Run.Append(new Break());
                    }
                }
            }
        }

        public static readonly char StartMergeField = '«';
        public static readonly char EndMergeField = '»';

        public void RemoveCurrentContents()
        {
            this.Run.RemoveAllChildren<Text>();
            this.Run.RemoveAllChildren<MailMerge>();
        }
    }
}
