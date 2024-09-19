using System;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI
{
    public class SettingsPanel : MonoBehaviour
    {
        [SerializeField]
        private Settings settings;

        public Settings Settings => settings;

        [SerializeField]
        Slider mouseSensitivitySlider;

        [SerializeField]
        Button backButton;

        private void Start()
        {
            LoadSettings();
            mouseSensitivitySlider.onValueChanged.AddListener(OnMouseSensitivityChanged);
            backButton.onClick.AddListener(SaveSettings);
        }


        private void OnMouseSensitivityChanged(float arg0)
        {
            settings.mouseSensitivity = Math.Clamp(arg0, 0.01f, 1f);
        }

        public void LoadSettings()
        {
            settings = SaveSystem.Settings?? new Settings();
            mouseSensitivitySlider.SetValueWithoutNotify(Math.Clamp(settings.mouseSensitivity, 0.01f, 1f));
        }

        public void SaveSettings()
        {
            SaveSystem.SaveSettings(settings);
        }
    }
}