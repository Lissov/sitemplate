using System;
using System.Collections;

namespace Sitemplate.TagProcessors
{
    class ForProcessor : BaseProcessor
    {
        public const string TagName = Constants.Tag.For;

        public override Tuple<string, bool> Process(string content, TagInfo tag, TemplateContext context)
        {
            if (tag.Parameters.Length != 3)
                throw new Exception($"'{TagName}' requires 3 parameters: " + tag.TagContent);
            if (tag.Parameters[1].Key != "of")
                throw new Exception($"'{TagName}' requires second parameter to be keyword 'of': " + tag.TagContent);

            var varName = tag.Parameters[0].Key;
            var listName = tag.Parameters[2].Key;

            if (!context.Variables.ContainsKey(listName))
                throw new Exception($"List '{listName}' not defined: " + tag.TagContent);
            var list = context.Variables[listName] as IEnumerable;
            if (list == null)
                throw new Exception($"List '{listName}' is not an IEnumerable and can't be used in ${TagName}: " + tag.TagContent);
            var res = "";
            var iterationContext = context.Clone();
            foreach (var item in list)
            {
                iterationContext.Variables[varName] = item;
                var template = tag.TagInside;
                res += context.processor.ProcessContent(template, iterationContext);
            }
                
            content = context.processor.ReplaceInContent(content, tag, res);

            return new Tuple<string, bool>(content, true);
        }
    }
}
