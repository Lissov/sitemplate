using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitemplate
{
    public class TagParser
    {
        public TagInfo FindFirstTag(string content, string tag, int startIndex = 0)
        {
            var start = content.IndexOf("<" + tag, startIndex, StringComparison.InvariantCultureIgnoreCase);
            var nextch = start >= 0 ? content[start + ("<" + tag).Length] : '-';
            while (start >=0
                && nextch != ' ' && nextch != '\t' && nextch != '\r' && nextch != '\n')
            {
                start = content.IndexOf("<" + tag, start + 1, StringComparison.InvariantCultureIgnoreCase);
                nextch = start >= 0 ? content[start + ("<" + tag).Length] : '-';
            }

            if (start < 0)
                return null;

            int i = FindTagEnd(content, start);
            if (i == content.Length)
                throw new Exception($"Error in content: '{tag}' tag not closed.");

            var closeTag = "</" + tag + ">";
            var end = content.IndexOf(closeTag, i, StringComparison.InvariantCultureIgnoreCase);
            if (end < 0)
                throw new Exception($"Error in content: no closing '{tag}' tag.");

            var result = new TagInfo { Start = start, End = end + closeTag.Length };

            var tagContent = content.Substring(start + tag.Length + 1, i - start - tag.Length - 1);
            result.Parameters = ParseParameters(tagContent);

            result.TagContent = tagContent;
            result.TagInside = content.Substring(i + 1, end - i - 1);

            return result;
        }

        private enum ParseState
        {
            WaitingParameter,
            ReadingKey,
            WaitingParameterOrEqual,
            WaitingValue,
            ReadingValueSingleQuote,
            ReadingValueDoubleQuote
        }
        private TagParameter[] ParseParameters(string tagContent)
        {
            var result = new List<TagParameter>();
            var i = 0;
            TagParameter tp = null;
            string value = "";
            var state = ParseState.WaitingParameter;
            while (i < tagContent.Length)
            {
                var ch = tagContent[i];
                string unexpectedExc = $"Unexpected character [{ch}] at [{i}] in: {tagContent}";
                switch (state)
                {
                    case ParseState.WaitingParameter:
                    case ParseState.WaitingParameterOrEqual:
                        switch (ch)
                        {
                            case ' ':
                            case '\t':
                            case '\r':
                            case '\n':
                                break;
                            case '=':
                                if (state == ParseState.WaitingParameter)
                                    throw new Exception("Err11:" + unexpectedExc);
                                state = ParseState.WaitingValue;
                                break;
                            default:
                                if (tp != null)
                                {
                                    result.Add(tp);
                                    tp = null;
                                }
                                value = "" + ch;
                                state = ParseState.ReadingKey;
                                break;
                        }
                        break;
                    case ParseState.ReadingKey:
                        switch (ch)
                        {
                            case ' ':
                            case '\t':
                            case '\r':
                            case '\n':
                            case '=':
                                state = ch == '='
                                    ? ParseState.WaitingValue
                                    : ParseState.WaitingParameterOrEqual;
                                tp = new TagParameter { Key = value };
                                break;
                            case '"':
                            case '\'':
                                throw new Exception("Err21:" + unexpectedExc);
                            default:
                                value += ch;
                                break;
                        }
                        break;
                    case ParseState.WaitingValue:
                        switch (ch)
                        {
                            case ' ':
                            case '\t':
                            case '\r':
                            case '\n':
                                break;
                            case '"':
                                state = ParseState.ReadingValueDoubleQuote;
                                value = "";
                                break;
                            case '\'':
                                state = ParseState.ReadingValueSingleQuote;
                                value = "";
                                break;
                            default: throw new Exception(unexpectedExc);
                        }
                        break;
                    case ParseState.ReadingValueSingleQuote:
                        switch (ch)
                        {
                            case '\'':
                                tp.Value = value;
                                state = ParseState.WaitingParameter;
                                break;
                            default:
                                value += ch;
                                break;
                        }
                        break;
                    case ParseState.ReadingValueDoubleQuote:
                        switch (ch)
                        {
                            case '"':
                                tp.Value = value;
                                state = ParseState.WaitingParameter;
                                break;
                            default:
                                value += ch;
                                break;
                        }
                        break;
                }

                i++;
            }

            if (state == ParseState.WaitingValue || state == ParseState.ReadingValueSingleQuote || state == ParseState.ReadingValueDoubleQuote)
                throw new Exception($"Err91: finished in unexpected state [{state}], tag content: " + tagContent);

            if (state == ParseState.ReadingKey)
            {
                tp = new TagParameter { Key = value };
            }
            if (tp != null)
                result.Add(tp);

            return result.ToArray();

            /*var paramsSplitted = SplitParameters(tagContent);
            var result = new List<TagParameter>();

            var i = 0;
            while (i < paramsSplitted.Length)
            {
                if (paramsSplitted[i] == "=")
                    throw new Exception("Unexpected '=' in tag parameters.");
                var tp = new TagParameter { Key = paramsSplitted[i] };
                i++;
                if (i < paramsSplitted.Length && paramsSplitted[i] == "=")
                {
                    if (i + 1 == paramsSplitted.Length)
                        throw new Exception("Missing parameter value tag parameters.");
                    tp.Value = paramsSplitted[i + 1];
                    i += 2;
                }
                result.Add(tp);
            }
            return result.ToArray();*/
        }

        private string[] SplitParameters(string tagContent)
        {
            var parSplit = tagContent.Split(new char[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            var allSplit = parSplit.SelectMany(p =>
            {
                var eq = p.IndexOf('=');
                if (eq == -1 || p == "=")
                    return new[] { p };
                if (eq == 0)
                    return new[] { "=", p.Substring(1) };
                if (eq == p.Length - 1)
                    return new[] { "=", p.Substring(0, p.Length - 1) };
                return new[] { p.Substring(0, eq), "=", p.Substring(eq + 1) };
            }).ToArray();
            return allSplit;
        }

        private int FindTagEnd(string content, int start)
        {
            var i = start + 1;
            char? quotes = null;
            while (i < content.Length && (content[i] != '>' || quotes != null))
            {
                if (quotes == null)
                {
                    if (content[i] == '\'' || content[i] == '"')
                        quotes = content[i];
                }
                else
                {
                    if (content[i] == quotes) quotes = null;
                }

                i++;
            }

            return i;
        }
    }

    public class TagInfo
    {
        public int Start { get; set; }
        public int End { get; set; }

        public TagParameter[] Parameters { get; set; }
        public string TagContent { get; internal set; }
        public string TagInside { get; internal set; }
    }

    public class TagParameter
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
