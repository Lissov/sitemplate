using System;

namespace Sitemplate.TagProcessors
{
    class IfProcessor : BaseProcessor
    {
        public const string TagName = Constants.Tag.If;

        public override Tuple<string, bool> Process(string content, TagInfo tag, TemplateContext context)
        {
            if (tag.Parameters.Length != 1)
                throw new Exception("'if' requires single parameter: " + tag.TagContent);
            var processedKey = context.processor.ProcessContent(tag.Parameters[0].Key, context);
            var processedValue = tag.Parameters[0].Value != null
                ? context.processor.ProcessContent(tag.Parameters[0].Value, context)
                : null;
            var ifelse = tag.TagInside.Split(Constants.Tag.Else);
            if (processedKey == processedValue)
                content = context.processor.ReplaceInContent(content, tag, ifelse.Length > 0 ? ifelse[0] : "");
            else
                content = context.processor.ReplaceInContent(content, tag, ifelse.Length > 1 ? ifelse[1] : "");

            return new Tuple<string, bool>(content, true);
        }
    }
}
