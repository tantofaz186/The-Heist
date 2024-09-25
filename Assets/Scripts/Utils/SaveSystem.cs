using System.IO;
using CombatReportScripts;
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
            return new Settings();
        }

        public static void SaveCombatReport(CombatReportData data)
        {
            File.WriteAllText(Application.persistentDataPath + "/combatReport.json", JsonUtility.ToJson(data));
        }

        public static CombatReportData LoadCombatReport()
        {
            if (File.Exists(Application.persistentDataPath + "/combatReport.json"))
            {
                CombatReportData data =
                    JsonUtility.FromJson<CombatReportData>(File.ReadAllText(Application.persistentDataPath + "/combatReport.json"));
                // File.Delete(Application.persistentDataPath + "/combatReport.json");
                return data;
            }
            Debug.LogError("CombatReport file not found");
            return new CombatReportData();
        }

        private static Settings settings;

        public static Settings Settings
        {
            get { return settings ??= LoadSettings(); }
        }
    }
}