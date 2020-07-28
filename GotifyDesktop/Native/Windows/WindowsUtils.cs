using System;

namespace GotifyDesktop.Native.Windows
{
    /// <summary>
    /// <see cref="WindowsUtils"/> provides helper methods to invoke native operating system functionality when running on Windows.
    /// </summary>
    public class WindowsUtils
    {
        /// <summary>
        /// Gets a value indicating whether Windows prefers for user apps to be in a light color theme.
        /// </summary>
        /// <returns>True if light theme is preferred, false otherwise.</returns>
        /// <remarks>
        /// Adapted from https://stackoverflow.com/questions/44713412/how-can-i-get-whether-windows-10-anniversary-update-or-later-is-using-its-light.
        /// </remarks>
        public static bool IsAppLightThemePreferred()
        {
            bool result = true;
            try
            {
                var v = Microsoft.Win32.Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", "1");
                if (v != null && v.ToString() == "0")
                {
                    result = false;
                }
            }
            catch (Exception)
            {
            }

            return result;
        }
    }
}
