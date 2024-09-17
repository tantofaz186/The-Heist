using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Audio;
using TMPro;

public class OptionsMenu : MonoBehaviour
{   [SerializeField] private GameObject OptionsMenuPanel;
    [SerializeField] private GameObject GraphicsPanel;
    [SerializeField] private GameObject SoundPanel;
    [SerializeField] private GameObject ControlsPanel;
    private bool menuOpen = false;
    public Slider brightnessSlider;
    public Slider MasterVolumeSlider;
    public Slider MusicSlider;
    public Slider SfxSlider;
    public AudioMixer audioMixer;
    public TMP_Dropdown resolutionDropdown;
    private Resolution[] resolutions;
    void Start()
    {   
        GetResolutions();
        MasterVolumeSlider.value = audioMixer.GetFloat("MasterVolume", out float value) ? value : 0;
        MusicSlider.value = audioMixer.GetFloat("MusicVolume", out float val) ? val : 0;
        SfxSlider.value = audioMixer.GetFloat("SfxVolume", out float valu) ? valu : 0;
        brightnessSlider.value = RenderSettings.ambientIntensity;
        OptionsMenuPanel.SetActive(false);
    }

    void GetResolutions()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);
            
            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }
    
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
    
    public void Brightness()
    {
        RenderSettings.ambientIntensity = brightnessSlider.value;
    }
    
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }
    
    public void SetMasterVolume()
    {
        audioMixer.SetFloat("MasterVolume", MasterVolumeSlider.value);
    }
    public void SetMusicVolume()
    {
        audioMixer.SetFloat("MusicVolume", MusicSlider.value);
    }
    public void SetSfxVolume()
    {
        audioMixer.SetFloat("SfxVolume", SfxSlider.value);
    }

    public void Sound()
    {
        GraphicsPanel.SetActive(false);
        SoundPanel.SetActive(true);
        ControlsPanel.SetActive(false);
    }
   public void Graphics()
    {
        GraphicsPanel.SetActive(true);
        SoundPanel.SetActive(false);
        ControlsPanel.SetActive(false);
    }
   public void Controls()
    {
        GraphicsPanel.SetActive(false);
        SoundPanel.SetActive(false);
        ControlsPanel.SetActive(true);
    }

    public void OnPause()
    {
        if (menuOpen)
        {
            OptionsMenuPanel.SetActive(false);
            menuOpen = false;
        }
        else
        {
            OptionsMenuPanel.SetActive(true);
            menuOpen = true;
        }
    }

    
}
