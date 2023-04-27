using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitemplate
{
    public class TemplateContext
    {
        public TextProcessor processor;
        public TemplateContext(TextProcessor processor)
        {
            this.processor = processor;
        }

        public Dictionary<string, object> Variables { get; set; } = new Dictionary<string, object>();

        public TemplateContext Clone()
        {
            return new TemplateContext(processor)
            {
                Variables = new Dictionary<string, object>(this.Variables)
            };
        }
    }
}
