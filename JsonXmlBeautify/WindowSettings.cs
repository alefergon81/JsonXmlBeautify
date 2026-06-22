using System;
using System.IO;
using System.Text.Json;

namespace JsonXmlBeautify
{
    /// <summary>
    /// Persisted window placement: position, size and whether the window was
    /// maximized. Stored as JSON under %AppData%\JsonXmlBeautify\window.json.
    /// All file I/O is best-effort — a missing or corrupt file simply yields
    /// no saved state, never an exception that blocks startup or shutdown.
    /// </summary>
    internal sealed class WindowSettings
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool Maximized { get; set; }

        private static string SettingsPath
        {
            get
            {
                string dir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "JsonXmlBeautify");
                return Path.Combine(dir, "window.json");
            }
        }

        /// <summary>Returns the saved settings, or null if none exist / can't be read.</summary>
        public static WindowSettings? Load()
        {
            try
            {
                string path = SettingsPath;
                if (!File.Exists(path))
                    return null;

                string json = File.ReadAllText(path);
                return JsonSerializer.Deserialize<WindowSettings>(json);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>Writes the settings to disk, swallowing any I/O failure.</summary>
        public void Save()
        {
            try
            {
                string path = SettingsPath;
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                File.WriteAllText(path, JsonSerializer.Serialize(this));
            }
            catch
            {
                // Persisting placement is non-critical; ignore failures.
            }
        }
    }
}
