using NUnit.Framework;

namespace Sitemplate.Test
{
    public class VariablesTest
    {
        [Test]
        public void If()
        {
            var file = @"
                <var $v1></var><set $v1>A</set>
                <var $v2></var>
                <if $v1='A'><set $v2>1</set><else><set $v2>2</set></if>
                <if $v2='2'><else><set $v1>X</set></if>
                $v1 - $v2";
            var processor = new TextProcessor();
            var context = new TemplateContext(processor);

            var result = processor.ProcessContent(file, context).Trim();

            Assert.AreEqual("X - 1", result);
        }
    }
}