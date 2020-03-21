namespace WebCompiler
{
    public class CompilerSettingsCollection
    {
        public BabelSettings Babel { get; set; } = new BabelSettings();
        public HandlebarsSettings Handlebars { get; set; } = new HandlebarsSettings();
        public IcedCoffeeScriptSettings IcedCoffeeScript { get; set; } = new IcedCoffeeScriptSettings();
        public LessSettings Less { get; set; } = new LessSettings();
        public SassSettings Sass { get; set; } = new SassSettings();
        public StylusSettings Stylus { get; set; } = new StylusSettings();
    }
}
