using System.IO;
using UI;
using UnityEngine;

namespace Utils
{
    public static class SaveSystem
    {
        public static void SaveSettings(Settings _settings)
        {
            File.WriteAllText(Application.persistentDataPath + "/settings.json", JsonUtility.ToJson(_settings));
        }

        public static Settings LoadSettings()
        {
            if (File.Exists(Application.persistentDataPath + "/settings.json"))
            {
                Settings _settings = JsonUtility.FromJson<Settings>(File.ReadAllText(Application.persistentDataPath + "/settings.json"));
                Debug.Log(_settings);
                if (_settings != null)
                {
                    return _settings;
                }

                File.Delete(Application.persistentDataPath + "/settings.json");
            }

            Debug.LogWarning("Settings file not found");
            return new Settings(1);
        }

        private static Settings settings;

        public static Settings Settings
        {
            get { return settings ??= LoadSettings(); }
        }
    }
}