using System;

namespace Sitemplate.TagProcessors
{
    class SetProcessor : BaseProcessor
    {
        public const string TagName = Constants.Tag.Set;

        public override Tuple<string, bool> Process(string content, TagInfo tag, TemplateContext context)
        {
            if (tag.Parameters.Length != 1)
                throw new Exception("'set' must have one parameter");
            context.Variables[tag.Parameters[0].Key] = context.processor.ProcessContent(tag.TagInside, context);
            content = context.processor.ReplaceInContent(content, tag, "");

            return new Tuple<string, bool>(content, true);
        }
    }
}
