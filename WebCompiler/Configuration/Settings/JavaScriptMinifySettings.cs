using NUglify.JavaScript;

namespace WebCompiler.Configuration.Settings
{
    /// <summary>
    /// Handle all options for JavaScript Minification
    /// </summary>
    public class JavaScriptMinifySettings : BaseMinifySettings
    {
        public bool RenameLocals { get; set; } = true;
        public bool PreserveImportantComments { get; set; } = true;
        public EvalTreatment EvalTreatment { get; set; } = EvalTreatment.Ignore;
        public static implicit operator CodeSettings(JavaScriptMinifySettings self)
        {
            return new CodeSettings
            {
                LocalRenaming = self.RenameLocals ? LocalRenaming.CrunchAll : LocalRenaming.KeepAll,
                PreserveImportantComments = self.PreserveImportantComments,
                Indent = new string(' ', self.IndentSize),
                TermSemicolons = self.TermSemicolons,
                OutputMode = self.OutputMode,
                EvalTreatment = self.EvalTreatment
            };
        }
    }
}
