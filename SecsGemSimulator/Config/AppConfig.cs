using System;
using System.IO;
using System.Text.Json;

namespace SecsGemSimulator.Config
{
    public class AppConfig
    {
        public string IpAddress { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 5000;
        public bool IsActiveMode { get; set; } = false; // True for Active (Client), False for Passive (Server)
        public ushort DeviceId { get; set; } = 0;
        public int T3Timeout { get; set; } = 45; // Seconds
        public int T5Timeout { get; set; } = 10;
        public int T6Timeout { get; set; } = 5;
        public int T7Timeout { get; set; } = 10;
        public int LinkTestInterval { get; set; } = 60; // Seconds

        private static string ConfigPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

        public static AppConfig Load()
        {
            string path = ConfigPath;
            if (File.Exists(path))
            {
                try
                {
                    string json = File.ReadAllText(path);
                    return JsonSerializer.Deserialize<AppConfig>(json) ?? new AppConfig();
                }
                catch
                {
                    return new AppConfig();
                }
            }
            return new AppConfig();
        }

        public void Save()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(this, options);
            File.WriteAllText(ConfigPath, json);
        }
    }
}
