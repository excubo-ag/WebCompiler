using NUglify.Css;

namespace WebCompiler.Configuration.Settings
{
    /// <summary>
    /// Handle all options for Css Minification
    /// </summary>
    public class CssMinifySettings : BaseMinifySettings
    {
        public CssComment CommentMode { get; set; } = CssComment.Important;
        public CssColor ColorNames { get; set; } = CssColor.Hex;
        public static implicit operator CssSettings(CssMinifySettings self)
        {
            return new CssSettings
            {
                TermSemicolons = self.TermSemicolons,
                IndentSize = self.IndentSize,
                CommentMode = self.CommentMode,
                ColorNames = self.ColorNames,
                OutputMode = self.OutputMode
            };
        }
    }
}
