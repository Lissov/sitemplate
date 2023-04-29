using System;
using System.Linq;

namespace Sitemplate.Processors
{
    abstract class BaseProcessor<T, U>
    {
        public abstract U Process(string content, T parameter, TemplateContext context);

        protected string GetIndentation(string content, int start)
        {
            var lineStart = content.LastIndexOf('\n', start);
            var spaces = content.Substring(lineStart + 1, start - lineStart - 1);
            if (string.IsNullOrWhiteSpace(spaces))
                return spaces ?? "";
            return "";
        }

        protected string RemoveFirstLineIndentation(string content)
        {
            var ind = 0;
            while (ind < content.Length && (content[ind] == ' ' || content[ind] == '\t')) ind++;
            var indentation = content.Substring(0, ind);
            return string.Join('\n',
                    content
                        .Split('\n')
                        .Select(x => x.StartsWith(indentation) ? x.Substring(ind) : x)
                );
        }

        protected string InjectIndentation(string template, string indentation)
        {
            var lines = template.Split('\n');
            if (lines.Length <= 1)
                return template;

            var indented = lines.Take(1).ToList();
            indented.AddRange(lines.Skip(1).Select(l => indentation + l));
            return string.Join('\n', indented);
        }
    }
}
