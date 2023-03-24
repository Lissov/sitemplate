using Newtonsoft.Json;
using System;
using System.IO;

namespace Sitemplate
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                    throw new ArithmeticException("Must specify parameter: path to project desctiptor file");

                Console.WriteLine("Working directory: " + Directory.GetCurrentDirectory());
                var projectfile = args[0];
                if (!File.Exists(projectfile))
                    throw new ArithmeticException("File not found: " + projectfile);

                var action = args.Length > 1 ? args[1] : "build";
                Console.WriteLine("Action: " + action);

                var content = File.ReadAllText(projectfile);
                var descriptor = JsonConvert.DeserializeObject<Descriptor>(content);
                Console.WriteLine("Descriptor loaded. Project name: " + descriptor.Name);

                var processor = new Processor(descriptor);
                processor.Process(action);
            } catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }
    }
}
