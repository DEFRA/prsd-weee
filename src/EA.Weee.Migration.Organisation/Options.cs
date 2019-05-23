namespace EA.Weee.Migration.Organisation
{
    using CommandLine;
    using CommandLine.Text;

    internal class Options
    {
        [Option('r', "read", Required = true,
            HelpText = "Input file to be processed.")]
        public string InputFile { get; set; }

        [Option('m', "mode", DefaultValue = Mode.LocalValidation,
            HelpText = "Mode to run the application in. Allowed values: LocalValidation, DatabaseValidation, DataMigration")]
        public Mode Mode { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
                current => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
