using NUglify;
using NUglify.Css;
using System;
using System.Collections.Generic;

namespace WebCompiler
{
    /// <summary>
    /// Handle all options for Css Minification
    /// </summary>
    public class CssMinifySettings : BaseMinifySettings
    {
        public string CommentMode { get; set; }
        public string ColorNames { get; set; }
        public CssSettings ToCssSettings()
        {
            var settings = new CssSettings
            {
                IndentSize = IndentSize
            };
            if (CommentMode != null)
            {
                settings.CommentMode = (CssComment)Enum.Parse(typeof(CssComment), CommentMode, true);
            }
            if (ColorNames != null)
            {
                settings.ColorNames = (CssColor)Enum.Parse(typeof(CssColor), ColorNames, true);
            }
            if (OutputMode != null)
            {
                settings.OutputMode = (OutputMode)Enum.Parse(typeof(OutputMode), OutputMode, true);
            }
            return settings;
        }
        public override void ChangeSettings(Dictionary<string, object> values)
        {
            if (values.ContainsKey(nameof(CommentMode).ToLowerInvariant()))
            {
                CommentMode = values[nameof(CommentMode).ToLowerInvariant()].ToString();
            }
            if (values.ContainsKey(nameof(ColorNames).ToLowerInvariant()))
            {
                ColorNames = values[nameof(ColorNames).ToLowerInvariant()].ToString();
            }
            base.ChangeSettings(values);
        }
    }
}
