using System;

namespace Sitemplate.Processors.TagProcessors
{
    class IfProcessor : BaseTagProcessor
    {
        public const string TagName = Constants.Tag.If;

        public override Tuple<string, bool> Process(string content, TagInfo tag, TemplateContext context)
        {
            if (tag.Parameters.Length != 1)
                throw new Exception("'if' requires single parameter: " + tag.TagContent);
            // special mode - allow to use variables in 'if' without {{..}}
            var processedKey = context.Variables.ContainsKey(tag.Parameters[0].Key)
                ? context.Variables[tag.Parameters[0].Key]
                : context.processor.ProcessContent(tag.Parameters[0].Key, context);
            var v = tag.Parameters[0].Value;
            var processedValue = v != null
                ? context.Variables.ContainsKey(v)
                    ? context.Variables[v]
                    : context.processor.ProcessContent(tag.Parameters[0].Value, context)
                : null;
            var ifelse = SplitByElse(tag.TagInside);
            if (processedKey.ToString() == processedValue.ToString())
                content = context.processor.ReplaceInContent(content, tag, ifelse.Item1);
            else
                content = context.processor.ReplaceInContent(content, tag, ifelse.Item2);

            return new Tuple<string, bool>(content, true);
        }

        private Tuple<string, string> SplitByElse(string tagInside)
        {
            var ind = 0;
            var ifCnt = 0;
            do
            {
                var i_if = tagInside.IndexOf('<' + Constants.Tag.If, ind);
                var i_eif = tagInside.IndexOf("</" + Constants.Tag.If, ind);
                var i_else = tagInside.IndexOf(Constants.Tag.Else, ind);
                if (i_else < 0)
                    return new Tuple<string, string>(tagInside, "");
                if (i_if >= 0 && i_if < i_else)
                {
                    ifCnt++;
                    ind = i_if + 1;
                    continue;
                }
                if (i_eif >= 0 && i_eif < i_else)
                {
                    ifCnt--;
                    if (ifCnt < 0)
                        return new Tuple<string, string>(tagInside, "");
                    ind = i_eif + 1;
                    continue;
                }
                if (ifCnt == 0) {
                    var e = i_else + Constants.Tag.Else.Length;
                    return new Tuple<string, string>(
                        tagInside.Substring(0, i_else),
                        tagInside.Substring(e, tagInside.Length - e)
                    );
                }
                ind = i_else + 1;
            } while (ind < tagInside.Length);
            return new Tuple<string, string>(tagInside, "");
        }
    }
}
