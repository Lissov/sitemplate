using NUnit.Framework;

namespace Sitemplate.Test
{
    public class InjectTemplateTest
    {
        [Test]
        public void SimpleInject()
        {
            var file = "<h1>H1</h1> <inject templ1></inject>End";
            var processor = new TextProcessor();
            processor.Templates.Add("templ1", "<div>Abc</div>");
            var context = new TemplateContext(processor);

            var result = processor.ProcessContent(file, context);

            Assert.AreEqual("<h1>H1</h1> <div>Abc</div>End", result);
        }

        [Test]
        public void InjectWithVariables()
        {
            var file = "<h1>H1</h1> <inject templVar $var=\"value1\"></inject>End";
            var processor = new TextProcessor();
            var context = new TemplateContext(processor);
            processor.Templates.Add("templVar", "<div>Var=$var</div>");

            var result = processor.ProcessContent(file, context);

            Assert.AreEqual("<h1>H1</h1> <div>Var=value1</div>End", result);
        }
    }
}