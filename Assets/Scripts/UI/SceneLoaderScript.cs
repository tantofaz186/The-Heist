using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneLoaderScript : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
