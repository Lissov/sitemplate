﻿using Sitemplate.TagProcessors;
using System;
using System.Collections.Generic;

namespace Sitemplate
{
    public class TextProcessor
    {
        public Dictionary<string, string> Templates = new Dictionary<string, string>();

        public string ProcessContent(string content, TemplateContext context)
        {
            var current = 0;
            var parser = new TagParser();
            while (current < content.Length)
            {
                current = GetStartingIndex(content, current);
                if (current == int.MaxValue) return content;
                var c = content[current];
                switch (c)
                {
                    case '$':
                        var varname = "$" + ReadLiteral(content, current);
                        
                        if (context.Variables.ContainsKey(varname))
                        {
                            var variable = context.Variables[varname];
                            var value = EvaluateVariable(variable, context);
                            content = ReplaceInPosition(content, current, current + varname.Length, (value ?? "").ToString());
                        } else {
                            current++;
                        }
                        break;
                    case '<':
                        var tagName = ReadLiteral(content, current);
                        var tag = parser.FindFirstTag(content, tagName, current);
                        var processor = TagFactory.GetProcessor(tagName);
                        var r = processor.Process(content, tag, context);
                        content = r.Item1;
                        if (!r.Item2) current++;
                        break;
                    default:
                        throw new Exception("Something failed");
                }
            }
            return content;
        }

        private int GetStartingIndex(string content, int current)
        {
            var tagStart = content.IndexOf('<', current);
            var varStart = content.IndexOf("$", current);
            return Math.Min(
                tagStart < 0 ? int.MaxValue : tagStart,
                varStart < 0 ? int.MaxValue : varStart);
        }

        private string ReadLiteral(string content, int tagStart)
        {
            var i = tagStart + 1;
            var tn = "";
            while (i < content.Length && (
                    (content[i] >= '0' && content[i] <= '9')
                    || (content[i] >= 'a' && content[i] <= 'z')
                    || (content[i] >= 'A' && content[i] <= 'Z')
                    )
                  )
            {
                tn += content[i];
                i++;
            }
            return tn;
        }

        public string ReplaceInContent(string content, TagInfo tag, string injectContent)
        {
            return ReplaceInPosition(content, tag.Start, tag.End, injectContent);
            return content.Substring(0, tag.Start) + injectContent + content.Substring(tag.End);
        }

        public string ReplaceInPosition(string content, int start, int end, string injectContent)
        {
            return content.Substring(0, start) + injectContent + content.Substring(end);
        }

        private object EvaluateVariable(object value, TemplateContext context)
        {
            // For now be simple
            return value;
        }
    }
}
