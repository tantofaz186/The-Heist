using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToDoListManager : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;
    [SerializeField] private Button lockerButton;
    [SerializeField] private Button shopButton;
    [SerializeField] private Button configButton;
    [SerializeField] private Button quitButton;


    private void Awake()
    {
        hostButton.onClick.AddListener(OnButtonHostClick);
        joinButton.onClick.AddListener(OnButtonJoinClick);
        lockerButton.onClick.AddListener(OnButtonLockerClick);
        shopButton.onClick.AddListener(OnButtonShopClick);
        configButton.onClick.AddListener(OnButtonConfigClick);
        quitButton.onClick.AddListener(OnButtonQuitClick);
    }

    private void OnButtonHostClick()
    {
        Debug.Log("Host");
    }
    private void OnButtonJoinClick()
    {
        Debug.Log("Join");
    }
    private void OnButtonLockerClick()
    {
        Debug.Log("Locker");
    }
    private void OnButtonShopClick()
    {
        Debug.Log("Shop");
    }
    private void OnButtonConfigClick()
    {
        Debug.Log("Config");
    }
    private void OnButtonQuitClick()
    {
        //Are you sure(bool) e se sim application.quit;
        Debug.Log("Quit");
        Application.Quit();
    }
}
