using System.ComponentModel;
using System.Diagnostics;

namespace ChronoDesk.App;

internal static class ExternalLinkLauncher
{
    public static bool TryOpen(string value)
    {
        if (!TryGetAllowedUri(value, out var uri))
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
                or InvalidOperationException
                or PlatformNotSupportedException)
        {
            return false;
        }
    }

    internal static bool TryGetAllowedUri(string value, out Uri uri)
    {
        if (Uri.TryCreate(value, UriKind.Absolute, out var candidate)
            && (candidate.Scheme == Uri.UriSchemeHttps || candidate.Scheme == Uri.UriSchemeMailto))
        {
            uri = candidate;
            return true;
        }

        uri = null!;
        return false;
    }
}
