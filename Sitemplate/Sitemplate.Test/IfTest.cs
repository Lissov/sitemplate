using NUnit.Framework;

namespace Sitemplate.Test
{
    public class IfTest
    {
        [Test]
        public void If()
        {
            var file = "<var $v1></var><set $v1>true</set><if $v1='true'>A</if>";
            var processor = new TextProcessor();
            var context = new TemplateContext(processor);

            var result = processor.ProcessContent(file, context);

            Assert.AreEqual("A", result);
        }

        [Test]
        public void IfWithElse()
        {
            var file = "<var $v1></var><set $v1>true</set><if $v1=\"true\">A<else>B</if>";
            var processor = new TextProcessor();
            var context = new TemplateContext(processor);

            var result = processor.ProcessContent(file, context);

            Assert.AreEqual("A", result);
        }

        [Test]
        public void IfNoElseFalse()
        {
            var file = "<var $v1></var><set $v1>false</set><if $v1=\"true\">A</if>";
            var processor = new TextProcessor();
            var context = new TemplateContext(processor);

            var result = processor.ProcessContent(file, context);

            Assert.AreEqual("", result);
        }

        [Test]
        public void IfElse()
        {
            var file = "<var $v1></var><set $v1>false</set><if $v1=\"true\">A<else>B</if>";
            var processor = new TextProcessor();
            var context = new TemplateContext(processor);

            var result = processor.ProcessContent(file, context);

            Assert.AreEqual("B", result);
        }

        [Test]
        public void IfInTemplate()
        {
            var file = "<inject templ $v1='true'></inject>";
            var processor = new TextProcessor();
            processor.Templates.Add("templ", "<if $v1='true'>A<else>B</if>");
            var context = new TemplateContext(processor);

            var result = processor.ProcessContent(file, context);

            Assert.AreEqual("A", result);
        }


        [Test]
        public void IfInTemplateFalse()
        {
            var file = "<inject templ $v1='false'></inject>";
            var processor = new TextProcessor();
            processor.Templates.Add("templ", "<if $v1='true'>A<else>B</if>");
            var context = new TemplateContext(processor);

            var result = processor.ProcessContent(file, context);

            Assert.AreEqual("B", result);
        }

        /*[Test]
        public void IfNotDeclared()
        {
            var file = "<if $v1=\"\">A<else>B</if>";
            var processor = new TextProcessor();
            var context = new TemplateContext(processor);

            var result = processor.ProcessContent(file, context);

            Assert.AreEqual("A", result);
        }*/

    }
}