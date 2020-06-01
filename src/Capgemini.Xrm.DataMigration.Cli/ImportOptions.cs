using CommandLine;

namespace Capgemini.Xrm.DataMigration.Cli
{
    [Verb("import", HelpText = "Import data into a Dynamics 365 instance.")]
    public class ImportOptions
    {
        [Option('c', "connectionstring", HelpText = "The connection string used to connect to Dynamics 365.", Required = true)]
        public string ConnectionString { get; set; }

        [Option("config", HelpText = "Path to an import configuration file.", Required = true)]
        public string ConfigurationFile { get; set; }
    }
}