using Sitemplate.Processors;
using System.Collections.Generic;
using System.IO;

namespace Sitemplate
{
    public class TemplateContext
    {
        public TextProcessor processor;
        private string FullFileName;
        public TemplateContext(TextProcessor processor, string fileName = null)
        {
            this.processor = processor;
            this.FullFileName = fileName;
        }

        public string FileName => Path.GetFileName(FullFileName);
        public string FileNameWithoutExt => Path.GetFileNameWithoutExtension(FullFileName);

        public Dictionary<string, object> Variables { get; set; } = new Dictionary<string, object>();

        public TemplateContext Clone()
        {
            return new TemplateContext(processor, FileName)
            {
                Variables = new Dictionary<string, object>(this.Variables)
            };
        }
    }
}
