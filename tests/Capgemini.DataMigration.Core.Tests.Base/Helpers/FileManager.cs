using System.IO;

namespace Capgemini.DataMigration.Core.Tests.Base.Helpers
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public static class FileManager
    {
        public static void DeleteAllContentsFromFolder(string path)
        {
            if (Directory.Exists(path))
            {
                var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
                foreach (var item in files)
                {
                    DeleteFile(item);
                }
            }
        }

        public static void DeleteFile(string file)
        {
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }
    }
}