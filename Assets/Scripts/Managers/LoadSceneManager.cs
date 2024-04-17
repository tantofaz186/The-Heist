using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
public class LoadSceneManager : MonoBehaviourSingletonPersistent<LoadSceneManager>
{
    //Esse script é utilizado para controlar o load das cenas no jogo (Todas as cenas)
    //private int sceneIndexToLoad = 1;

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        //Transition.LoadLevel("MainMenu", 0.5f, Color.black);
    }

    #if UNITY_EDITOR
    public List<SceneAsset> scenes = new List<SceneAsset>();

    public static List<SceneAsset> GetScenes()
    {
        return Instance.scenes;
    }

    public static void SetScenes(List<SceneAsset> scenes)
    {
        Instance.scenes = scenes;
    }
    [CustomEditor(typeof(LoadSceneManager))]
    public class LoadSceneManagerEditor : Editor
    {
        private List<SceneAsset> scenes = GetScenes();
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Load Main Menu"))
            {
                LoadSceneManager manager = (target as LoadSceneManager)!;
                manager.LoadMainMenu();
            }

            if (GUILayout.Button("RegisterScenes"))
            {
                LoadSceneManager manager = (target as LoadSceneManager)!;
                manager.scenes = GetAllInstances<SceneAsset>().ToList();
            }

            if (GUILayout.Button("ResetInstance"))
            {
                Instance.scenes = new List<SceneAsset>();
            }

            if (GUILayout.Button("LoadMenu()"))
            {
                EditorSceneManager.SaveOpenScenes();
                EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
            }

            
            for (int i = 0; i < scenes.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(scenes[i].name);
                if (GUILayout.Button($"Load {scenes[i].name}"))
                {
                    var allScenes = AssetDatabase.FindAssets("t:Scene");
                    foreach (string sceneGUID in allScenes)
                    {
                        var assetPath = AssetDatabase.GUIDToAssetPath(sceneGUID);
                        Debug.Log(sceneGUID);
                        Debug.Log(assetPath);
                        Debug.Log(scenes[i].name);
                        string sceneName = Regex.Replace(assetPath , @"^.*\/*(.*?)\.unity$", "$1");
                        Debug.Log(sceneName);
                        if (sceneName == scenes[i].name)
                        {
                            // ReSharper disable once AccessToStaticMemberViaDerivedType
                            // Tirar o comentário acima pode induzir a erro,
                            // a unity pede especificamente para chamar o tipo derivado EditorSceneManager
                            EditorSceneManager.SaveOpenScenes();
                            // Load the given scene
                            
                            EditorSceneManager.OpenScene(sceneGUID, OpenSceneMode.Single);
                        }
                    }
                }

                GUILayout.EndHorizontal();
            }
        }

        public IEnumerable<T> GetAllInstances<T>() where T : UnityEngine.Object
        {
            return AssetDatabase.FindAssets($"t: {typeof(T).Name}").ToList()
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<T>)
                .ToList();
        }
    }
    #endif
}