using System;

namespace Sitemplate.Processors.TagProcessors
{
    class SetProcessor : VarProcessor
    {
        public new const string TagName = Constants.Tag.Set;

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
    }
}
