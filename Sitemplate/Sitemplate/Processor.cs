using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    Console.Error.WriteLine("Directories not yet supported");
                    //throw new Exception("Directories not yet supported");
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
            var updated = ProcessContent(content, new TemplateContext());

            if (string.IsNullOrEmpty(updated))
                return;

            // write to output
            WriteToOutput(item, updated);
        }

        private string ProcessContent(string content, TemplateContext context)
        {
            content = ProcessVariables(content, context);
            content = InjectTemplates(content, context);
            return content;
        }

        private string DeclareVariables(string content, TemplateContext context)
        {
            throw new NotImplementedException();
        }

        private string ProcessVariables(string content, TemplateContext context)
        {
            var proceed = true;
            while (proceed)
            {
                content = ProcessVariablesStep(content, context, out proceed);
            }
            return content;
        }
        private string ProcessVariablesStep(string content, TemplateContext context, out bool replaced)
        {
            replaced = false;

            bool b;
            content = ProcessIfs(content, context, out b);
            replaced |= b;
            content = ProcessDeclares(content, context, out b);
            replaced |= b;
            content = ProcessSets(content, context, out b);
            replaced |= b;

            // variables can contain other variables, so need to iterate till all replaced
            foreach (var var in context.Variables)
            {
                var varToReplace = var.Key;
                if (content.Contains(varToReplace))
                {
                    var value = EvaluateVariable(var.Value, context);
                    content = content.Replace(varToReplace, (value ?? "").ToString());
                    replaced = true;
                }
            }
            return content;
        }

        private string ProcessSets(string content, TemplateContext context, out bool processed)
        {
            var tagname = Constants.Tag.Set;
            var parser = new TagParser();
            var tag = parser.FindFirstTag(content, tagname);
            processed = tag != null;
            while (tag != null)
            {
                if (tag.Parameters.Length != 1)
                    throw new Exception("'set' must have one parameter");
                context.Variables[tag.Parameters[0].Key] = ProcessContent(tag.TagInside, context);
                content = ReplaceInContent(content, tag, "");

                tag = parser.FindFirstTag(content, tagname);
            }
            return content;
        }

        private string ProcessIfs(string content, TemplateContext context, out bool processed)
        {
            var tagname = Constants.Tag.If;
            var parser = new TagParser();
            var tag = parser.FindFirstTag(content, tagname);
            processed = tag != null;
            while (tag != null)
            {
                if (tag.Parameters.Length != 1)
                    throw new Exception("'if' requires single parameter: " + tag.TagContent);
                var processedKey = ProcessContent(tag.Parameters[0].Key, context);
                var processedValue = tag.Parameters[0].Value != null ? ProcessContent(tag.Parameters[0].Value, context) : null;
                var ifelse = tag.TagInside.Split(Constants.Tag.Else);
                if (processedKey == processedValue)
                    content = ReplaceInContent(content, tag, ifelse.Length > 0 ? ifelse[0] : "");
                else
                    content = ReplaceInContent(content, tag, ifelse.Length > 1 ? ifelse[1] : "");

                tag = parser.FindFirstTag(content, tagname);
            }
            return content;
        }


        private string ProcessDeclares(string content, TemplateContext context, out bool processed)
        {
            var tagname = Constants.Tag.Declare;
            var parser = new TagParser();
            var tag = parser.FindFirstTag(content, tagname);
            processed = tag != null;
            while (tag != null)
            {
                foreach (var par in tag.Parameters)
                {
                    context.Variables[par.Key] = par.Value;
                }
                content = ReplaceInContent(content, tag, "");
                tag = parser.FindFirstTag(content, tagname);
            }
            return content;
        }

        private object EvaluateVariable(object value, TemplateContext context)
        {
            // For now be simple
            return value;
        }

        private string InjectTemplates(string content, TemplateContext context)
        {
            var tagname = Constants.Tag.Inject;
            var parser = new TagParser();
            var tag = parser.FindFirstTag(content, tagname);

            if (tag == null)
                return content;

            if (tag.Parameters.Length < 1 || tag.Parameters[0].Value != null)
                throw new Exception($"Tag '{tagname}' must have first parameter as template name");
            var templatename = tag.Parameters[0].Key;
            if (!templates.ContainsKey(templatename))
                throw new Exception($"Template not found: '{templatename}'.");
            var tagContext = context.Clone();
            PushParameters(tagContext, tag);

            var template = templates[templatename];
            template = ProcessContent(template, tagContext);
            var indentation = GetIndentation(content, tag.Start);
            template = InjectIndentation(template, indentation);

            content = ReplaceInContent(content, tag, template);
            Console.WriteLine($"\tInjected template: {templatename}");

            return InjectTemplates(content, tagContext);
        }

        private static string ReplaceInContent(string content, TagInfo tag, string injectContent)
        {
            return content.Substring(0, tag.Start) + injectContent + content.Substring(tag.End);
        }

        private void PushParameters(TemplateContext tagContext, TagInfo tag)
        {
            foreach (var par in tag.Parameters)
            {
                if (par.Key.StartsWith(Constants.VariablePrefix))
                {
                    tagContext.Variables[par.Key] = par.Value;
                }
            }
        }

        private string InjectIndentation(string template, object indentation)
        {
            var lines = template.Split('\n');
            if (lines.Length <= 1)
                return template;

            var indented = lines.Take(1).Union(lines.Skip(1).Select(l => indentation + l));
            return string.Join('\n', indented);
        }

        private object GetIndentation(string content, int start)
        {
            var lineStart = content.LastIndexOf('\n', start);
            var spaces = content.Substring(lineStart + 1, start - lineStart - 1);
            if (string.IsNullOrWhiteSpace(spaces))
                return spaces ?? "";
            return "";
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
