using Sitemplate.Processors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sitemplate
{
    public class Processor
    {
        private Descriptor descriptor;
        private Dictionary<string, string> templates = new Dictionary<string, string>();

        public Processor(Descriptor descriptor)
        {
           this.descriptor = descriptor;
        }
        internal void Process(string action)
        {
            switch (action)
            {
                case "clean":
                    Clean();
                    return;
                case "build":
                    Clean();
                    ReadTemplates();
                    Build();
                    return;
                default:
                    throw new ArgumentException("Action not supported: " + action);
            }
        }

        private void ReadTemplates()
        {
            foreach (var tfolder in descriptor.Templates)
            {
                var tfiles = Enumerate(tfolder, new[] { "*" }, enumerateSubDirectories: true);

                foreach (var tfile in tfiles)
                {
                    if (Directory.Exists(tfile))
                        throw new Exception("Should not get directory here");
                    ReadTemplate(tfolder, tfile);
                }
            }
        }

        private void ReadTemplate(string folder, string tfile)
        {
            var template = File.ReadAllText(Path.Combine(folder, tfile));
            var tname = GetTemplateName(tfile);
            templates.Add(tname, template);
            Console.WriteLine("Template added: " + tname);
        }

        private string GetTemplateName(string tfile)
        {
            var ext = Path.GetExtension(tfile);
            var tname = string.IsNullOrEmpty(ext)
                ? tfile
                : tfile.Substring(0, tfile.Length - ext.Length);
            return tname.Replace("\\", ".").Replace("/", ".");
        }

        private void Build()
        {
            var includes = Enumerate(descriptor.InputFolder, descriptor.Include, enumerateSubDirectories: true);

            foreach (var item in includes)
            {
                if (Directory.Exists(item))
                {
                    //Console.Error.WriteLine("Directories not yet supported");
                    throw new Exception("Directories not yet supported");
                } else
                {
                    ProcessFile(item);
                }
            }
        }

        private void ProcessFile(string item)
        {
            Console.WriteLine("Processing file: " + item);
            var content = File.ReadAllText(Path.Combine(descriptor.InputFolder, item));

            // process
            var textProcessor = new TextProcessor()
            {
                Templates = templates
            };
            var updated = textProcessor.ProcessContent(content, new TemplateContext(textProcessor));

            if (string.IsNullOrEmpty(updated))
                return;

            // write to output
            WriteToOutput(item, updated);
        }

        private void WriteToOutput(string filePath, string content)
        {
            var path = Path.Combine(descriptor.OutputFolder, filePath);

            var directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            Console.WriteLine("\tWriting file: " + path);
            File.WriteAllText(path, content);
        }

        private void Clean()
        {
            Console.WriteLine("Cleaning...");

            var output = descriptor.OutputFolder ?? ".";

            var skip = Enumerate(output, descriptor.IgnoreInOutput).ToList();
            var locations = Enumerate(output, descriptor.Include)
                .Union(Enumerate(output, descriptor.Clean))
                .Where(l => !skip.Contains(l));

            foreach (var location in locations)
            {
                var path = Path.Combine(descriptor.OutputFolder, location);
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, recursive: true);
                    Console.Error.WriteLine("Directory deleted: " + location);
                } else
                {
                    File.Delete(path);
                    Console.WriteLine("File deleted: " + location);
                }
            }            
        }

        private List<string> Enumerate(string folder, string[] masks, bool enumerateSubDirectories = false)
        {
            var currentDir = Directory.GetCurrentDirectory();

            try
            {
                Directory.SetCurrentDirectory(folder);
                if (masks == null)
                {
                    return new List<string>();
                }

                var all = masks.SelectMany(m =>
                {
                    var directories = Directory.EnumerateDirectories(".", m);
                    var files = Directory.EnumerateFiles(".", m);
                    if (!enumerateSubDirectories) return directories.Union(files);
                    return directories.SelectMany(d => EnumerateSubfolders(d))
                        .Union(files);
                });

                return all
                    .Select(v => v.StartsWith(".\\") ? v.Substring(2) : v )
                    .ToList();
            } finally
            {
                Directory.SetCurrentDirectory(currentDir);
            }
        }

        private IEnumerable<string> EnumerateSubfolders(string folder)
        {
            return
                Directory
                    .EnumerateDirectories(folder)
                        .SelectMany(f => EnumerateSubfolders(f))
                    .Union(Directory.EnumerateFiles(folder));
        }
    }
}
