using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{
    public enum Scene
    {
        MainMenuScene,
        GameScene,
        LoadingScene,
        LobbyScene,
        CharacterSelectScene,
        CombatReportScene
    }

    private static Scene targetScene;

    public static void Load(Scene targetScene)
    {
        Loader.targetScene = targetScene;

        SceneManager.LoadScene(targetScene.ToString());
    }

    public static void LoadNetwork(Scene targetScene)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(), LoadSceneMode.Single);
    }

    public static void LoadCombatReportScene()
    {
        SceneManager.LoadScene(Scene.CombatReportScene.ToString());
        SceneManager.sceneLoaded += LoadCombatReportScene_Multiplayer;
    }

    private static void LoadCombatReportScene_Multiplayer(UnityEngine.SceneManagement.Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.name == Scene.CombatReportScene.ToString())
        {
            SceneManager.sceneLoaded -= LoadCombatReportScene_Multiplayer;
            NetworkManager.Singleton.SceneManager.LoadScene(Scene.CombatReportScene.ToString(), LoadSceneMode.Single);
        }
    }

    public static void LoaderCallback()
    {
        SceneManager.LoadScene(targetScene.ToString());
    }
}