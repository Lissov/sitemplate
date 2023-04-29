using Newtonsoft.Json;
using System;
using System.Linq;

namespace Sitemplate.Processors.TagProcessors
{
    class VarProcessor : BaseTagProcessor
    {
        public const string TagName = Constants.Tag.Declare;

        public override Tuple<string, int> Process(string content, TagInfo tag, TemplateContext context)
        {
            if (tag.Parameters.Length < 1)
                throw new Exception("Variable name expected: " + tag.TagContent);
            if (tag.Parameters.Length > 2)
                throw new Exception("Too many parameters: " + tag.TagContent);
            var mode = tag.Parameters.Length == 2 ? tag.Parameters[1].Key : "";
            foreach (var par in tag.Parameters)
            {
                context.Variables[par.Key] = par.Value != null
                    ? ProcessWithMode(par.Value, mode, context)
                    : ProcessWithMode(tag.TagInside, mode, context);
            }
            return context.processor.ReplaceInContent(content, tag, "");
        }


        protected object ProcessWithMode(string value, string mode, TemplateContext context)
        {
            switch (mode)
            {
                case "list":
                    var values = value.Split(',', ';').Select(x => x.Trim());
                    return values.Select(v => context.processor.ProcessContent(v, context)).ToList();
                case "json":
                    return JsonConvert.DeserializeObject(value);
                default:
                    return context.processor.ProcessContent(value, context);
            }
        }
    }
}
