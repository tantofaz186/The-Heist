using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button controlsButton;
    [SerializeField] private Button featuresButton;
    [SerializeField] private Button backButtonControls;
    [SerializeField] private Button backButtonFeatures;
    [SerializeField] private Button quitButton;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject featuresPanel;

    private void Awake()
    {
        playButton.onClick.AddListener(() => 
        { 
            Loader.Load(Loader.Scene.LobbyScene);
        });
        controlsButton.onClick.AddListener(() => 
        { 
            controlsPanel.SetActive(true);
        });
        featuresButton.onClick.AddListener(() => 
        { 
            featuresPanel.SetActive(true);
        });
        backButtonControls.onClick.AddListener(() => 
        { 
            controlsPanel.SetActive(false);
        });
        backButtonFeatures.onClick.AddListener(() => 
        { 
            featuresPanel.SetActive(false);
        });
        quitButton.onClick.AddListener(() => 
        { 
            Application.Quit();
        });
    }

    private void Start()
    {
        controlsPanel.SetActive(false);
    }
}
