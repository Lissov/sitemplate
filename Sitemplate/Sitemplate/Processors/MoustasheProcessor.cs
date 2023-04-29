using Newtonsoft.Json.Linq;
using System;

namespace Sitemplate.Processors
{
    class MoustasheProcessor: BaseProcessor<string, object>
    {
        public bool IsMoustashe(string value)
        {
            var t = value.Trim();
            return t.StartsWith(Constants.MoustasheStart) && t.EndsWith(Constants.MoustasheEnd);
        }

        public override object Process(string content, string parameter, TemplateContext context)
        {
            var trimmed = content.Substring(2, content.Length - 4).Trim();
            var path = trimmed.Split('.');
            if (context.Variables.ContainsKey(path[0]))
            {
                var obj = context.Variables[path[0]];
                if (path.Length == 1)
                    return obj;

                if (obj is JObject)
                {
                    return ((JObject)obj).SelectToken(trimmed.Substring(path[0].Length));
                }
                else
                {
                    for (var i = 1; i < path.Length; i++)
                    {
                        if (obj == null)
                            throw new Exception($"Null reference in expression [{content}], evaluating [{path[0]}]");
                        var p = obj.GetType().GetProperty(path[i]);
                        if (p == null)
                            throw new Exception($"Missing property [{path[i]}] in expression [{content}].");
                        obj = p.GetValue(obj);
                    }
                    return obj;
                }
            }
            return null;
        }
    }
}
