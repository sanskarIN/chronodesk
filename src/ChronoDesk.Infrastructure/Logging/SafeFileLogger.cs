using System.Text.Json;
using System.Text.RegularExpressions;
using ChronoDesk.Core.Abstractions;

namespace ChronoDesk.Infrastructure.Logging;

public sealed class SafeFileLogger : IAppLogger
{
    private const long MaxLogBytes = 1_048_576;
    private static readonly Regex EmailRegex = new(
        @"\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
    private static readonly Regex SecretRegex = new(
        @"(?i)\b(token|secret|password|authorization|api[-_]?key)\s*[:=]\s*[^\s,;]+",
        RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private readonly object sync = new();
    private readonly string logPath;

    public SafeFileLogger(string? logDirectory = null)
    {
        logPath = logDirectory is null
            ? AppPaths.GetLogPath()
            : Path.Combine(logDirectory, "chronodesk.log.jsonl");
    }

    public void Info(string eventName, string message) => Write("info", eventName, message, null);

    public void Warning(string eventName, string message) => Write("warning", eventName, message, null);

    public void Error(string eventName, Exception exception, string safeMessage)
    {
        ArgumentNullException.ThrowIfNull(exception);
        Write("error", eventName, safeMessage, exception.GetType().Name);
    }

    private void Write(string level, string eventName, string message, string? exceptionType)
    {
        try
        {
            var safeEvent = Sanitize(eventName, 80);
            var safeMessage = Sanitize(message, 500);
            var entry = new
            {
                timestampUtc = DateTimeOffset.UtcNow,
                level,
                @event = safeEvent,
                message = safeMessage,
                exceptionType,
            };

            lock (sync)
            {
                var directory = Path.GetDirectoryName(logPath)
                    ?? throw new InvalidOperationException("ChronoDesk log directory could not be resolved.");
                Directory.CreateDirectory(directory);
                RotateIfNeeded(logPath);
                File.AppendAllText(logPath, JsonSerializer.Serialize(entry) + Environment.NewLine);
            }
        }
        catch (IOException)
        {
            // Logging must never crash the application.
        }
        catch (UnauthorizedAccessException)
        {
            // Logging must never crash the application.
        }
        catch (InvalidOperationException)
        {
            // Logging must never crash the application.
        }
    }

    private static string Sanitize(string? input, int maximumLength)
    {
        var value = string.IsNullOrWhiteSpace(input) ? "unspecified" : input.Trim();
        value = EmailRegex.Replace(value, "[redacted-email]");
        value = SecretRegex.Replace(value, match => $"{match.Groups[1].Value}=[redacted]");
        return value.Length <= maximumLength ? value : value[..maximumLength];
    }

    private static void RotateIfNeeded(string path)
    {
        if (!File.Exists(path) || new FileInfo(path).Length < MaxLogBytes)
        {
            return;
        }

        var archive = Path.Combine(
            Path.GetDirectoryName(path)!,
            $"chronodesk-{DateTimeOffset.UtcNow:yyyyMMdd-HHmmssfff}-{Guid.NewGuid():N}.log.jsonl");
        File.Move(path, archive, overwrite: false);
    }
}
