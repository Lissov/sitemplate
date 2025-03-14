using NUnit.Framework;
using Sitemplate.Processors;

namespace Sitemplate.Test
{
    public class VariablesTest
    {
        [Test]
        public void If()
        {
            var file = @"
                <var v1></var><set v1>A</set>
                <var v2></var>
                <if {{v1}}='A'><set v2>1</set><else><set v2>2</set></if>
                <if {{v2}}='2'><else><set v1>X</set></if>
                {{v1}} - {{v2}}";
            var processor = new TextProcessor();
            var context = new TemplateContext(processor);

            var result = processor.ProcessContent(file, context).Trim();

            Assert.AreEqual("X - 1", result);
        }

        [Test]
        public void VarDefineSetValue()
        {
            var file = @"
                <var v1>Abc</var>
                {{v1}}";
            var processor = new TextProcessor();
            var context = new TemplateContext(processor);

            var result = processor.ProcessContent(file, context).Trim();

            Assert.AreEqual("Abc", result);
        }

        [Test]
        public void VarDefineSetValuePar()
        {
            var file = @"
                <var v1='Abs'></var>
                {{v1}}";
            var processor = new TextProcessor();
            var context = new TemplateContext(processor);

            var result = processor.ProcessContent(file, context).Trim();

            Assert.AreEqual("Abs", result);
        }

        [Test]
        public void VarDefineMode()
        {
            var file = @"
                <var v1 json>{x: 1}</var>
                {{v1.x}}";
            var processor = new TextProcessor();
            var context = new TemplateContext(processor);

            var result = processor.ProcessContent(file, context).Trim();

            Assert.AreEqual("1", result);
        }
        
        [Test]
        public void SpecialChars()
        {
            var file = @"
                <var $(r-1)></var><set $(r-1)>A</set>
                <var $r></var><set $r>B</set>
                {{$(r-1)}}x : {{$r}}-1";
            var processor = new TextProcessor();
            var context = new TemplateContext(processor);

            var result = processor.ProcessContent(file, context).Trim();

            Assert.AreEqual("Ax : B-1", result);
        }


        [Test]
        public void FileName()
        {
            var file = @"
                <var ctx context></var>
                {{ctx.FileName}}";
            var processor = new TextProcessor();
            var context = new TemplateContext(processor, "C:/temp/MyFile.html");

            var result = processor.ProcessContent(file, context).Trim();

            Assert.AreEqual("MyFile.html", result);
        }
    }
}