using System.Collections.Generic;

namespace WebCompiler
{
    /// <summary>
    /// Give all options for the CoffeeScript compiler
    /// </summary>
    public class IcedCoffeeScriptSettings : BaseSettings
    {

        /// <summary>
        /// Compile the JavaScript without the top-level function safety wrapper.
        /// </summary>
        public bool Bare { get; set; } = false;

        /// <summary>
        /// Specify how the Iced runtime is included in the output JavaScript file.
        /// </summary>
        public string RuntimeMode { get; set; } = "node";
        public override void ChangeSettings(Dictionary<string, object> values)
        {
            if (values.ContainsKey(nameof(Bare)))
            {
                Bare = (bool)values[nameof(Bare)];
            }
            if (values.ContainsKey(nameof(RuntimeMode)))
            {
                RuntimeMode = values[nameof(RuntimeMode)].ToString();
            }
            base.ChangeSettings(values);
        }
    }
}
