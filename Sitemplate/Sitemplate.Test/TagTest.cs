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

/*        [Test]
        public void ForIfForComplicated()
        {
            var file = @"<if A='-'><if B='-'>C</if></if>";
            var processor = new TextProcessor();
            @"<if {{$maps}}=""><else>
                <for $map of $maps><a href='{ {$map.name} }
            " target="_blank">Karte</a>&nbsp;</for>
<if { {$map.name} }= "" ><else>< a href = "../gps/{{$map.name}}.gpx" target = "_blank" > GPX Datei </ a >

< button type = "button" onclick = "navigator.clipboard.writeText(window.location.origin + '/gps/{{$map.name}}.gpx')" > Copy Link </ button >

</if>

</if> "
            var context = new TemplateContext(processor);

            var result = processor.ProcessContent(file, context).Trim();

            Assert.AreEqual("", result);
        }*/

    }
}