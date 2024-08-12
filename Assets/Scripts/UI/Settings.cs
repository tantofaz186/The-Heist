using Newtonsoft.Json;
using UnityEngine;

namespace UI
{
    [System.Serializable]
    [JsonObject]
    public class Settings
    {
        [SerializeField]
        [JsonProperty]
        public float mouseSensitivity;

        public Settings (float value)
        {
            mouseSensitivity = value;
        }
        public Settings()
        {
            mouseSensitivity = 1;
        }
    }
}