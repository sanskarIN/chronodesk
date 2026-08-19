using System.ComponentModel;
using System.Diagnostics;

namespace ChronoDesk.App.Services;

public static class ExternalUriLauncher
{
    public static bool IsAllowed(string value) => TryCreateAllowedUri(value, out _);

    public static bool TryOpen(string value)
    {
        if (!TryCreateAllowedUri(value, out var uri) || uri is null)
        {
            return false;
        }

        try
        {
            Process.Start(new ProcessStartInfo(uri.AbsoluteUri) { UseShellExecute = true });
            return true;
        }
        catch (Exception exception) when (
            exception is Win32Exception
                or IOException
                or InvalidOperationException
                or NotSupportedException)
        {
            return false;
        }
    }

    private static bool TryCreateAllowedUri(string value, out Uri? uri)
    {
        uri = null;
        if (string.IsNullOrWhiteSpace(value)
            || !Uri.TryCreate(value, UriKind.Absolute, out var candidate))
        {
            return false;
        }

        if (candidate.Scheme == Uri.UriSchemeHttps)
        {
            if (string.IsNullOrWhiteSpace(candidate.Host)
                || !string.IsNullOrEmpty(candidate.UserInfo))
            {
                return false;
            }

            uri = candidate;
            return true;
        }

        if (candidate.Scheme == Uri.UriSchemeMailto
            && !string.IsNullOrWhiteSpace(candidate.OriginalString["mailto:".Length..]))
        {
            uri = candidate;
            return true;
        }

        return false;
    }
}
