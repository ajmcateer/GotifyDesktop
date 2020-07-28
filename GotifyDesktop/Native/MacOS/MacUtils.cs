using System;
using System.Diagnostics;

namespace GotifyDesktop.Native.MacOS
{
    /// <summary>
    /// <see cref="MacUtils"/> provides helper methods to invoke native operating system functionality when running on macOS.
    /// </summary>
    public class MacUtils
    {
        /// <summary>
        /// Gets a value indicating whether the user has the system dark theme enabled.
        /// </summary>
        /// <returns>True if dark theme is set, false otherwise.</returns>
        /// <remarks>
        /// Adapted from https://brettterpstra.com/2018/09/26/shell-tricks-toggling-dark-mode-from-terminal/.
        /// </remarks>
        public static bool IsDarkModeEnabled()
        {
            try
            {
                var result = RunAppleScriptCommand("tell app \"System Events\" to tell appearance preferences to get dark mode");

                if (result.StartsWith("true"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Executes an AppleScript command and returns any generated output.
        /// </summary>
        /// <param name="command">The AppleScript command to run.</param>
        /// <returns>The result from executing the given command.</returns>
        public static string RunAppleScriptCommand(string command)
        {
            var psInfo = new ProcessStartInfo("osascript");
            psInfo.ArgumentList.Add($"-e {command}");

            psInfo.RedirectStandardOutput = true;
            psInfo.RedirectStandardError = true;

            var p = Process.Start(psInfo);
            p.WaitForExit();
            var result = p.StandardOutput.ReadToEnd();

            return result;
        }
    }
}
