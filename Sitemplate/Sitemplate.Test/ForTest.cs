using NUnit.Framework;
using Sitemplate.Processors;

namespace Sitemplate.Test
{
    public class ForTest
    {
        [Test]
        public void RegularFor()
        {
            var file = @"<var l1></var>
                <set l1 list>A,B,C</set>
                <for item of l1>Item: {{item}}.</for>";
            var processor = new TextProcessor();
            var context = new TemplateContext(processor);

            var result = processor.ProcessContent(file, context).Trim();

            Assert.AreEqual("Item: A.Item: B.Item: C.", result);
        }

        [Test]
        public void ListToTemplate()
        {
            var file = @"
                <var l1></var>
                <set l1 list>xyz1,xyz2</set>
                <inject templ lst='{{l1}}'></inject>";
            var processor = new TextProcessor();
            processor.Templates.Add("templ", "<if lst=\"\"><else><for item of lst>{{item}}</for></if>");
            var context = new TemplateContext(processor);

            var result = processor.ProcessContent(file, context).Trim();

            Assert.AreEqual("xyz1xyz2", result);
        }

        [Test]
        public void JsonToTemplate()
        {
            var file = @"
                <var lst json>[{ name: 'Anne', age: '15' }, { name: 'Paul', age: '17' }]</var>
                <inject bio people='{{lst}}'></inject>";
            var processor = new TextProcessor();
            processor.Templates.Add("bio", "<if {{people}}=\"\"><else><for pers of people>{{pers.name}}: {{pers.age}}. </for></if>");
            var context = new TemplateContext(processor);

            var result = processor.ProcessContent(file, context).Trim();

            Assert.AreEqual("Anne: 15. Paul: 17.", result);
        }
    }
}