using NUglify;
using NUglify.JavaScript;
using System;
using System.Collections.Generic;

namespace WebCompiler
{
    /// <summary>
    /// Handle all options for JavaScript Minification
    /// </summary>
    public class JavaScriptMinifySettings : BaseMinifySettings
    {
        public bool RenameLocals { get; set; } = true;
        public bool PreserveImportantComments { get; set; } = true;
        public string EvalTreatment { get; set; } = "ignore";
        public CodeSettings ToCodeSettings()
        {
            var settings = new CodeSettings
            {
                LocalRenaming = RenameLocals ? LocalRenaming.CrunchAll : LocalRenaming.KeepAll,
                PreserveImportantComments = PreserveImportantComments,
                IndentSize = IndentSize
            };
            if (EvalTreatment != null)
            {
                settings.EvalTreatment = (EvalTreatment)Enum.Parse(typeof(EvalTreatment), EvalTreatment, true);
            }
            if (OutputMode != null)
            {
                settings.OutputMode = (OutputMode)Enum.Parse(typeof(OutputMode), OutputMode, true);
            }
            return settings;
        }
        public override void ChangeSettings(Dictionary<string, object> values)
        {
            if (values.ContainsKey(nameof(RenameLocals).ToLowerInvariant()))
            {
                RenameLocals = bool.Parse(values[nameof(RenameLocals).ToLowerInvariant()].ToString());
            }
            if (values.ContainsKey(nameof(PreserveImportantComments).ToLowerInvariant()))
            {
                PreserveImportantComments = bool.Parse(values[nameof(PreserveImportantComments).ToLowerInvariant()].ToString());
            }
            if (values.ContainsKey(nameof(EvalTreatment).ToLowerInvariant()))
            {
                EvalTreatment = values[nameof(EvalTreatment).ToLowerInvariant()].ToString();
            }
            base.ChangeSettings(values);
        }
    }
}
