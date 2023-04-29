using System;
using System.Collections;

namespace Sitemplate.Processors.TagProcessors
{
    class ForProcessor : BaseTagProcessor
    {
        public const string TagName = Constants.Tag.For;

        public override Tuple<string, int> Process(string content, TagInfo tag, TemplateContext context)
        {
            if (tag.Parameters.Length != 3)
                throw new Exception($"'{TagName}' requires 3 parameters: " + tag.TagContent);
            if (tag.Parameters[1].Key != "of")
                throw new Exception($"'{TagName}' requires second parameter to be keyword 'of': " + tag.TagContent);

            var varName = tag.Parameters[0].Key;
            var listName = tag.Parameters[2].Key;

            var list = context.Variables.ContainsKey(listName)
                ? context.Variables[listName] as IEnumerable
                : null;
            var res = "";
            if (list != null)
            {
                var iterationContext = context.Clone();
                foreach (var item in list)
                {
                    iterationContext.Variables[varName] = item;
                    var template = tag.TagInside;
                    res += context.processor.ProcessContent(template, iterationContext);
                }
            }
                
            return ReplaceInContent(content, tag, context, res, true);
        }
    }
}
