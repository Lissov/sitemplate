using System;
using System.Linq;

namespace Sitemplate.Processors
{
    abstract class BaseProcessor<T, U>
    {
        public abstract U Process(string content, T parameter, TemplateContext context);

        protected object GetIndentation(string content, int start)
        {
            var lineStart = content.LastIndexOf('\n', start);
            var spaces = content.Substring(lineStart + 1, start - lineStart - 1);
            if (string.IsNullOrWhiteSpace(spaces))
                return spaces ?? "";
            return "";
        }

        protected string InjectIndentation(string template, object indentation)
        {
            var lines = template.Split('\n');
            if (lines.Length <= 1)
                return template;

            var indented = lines.Take(1).Union(lines.Skip(1).Select(l => indentation + l));
            return string.Join('\n', indented);
        }
    }
}
