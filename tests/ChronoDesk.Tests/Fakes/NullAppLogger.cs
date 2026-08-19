using ChronoDesk.Core.Abstractions;

namespace ChronoDesk.Tests.Fakes;

internal sealed class NullAppLogger : IAppLogger
{
    public void Info(string eventName, string message)
    {
    }

    public void Warning(string eventName, string message)
    {
    }

    public void Error(string eventName, Exception exception, string safeMessage)
    {
    }
}
