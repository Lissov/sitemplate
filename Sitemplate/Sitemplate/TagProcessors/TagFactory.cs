namespace Sitemplate.TagProcessors
{
    class TagFactory
    {
        public static BaseProcessor GetProcessor(string tagName)
        {
            switch (tagName)
            {
                case InjectProcessor.TagName: return new InjectProcessor();
                case VarProcessor.TagName: return new VarProcessor();
                case SetProcessor.TagName: return new SetProcessor();
                case IfProcessor.TagName: return new IfProcessor();
                case BaseProcessor.TagName:
                default:
                    return new BaseProcessor();
            }
        }
    }
}
