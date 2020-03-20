namespace WebCompiler
{
    internal class LessDependencyResolver : SassDependencyResolver
    {
        public override string[] SearchPatterns => new string[] { "*.less" };

        public override string FileExtension => ".less";

    }
}
