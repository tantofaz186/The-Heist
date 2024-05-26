using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button controlsButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private GameObject controlsPanel;

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
        backButton.onClick.AddListener(() => 
        { 
            controlsPanel.SetActive(false);
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
