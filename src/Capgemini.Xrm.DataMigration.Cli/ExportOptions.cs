using CommandLine;

namespace Capgemini.Xrm.DataMigration.Cli
{
    [Verb("export", HelpText = "Export data from a Dynamics 365 instance.")]
    public class ExportOptions
    {
        [Option('c', "connectionstring", HelpText = "The connection string used to connect to Dynamics 365.", Required = true)]
        public string ConnectionString { get; set; }

        [Option("config", HelpText = "Path to an export configuration file.", Required = true)]
        public string ConfigurationFile { get; set; }
    }
}