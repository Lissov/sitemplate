using NUnit.Framework;

namespace Sitemplate.Test
{
    public class ForTest
    {
        [Test]
        public void RegularFor()
        {
            var file = @"<var $list></var>
                <set $list list>A,B,C</set>
                <for $item of $list>Item: $item.</for>";
            var processor = new TextProcessor();
            var context = new TemplateContext(processor);

            var result = processor.ProcessContent(file, context).Trim();

            Assert.AreEqual("Item: A.Item: B.Item: C.", result);
        }

        [Test]
        public void ListToTemplate()
        {
            var file = @"
                <var $list1></var>
                <set $list1 list>xyz1,xyz2</set>
                <inject templ $list=$list1></inject>";
            var processor = new TextProcessor();
            processor.Templates.Add("templ", "<if $list=\"\"><else><for $item of $list>$item</for></if> ");
            var context = new TemplateContext(processor);

            var result = processor.ProcessContent(file, context).Trim();

            Assert.AreEqual("xyz1xyz2", result);
        }
    }
}