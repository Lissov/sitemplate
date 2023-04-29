using NUnit.Framework;
using Sitemplate.Processors;

namespace Sitemplate.Test
{
    public class TagTest
    {
        [Test]
        public void Enclosed()
        {
            var file = @"<if A='-'><if B='-'>C</if></if>";
            var processor = new TextProcessor();
            var context = new TemplateContext(processor);

            var result = processor.ProcessContent(file, context).Trim();

            Assert.AreEqual("", result);
        }

        [Test]
        public void IfForIfComplicated()
        {
            var file = @"<var $arr json>[
                    { name: 'name1', map: 'map_1' },
                    { name: 'name2', map: 'map_2' }
                ]</var>
                <inject tem.1 $maps='{{arr}}'></inject>";
            var processor = new TextProcessor();
            processor.Templates.Add("tem.1", @"<if {{$maps}}=""""><else>
                <for $map of $maps>{{$map.name}}&nbsp;</for>
                <if {{$map.name}}=""""><else>{{$map.name}}
                </if>
              </if>");
            var context = new TemplateContext(processor);

            var result = processor.ProcessContent(file, context).Trim();

            Assert.AreEqual("", result);
        }

        [Test]
        public void IfElseEnclosed()
        {
            var file = @"<var v1='x'></var>
                <if {{v1}}='y'>
                    <if {{v1}}='x'>All if skipped<else>Else of skipped if</if>
                <else>
                    Else of outer if
                </if>";
            var processor = new TextProcessor();
            var context = new TemplateContext(processor);

            var result = processor.ProcessContent(file, context).Trim();

            Assert.AreEqual("Else of outer if", result);
        }

        [Test]
        public void IfEnclosedIff()
        {
            var file = @"<var v1='x'></var>
                <if {{v1}}='y'><iffy></iffy><else>Else</if>";
            var processor = new TextProcessor();
            var context = new TemplateContext(processor);

            var result = processor.ProcessContent(file, context).Trim();

            Assert.AreEqual("Else", result);
        }
    }
}