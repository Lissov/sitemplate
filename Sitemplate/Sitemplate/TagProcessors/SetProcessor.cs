using System;
using System.Linq;

namespace Sitemplate.TagProcessors
{
    class SetProcessor : BaseProcessor
    {
        public const string TagName = Constants.Tag.Set;

        public override Tuple<string, bool> Process(string content, TagInfo tag, TemplateContext context)
        {
            if (tag.Parameters.Length < 1)
                throw new Exception("'set' must have at lease one parameter with var name");
            var mode = tag.Parameters.Length >= 2 ? tag.Parameters[1].Key : null;
            var value = ProcessWithMode(tag.TagInside, mode, context);
            context.Variables[tag.Parameters[0].Key] = value;
            content = context.processor.ReplaceInContent(content, tag, "");

            return new Tuple<string, bool>(content, true);
        }

        private object ProcessWithMode(string value, string mode, TemplateContext context)
        {
            switch (mode)
            {
                case "list":
                    var values = value.Split(',', ';').Select(x => x.Trim());
                    return values.Select(v => context.processor.ProcessContent(v, context)).ToList();
                default:
                    return context.processor.ProcessContent(value, context);
            }
        }
    }
}
