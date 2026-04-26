using NuciLog.Core;

namespace NuciAPI.Middleware.Logging
{
    internal sealed class MyLogInfoKey : LogInfoKey
    {
        MyLogInfoKey(string name) : base(name) { }

        internal static LogInfoKey ClientId => new MyLogInfoKey(nameof(ClientId));
        internal static LogInfoKey ElapsedMilliseconds => new MyLogInfoKey(nameof(ElapsedMilliseconds));
        internal static LogInfoKey Hostname => new MyLogInfoKey(nameof(Hostname));
        internal static LogInfoKey HmacToken => new MyLogInfoKey(nameof(HmacToken));
        internal static LogInfoKey IpAddress => new MyLogInfoKey(nameof(IpAddress));
        internal static LogInfoKey Method => new MyLogInfoKey(nameof(Method));
        internal static LogInfoKey Path => new MyLogInfoKey(nameof(Path));
        internal static LogInfoKey QueryString => new MyLogInfoKey(nameof(QueryString));
        internal static LogInfoKey RequestId => new MyLogInfoKey(nameof(RequestId));
        internal static LogInfoKey StatusCode => new MyLogInfoKey(nameof(StatusCode));
        internal static LogInfoKey Timestamp => new MyLogInfoKey(nameof(Timestamp));
    }
}
