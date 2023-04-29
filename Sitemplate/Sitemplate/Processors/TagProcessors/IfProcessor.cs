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
            var ifelse = tag.TagInside.Split(Constants.Tag.Else);
            if (processedKey.ToString() == processedValue.ToString())
                content = context.processor.ReplaceInContent(content, tag, ifelse.Length > 0 ? ifelse[0] : "");
            else
                content = context.processor.ReplaceInContent(content, tag, ifelse.Length > 1 ? ifelse[1] : "");

            return new Tuple<string, bool>(content, true);
        }
    }
}
