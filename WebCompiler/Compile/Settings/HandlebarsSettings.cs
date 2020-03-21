using System.Collections.Generic;

namespace WebCompiler
{
    public class HandlebarsSettings : BaseSettings
    {
        public string Name { get; set; }
        public string Namespace { get; set; }
        public string Root { get; set; }
        public string CommonJs { get; set; }
        public bool AMD { get; set; }
        public bool ForcePartial { get; set; }
        public bool NoBOM { get; set; }
        public bool KnownHelpersOnly { get; set; }
        public string[] KnownHelpers { get; set; }
        public override void ChangeSettings(Dictionary<string, object> values)
        {
            if (values.ContainsKey(nameof(Name)))
            {
                Name = values[nameof(Name)].ToString();
            }
            if (values.ContainsKey(nameof(Namespace)))
            {
                Namespace = values[nameof(Namespace)].ToString();
            }
            if (values.ContainsKey(nameof(Root)))
            {
                Root = values[nameof(Root)].ToString();
            }
            if (values.ContainsKey(nameof(CommonJs)))
            {
                CommonJs = values[nameof(CommonJs)].ToString();
            }
            if (values.ContainsKey(nameof(AMD)))
            {
                AMD = (bool)values[nameof(AMD)];
            }
            if (values.ContainsKey(nameof(ForcePartial)))
            {
                ForcePartial = (bool)values[nameof(ForcePartial)];
            }
            if (values.ContainsKey(nameof(NoBOM)))
            {
                NoBOM = (bool)values[nameof(NoBOM)];
            }
            if (values.ContainsKey(nameof(KnownHelpersOnly)))
            {
                KnownHelpersOnly = (bool)values[nameof(KnownHelpersOnly)];
            }
            if (values.ContainsKey(nameof(KnownHelpers)))
            {
                KnownHelpers = (string[])values[nameof(KnownHelpers)];
            }
            base.ChangeSettings(values);
        }
    }
}
