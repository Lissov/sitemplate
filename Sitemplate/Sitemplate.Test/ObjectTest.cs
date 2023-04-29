using NUnit.Framework;
using Sitemplate.Processors;

namespace Sitemplate.Test
{
    public class ObjectTest
    {
        [Test]
        public void ParseJsonTest()
        {
            var file = @"
                <var v1></var>
                <set v1 json>{one: '1', two: '2'}</set>
                {{v1.one}} and {{v1.two}}";
            var processor = new TextProcessor();
            var context = new TemplateContext(processor);

            var result = processor.ProcessContent(file, context).Trim();

            Assert.AreEqual("1 and 2", result);
        }

    }
}