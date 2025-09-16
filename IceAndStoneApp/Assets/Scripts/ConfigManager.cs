using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace IceAndStone.App.Config
{
    public static class ConfigManager
    {
        private static AppConfig _current = new AppConfig();

        public static AppConfig Current => _current;

        public static string BaseFolder
        {
            get
            {
#if UNITY_EDITOR
                return Application.dataPath;
#else
                var path = Path.Combine(Application.persistentDataPath, "IceAndStone");
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                return path;
#endif
            }
        }

        public static string ConfigPath => Path.Combine(BaseFolder, "config.json");

#if UNITY_EDITOR
        public static string AppVersionPath => Path.Combine(Application.dataPath, "app-version.json");
#endif

        public static void Startup()
        {
            try
            {
                if (!File.Exists(ConfigPath))
                {
                    WriteConfig(_current);
                    Debug.Log($"[Config] Created default config at: {ConfigPath}");
                }

                var json = File.ReadAllText(ConfigPath);
                _current = JsonConvert.DeserializeObject<AppConfig>(json) ?? new AppConfig();

#if UNITY_EDITOR
                if (File.Exists(AppVersionPath))
                {
                    try
                    {
                        var versionText = File.ReadAllText(AppVersionPath);
                        var jsonObject = JObject.Parse(versionText);
                        var version = jsonObject["version"]?.ToString();

                        if (!string.IsNullOrWhiteSpace(version) && _current.AppVersion != version)
                        {
                            _current.AppVersion = version;
                            WriteConfig(_current);
                            Debug.Log($"[Config] Synced appVersion from app-version.json: {version}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"[Config] Failed to read app-version.json: {ex.Message}");
                    }
                }
#endif

                Debug.Log($"[Config] Loaded config (ApiBaseUrl={_current.ApiBaseUrl}, VenueId={_current.VenueId}, LaneId={_current.LaneId}, AppVersion={_current.AppVersion}) from {ConfigPath}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Config] Failed to load config: {ex}");
                _current = new AppConfig();
                WriteConfig(_current);
            }
        }

        private static void WriteConfig(AppConfig config)
        {
            var jsonOut = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText(ConfigPath, jsonOut);
        }
    }
}
