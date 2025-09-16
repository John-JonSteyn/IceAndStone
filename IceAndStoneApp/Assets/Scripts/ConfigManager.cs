using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace IceAndStone.App.Config
{
    public static class ConfigManager
    {
        public static Config Current { get; private set; } = new();

        private static string ConfigPath =>
#if UNITY_EDITOR
            Path.Combine(Application.dataPath, "config.json");
#else
            Path.Combine(Application.persistentDataPath, "config.json");
#endif

        public static async Task<Config> StartupAsync()
        {
            if (File.Exists(ConfigPath))
            {
                var json = await File.ReadAllTextAsync(ConfigPath);
                Current = JsonConvert.DeserializeObject<Config>(json) ?? new Config();
            }
            else
            {
                Current = new Config
                {
                    ApiBaseUrl = "http://localhost:8080",
                    VenueId = 1,
                    LaneId = 1,
                    AppVersion = "dev"
                };
                var json = JsonConvert.SerializeObject(Current, Formatting.Indented);
                await File.WriteAllTextAsync(ConfigPath, json);
            }

#if UNITY_EDITOR
            var versionFile = Path.Combine(Application.dataPath, "app-version.json");
            if (File.Exists(versionFile))
            {
                var versionJson = await File.ReadAllTextAsync(versionFile);
                var parsed = JsonConvert.DeserializeObject<ConfigVersion>(versionJson);
                if (!string.IsNullOrWhiteSpace(parsed?.Version))
                {
                    Current.AppVersion = parsed.Version;
                }
            }
#endif

            return Current;
        }

        private class ConfigVersion { public string Version { get; set; } = ""; }
    }
}
