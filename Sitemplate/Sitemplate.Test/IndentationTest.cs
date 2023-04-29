using NUnit.Framework;
using Sitemplate.Processors;

namespace Sitemplate.Test
{
    public class IndentationTest
    {
        [Test]
        public void IndentationInInjectTest()
        {
            var file = @"
<div>
    <inject templ1></inject>
    Next
</div>";
            var processor = new TextProcessor();
            processor.Templates.Add("templ1",@"<p>Abc</p>
<p>Cde</p>");
            var context = new TemplateContext(processor);

            var result = processor.ProcessContent(file, context);

            Assert.AreEqual(@"
<div>
    <p>Abc</p>
    <p>Cde</p>
    Next
</div>", result);
        }

        [Test]
        public void PreserveDivTest()
        {
            var file = @"
    Here goes some content
<inject templ1></inject>";
            var processor = new TextProcessor();
            processor.Templates.Add("templ1", @"    </div>
    <div class='footer'>
      <div class='footer-content'>
      </div>
    </div>
  </body>
</html>");
            var context = new TemplateContext(processor);

            var result = processor.ProcessContent(file, context);

            Assert.AreEqual(@"
    Here goes some content
    </div>
    <div class='footer'>
      <div class='footer-content'>
      </div>
    </div>
  </body>
</html>", result);
        }

        [Test]
        public void ClearBlanksTest()
        {
            var file = @"<inject templ1></inject>";
            var processor = new TextProcessor();
            processor.Templates.Add("templ1", @"
                <var v1='a'></var>
                <var v2='b'></var>
                <if {{v1}}='b'>X</if>
                <if {{v2}}='b'>Y</if>");
            var context = new TemplateContext(processor);

            var result = processor.ProcessContent(file, context);

            Assert.AreEqual(@"
                Y", result);
        }

        [Test]
        public void ClearLinesAround()
        {
            var file = @"
                <var v1='a'></var>
                <if {{v1}}='a'>
                A
                </if>
                B
                <if {{v1}}='b'>
                C
                </if>
                D";
            var processor = new TextProcessor();
            var context = new TemplateContext(processor);

            var result = processor.ProcessContent(file, context);

            Assert.AreEqual(@"
                A
                B
                D", result);
        }

        [Test]
        public void IndentationDoesNotOffsetFurterParser()
        {
            var file = @"
                <var v1='a'></var>
                        <if {{v1}}='b'>More indentation</if>
                <if {{v1}}='a'>Expected</if>";
            var processor = new TextProcessor();
            var context = new TemplateContext(processor);

            var result = processor.ProcessContent(file, context);

            Assert.AreEqual(@"
                Expected", result);
        }

        [Test]
        public void IndentationAdjusted()
        {
            var file = @"
                <var v1='a'></var>
                <if {{v1}}='a'>
                    Should not be extra indented.
                    Next line should stay.
                        And a bit indented if needed.
                </if>
                <if {{v1}}='b'><else>
                    Else should also stay.
                </if>";
            var processor = new TextProcessor();
            var context = new TemplateContext(processor);

            var result = processor.ProcessContent(file, context);

            Assert.AreEqual(@"
                Should not be extra indented.
                Next line should stay.
                    And a bit indented if needed.
                Else should also stay.", result);
        }

        [Test]
        public void IndentationInTemplate()
        {
            var file = @"
                Aligned to text, please.
                <inject t1></inject>";
            var processor = new TextProcessor();
            processor.Templates.Add("t1", @"
<p>
    Something indented.
    And aligned.
      with 2 extra spaces.
</p>");
            var context = new TemplateContext(processor);

            var result = processor.ProcessContent(file, context);

            Assert.AreEqual(@"
                Aligned to text, please.
                
                <p>
                    Something indented.
                    And aligned.
                      with 2 extra spaces.
                </p>", result);
        }
    }
}