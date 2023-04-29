namespace Sitemplate.Processors.TagProcessors
{
    class TagFactory
    {
        public static BaseTagProcessor GetProcessor(string tagName)
        {
            switch (tagName)
            {
                case InjectProcessor.TagName: return new InjectProcessor();
                case VarProcessor.TagName: return new VarProcessor();
                case SetProcessor.TagName: return new SetProcessor();
                case IfProcessor.TagName: return new IfProcessor();
                case ForProcessor.TagName: return new ForProcessor();
                default:
                    return null;
            }
        }
    }
}
