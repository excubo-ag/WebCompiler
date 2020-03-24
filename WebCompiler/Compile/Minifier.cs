using System.IO;

namespace WebCompiler.Compile
{
    public abstract class Minifier : Compiler
    {
        protected static string GetMinFileName(string file)
        {
            var ext = Path.GetExtension(file);

            var fileName = file.Substring(0, file.LastIndexOf(ext));
            if (!fileName.EndsWith(".min"))
            {
                fileName += ".min";
            }

            return fileName + ext;
        }
    }
}
