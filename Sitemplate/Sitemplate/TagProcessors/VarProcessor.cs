using System;

namespace Sitemplate.TagProcessors
{
    class VarProcessor : BaseProcessor
    {
        public const string TagName = Constants.Tag.Declare;

        public override Tuple<string, bool> Process(string content, TagInfo tag, TemplateContext context)
        {
            foreach (var par in tag.Parameters)
            {
                context.Variables[par.Key] = par.Value;
            }
            content = context.processor.ReplaceInContent(content, tag, "");

            return new Tuple<string, bool>(content, true);
        }
    }
}
