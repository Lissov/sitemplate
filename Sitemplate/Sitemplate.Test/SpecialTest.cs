using NUnit.Framework;

namespace Sitemplate.Test
{
    public class SpecialTest
    {
        [Test]
        public void NotClosedHtml()
        {
            var file = "<body><div></a></div>";
            var processor = new TextProcessor();
            var context = new TemplateContext(processor);

            var result = processor.ProcessContent(file, context).Trim();

            Assert.AreEqual("<body><div></a></div>", result);
        }
    }
}