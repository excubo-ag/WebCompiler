using JavaScriptEngineSwitcher.Core;
using JavaScriptEngineSwitcher.V8;

namespace WebCompiler.Compile
{
    public static class JsEngineFactory
    {
        private static readonly IJsEngineFactory jsEngineFactory = new V8JsEngineFactory();
        public static IJsEngineFactory Instance => jsEngineFactory;
    }
}
