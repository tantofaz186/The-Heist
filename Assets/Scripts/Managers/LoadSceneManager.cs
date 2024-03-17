using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneManager : MonoBehaviour
{
    //Esse script é utilizado para controlar o load das cenas no jogo (Todas as cenas)
    //private int sceneIndexToLoad = 1;
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
