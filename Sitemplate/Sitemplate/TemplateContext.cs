using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitemplate
{
    public class TemplateContext
    {
        public Dictionary<string, object> Variables { get; set; } = new Dictionary<string, object>();

        public TemplateContext Clone()
        {
            return new TemplateContext
            {
                Variables = new Dictionary<string, object>(this.Variables)
            };
        }
    }
}
