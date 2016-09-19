using System;
using System.IO;
using System.Linq;
using Wintellect.Extensions;

namespace SetEnvironmentVariablePath
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var environmentPath = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine);

                var currentFolderPath = new DirectoryInfo(".").FullName;
                string currentFolderPathWithTrailingSlash = null;

                if (currentFolderPath.EndsWith("\\"))
                {
                    currentFolderPathWithTrailingSlash = currentFolderPath;
                    currentFolderPath = currentFolderPath.Substring(0, currentFolderPath.Length - 1);
                }
                else
                {
                    currentFolderPathWithTrailingSlash = currentFolderPath.Plus("\\");
                }

                if (!environmentPath.Split(';').Any(p => p.Equals(currentFolderPath, StringComparison.InvariantCultureIgnoreCase) ||
                    p.Equals(currentFolderPathWithTrailingSlash, StringComparison.InvariantCultureIgnoreCase)))
                {
                    var newPath = environmentPath.Plus(";", currentFolderPathWithTrailingSlash);

                    Environment.SetEnvironmentVariable("PATH", newPath, EnvironmentVariableTarget.Machine);
                }

                Console.WriteLine("{0} path added to the environment variable PATH.\n\nPress any key to exit this program.", currentFolderPathWithTrailingSlash);
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}\n\nPress any key to exit this program.", ex.ToString());
            }

            Console.ReadKey();
        }
    }
}
