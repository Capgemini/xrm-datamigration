using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Capgemini.DataMigration.Core.Model;
using CsvHelper;

namespace Capgemini.DataMigration.Core.Helpers
{
    public static class ObfuscationLookupHelper
    {
        public static Dictionary<string, ObfuscationLookup> ObfuscationLookups { get; private set; } = new Dictionary<string, ObfuscationLookup>();

        /// <summary>
        /// Remove all lookups from memory.
        /// </summary>
        public static void CleanLookups()
        {
            ObfuscationLookups = new Dictionary<string, ObfuscationLookup>();
        }

        /// <summary>
        /// Load lookups from a Dictionary.
        /// </summary>
        public static void LoadLookups(Dictionary<string, ObfuscationLookup> lookups)
        {
            ObfuscationLookups = lookups;
        }

        /// <summary>
        /// Load all csv files into memory from the specified path.
        /// </summary>
        /// <param name="folderPath">The folder that contains the csv files to be used as lookups.</param>
        public static void LoadLookups(string folderPath)
        {
            Dictionary<string, ObfuscationLookup> lookups = new Dictionary<string, ObfuscationLookup>();

            foreach (string filePath in System.IO.Directory.GetFiles(folderPath, "*.csv", SearchOption.TopDirectoryOnly))
            {
                var file = new FileInfo(filePath);
                ObfuscationLookup f = ReadFromFile(filePath);

                lookups.Add(file.Name, f);
            }

            ObfuscationLookups = lookups;
        }

        public static ObfuscationLookup ReadFromFile(string fileName)
        {
            List<dynamic> records = new List<dynamic>();

            using (TextReader tr = File.OpenText(fileName))
            {
                using (var reader = new CsvReader(tr))
                {
                    records = reader.GetRecords<dynamic>().ToList();
                }
            }

            return new ObfuscationLookup(fileName, records);
        }
    }
}
