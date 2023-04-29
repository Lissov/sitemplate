using System;

namespace Sitemplate.Processors.TagProcessors
{
    abstract class BaseTagProcessor : BaseProcessor<TagInfo, Tuple<string, int>>
    {
        protected void PushParameters(TemplateContext tagContext, TagInfo tag)
        {
            foreach (var par in tag.Parameters)
            {
                if (par.Value != null)
                {
                    tagContext.Variables[par.Key] = tagContext.processor.EvaluateValue(par.Value, tagContext);
                }
            }
        }

        protected Tuple<string, int> ReplaceInContent(string content, TagInfo tag, TemplateContext context, string insert, bool cleanIndentation)
        {
            var indentation = GetIndentation(content, tag.Start);
            var cleaned = cleanIndentation ? RemoveFirstLineIndentation(insert) : insert;
            var indented = InjectIndentation(cleaned, indentation);
            return context.processor.ReplaceInContent(content, tag, indented);
        }
    }
}
