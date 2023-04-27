﻿using System;
using System.Linq;

namespace Sitemplate.TagProcessors
{
    class BaseProcessor
    {
        public const string TagName = "";

        public virtual Tuple<string, bool> Process(string content, TagInfo tag, TemplateContext context)
        {
            return new Tuple<string, bool>(content, false);
        }

        protected void PushParameters(TemplateContext tagContext, TagInfo tag)
        {
            foreach (var par in tag.Parameters)
            {
                if (par.Key.StartsWith(Constants.VariablePrefix))
                {
                    tagContext.Variables[par.Key] = par.Value;
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