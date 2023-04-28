using System;
using System.Linq;

namespace Sitemplate.TagProcessors
{
    abstract class BaseProcessor
    {
        public abstract Tuple<string, bool> Process(string content, TagInfo tag, TemplateContext context);

        protected void PushParameters(TemplateContext tagContext, TagInfo tag)
        {
            foreach (var par in tag.Parameters)
            {
                if (par.Key.StartsWith(Constants.VariablePrefix))
                {
                    tagContext.Variables[par.Key] = tagContext.processor.EvaluateValue(par.Value, tagContext);
                }
            }
        }

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
