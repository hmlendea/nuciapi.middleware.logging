using NuciLog.Core;

namespace NuciAPI.Middleware.Logging
{
    internal sealed class MyOperation : Operation
    {
        MyOperation(string name) : base(name) { }

        internal static Operation HttpRequest => new MyOperation(nameof(HttpRequest));
    }
}
