using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Audio;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject OptionsMenuPanel;

    [SerializeField]
    private GameObject GraphicsPanel;

    [SerializeField]
    private GameObject SoundPanel;

    [SerializeField]
    private GameObject ControlsPanel;

    [SerializeField]
    private GameObject QuitPanel;

    private bool menuOpen = false;
    public Slider brightnessSlider;
    public Slider MasterVolumeSlider;
    public Slider MusicSlider;
    public Slider SfxSlider;
    public AudioMixer audioMixer;
    public TMP_Dropdown resolutionDropdown;
    private Resolution[] resolutions;

    public static OptionsMenu instance;

    PlayerInputActions inputActions;

    void Start()
    {
        GetResolutions();
        MasterVolumeSlider.value = audioMixer.GetFloat("MasterVolume", out float value) ? Mathf.Pow(10, value / 20) : 0;
        MusicSlider.value = audioMixer.GetFloat("MusicVolume", out float val) ? Mathf.Pow(10, val / 20) : 0;
        SfxSlider.value = audioMixer.GetFloat("SfxVolume", out float valu) ? Mathf.Pow(10, valu / 20) : 0;
        brightnessSlider.value = RenderSettings.ambientIntensity;
        OptionsMenuPanel.SetActive(false);
    }

    private void OnEnable()
    {
        inputActions = new PlayerInputActions();
        inputActions.Enable();
        inputActions.Player.Menu.performed += OnPause;
    }

    private void OnDisable()
    {
        inputActions.Player.Menu.performed -= OnPause;
        inputActions.Disable();
    }

    private void OnPause(InputAction.CallbackContext obj)
    {
        OnPause();
    }

    public void OnPause()
    {
        if (menuOpen)
        {
            OptionsMenuPanel.SetActive(false);
            menuOpen = false;
            if (SceneManager.GetActiveScene().name == "GameScene")
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
        else
        {
            OptionsMenuPanel.SetActive(true);
            menuOpen = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    void GetResolutions()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height + " : " + resolutions[i].refreshRateRatio;
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
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(MasterVolumeSlider.value) * 20);
    }

    public void SetMusicVolume()
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(MusicSlider.value) * 20);
    }

    public void SetSfxVolume()
    {
        audioMixer.SetFloat("SfxVolume", Mathf.Log10(SfxSlider.value) * 20);
    }

    public void Sound()
    {
        GraphicsPanel.SetActive(false);
        SoundPanel.SetActive(true);
        ControlsPanel.SetActive(false);
        QuitPanel.SetActive(false);
    }

    public void Graphics()
    {
        GraphicsPanel.SetActive(true);
        SoundPanel.SetActive(false);
        ControlsPanel.SetActive(false);
        QuitPanel.SetActive(false);
    }

    public void Controls()
    {
        GraphicsPanel.SetActive(false);
        SoundPanel.SetActive(false);
        ControlsPanel.SetActive(true);
        QuitPanel.SetActive(false);
    }

    public void QuitMenu()
    {
        GraphicsPanel.SetActive(false);
        SoundPanel.SetActive(false);
        ControlsPanel.SetActive(false);
        QuitPanel.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }
}