using System;

namespace Sitemplate.Processors.TagProcessors
{
    abstract class BaseTagProcessor : BaseProcessor<TagInfo, Tuple<string, bool>>
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
    }
}
