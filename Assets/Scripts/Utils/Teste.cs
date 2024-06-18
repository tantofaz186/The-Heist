using UnityEditor;
using UnityEngine;

public class Teste : SingletonPerPlayer<Teste>
{
    public void TesteMethod()
    {
        if (IsServer)
        {
            Debug.Log("Teste");
            Debug.Log(Players);
        }
        
        Debug.Log(Instance);
        Debug.Log(Instance.gameObject.name);
    }
    
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(Teste))]
    public class TesteEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Teste"))
            {
                Debug.Log("Teste UI");
                Teste teste = (target as Teste)!;
                teste.TesteMethod();
            }
        }
    }
    #endif
    
}