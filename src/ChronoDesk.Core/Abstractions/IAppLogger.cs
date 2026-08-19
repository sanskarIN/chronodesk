namespace ChronoDesk.Core.Abstractions;

public interface IAppLogger
{
    void Info(string eventName, string message);

    void Warning(string eventName, string message);

    void Error(string eventName, Exception exception, string safeMessage);
}
