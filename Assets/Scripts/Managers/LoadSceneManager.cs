using UnityEngine.SceneManagement;

public class LoadSceneManager : MonoBehaviourSingletonPersistent<LoadSceneManager>
{
    //Esse script é utilizado para controlar o load das cenas no jogo (Todas as cenas)
    //private int sceneIndexToLoad = 1;

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}