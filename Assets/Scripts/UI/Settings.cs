using System.Collections.Generic;
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

        [SerializeField]
        [JsonProperty]
        public List<Keybind> keybinds;

        public Settings(float value, List<Keybind> keybinds)
        {
            mouseSensitivity = value;
            this.keybinds = keybinds;
        }

        public Settings()
        {
            mouseSensitivity = 1;
            keybinds = new List<Keybind>();
        }
    }

    [System.Serializable]
    [JsonObject]
    public class Keybind
    {
        [SerializeField]
        [JsonProperty]
        public string actionMap;

        [SerializeField]
        [JsonProperty]
        public string actionName;

        [SerializeField]
        [JsonProperty]
        public string index;

        [SerializeField]
        [JsonProperty]
        public string key;

        public Keybind(string actionMap, string actionName, string index, string key)
        {
            this.actionMap = actionMap;
            this.actionName = actionName;
            this.index = index;
            this.key = key;
        }
    }
}