using System.Security;

namespace ChronoDesk.Infrastructure.Platform;

internal static class MacLaunchAgentPlist
{
    internal static string Create(string executablePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(executablePath);

        var escapedExecutable = SecurityElement.Escape(executablePath) ?? executablePath;
        return $"""
            <?xml version="1.0" encoding="UTF-8"?>
            <!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
            <plist version="1.0">
              <dict>
                <key>Label</key>
                <string>com.sanskar.chronodesk</string>
                <key>ProgramArguments</key>
                <array>
                  <string>{escapedExecutable}</string>
                  <string>--background</string>
                </array>
                <key>RunAtLoad</key>
                <true/>
              </dict>
            </plist>
            """;
    }
}
