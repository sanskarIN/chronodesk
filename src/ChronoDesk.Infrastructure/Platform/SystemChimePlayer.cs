using System.Diagnostics;
using System.Runtime.Versioning;
using ChronoDesk.Core.Abstractions;

namespace ChronoDesk.Infrastructure.Platform;

public sealed class SystemChimePlayer : IChimePlayer
{
    public async Task PlayAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (OperatingSystem.IsWindows())
        {
            await PlayWindowsAsync(cancellationToken);
            return;
        }

        if (OperatingSystem.IsMacOS())
        {
            await TryRunAsync(
                "/usr/bin/afplay",
                ["/System/Library/Sounds/Glass.aiff"],
                cancellationToken);
            return;
        }

        if (OperatingSystem.IsLinux())
        {
            if (await TryRunAsync(
                    "/usr/bin/canberra-gtk-play",
                    ["--id", "message"],
                    cancellationToken))
            {
                return;
            }

            if (await TryRunAsync(
                    "/usr/bin/paplay",
                    ["/usr/share/sounds/freedesktop/stereo/message.oga"],
                    cancellationToken))
            {
                return;
            }

            await TryRunAsync(
                "/usr/bin/aplay",
                ["/usr/share/sounds/alsa/Front_Center.wav"],
                cancellationToken);
        }
    }

    [SupportedOSPlatform("windows")]
    private static Task PlayWindowsAsync(CancellationToken cancellationToken) =>
        Task.Run(
            () =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                Console.Beep(880, 110);
                Console.Beep(1047, 160);
            },
            cancellationToken);

    private static async Task<bool> TryRunAsync(
        string executable,
        IReadOnlyList<string> arguments,
        CancellationToken cancellationToken)
    {
        if (!File.Exists(executable))
        {
            return false;
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = executable,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        foreach (var argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        try
        {
            using var process = Process.Start(startInfo);
            if (process is null)
            {
                return false;
            }

            await process.WaitForExitAsync(cancellationToken);
            return process.ExitCode == 0;
        }
        catch (Exception exception) when (exception is InvalidOperationException or IOException)
        {
            return false;
        }
    }
}
