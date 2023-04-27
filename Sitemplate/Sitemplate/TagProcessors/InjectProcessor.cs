using System;

namespace Sitemplate.TagProcessors
{
    class InjectProcessor : BaseProcessor
    {
        public const string TagName = Constants.Tag.Inject;

        public override Tuple<string, bool> Process(string content, TagInfo tag, TemplateContext context)
        {
            if (tag.Parameters.Length < 1 || tag.Parameters[0].Value != null)
                throw new Exception($"Tag '{TagName}' must have first parameter as template name");
            var templatename = tag.Parameters[0].Key;
            var templates = context.processor.Templates;
            if (!templates.ContainsKey(templatename))
                throw new Exception($"Template not found: '{templatename}'.");
            var tagContext = context.Clone();
            PushParameters(tagContext, tag);

            var template = templates[templatename];
            template = context.processor.ProcessContent(template, tagContext);
            var indentation = GetIndentation(content, tag.Start);
            template = InjectIndentation(template, indentation);

            content = context.processor.ReplaceInContent(content, tag, template);
            Console.WriteLine($"\tInjected template: {templatename}");


            return new Tuple<string, bool>(content, true);
        }
    }
}
