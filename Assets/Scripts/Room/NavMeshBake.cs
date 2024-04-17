using UnityEngine;
using Unity.AI.Navigation;
#if UNITY_EDITOR
using NavMeshBuilder = UnityEditor.AI.NavMeshBuilder;
#endif
public class NavMeshBake : MonoBehaviour
{   
    public NavMeshSurface surface;
   public void Bake()
    {   
        #if UNITY_EDITOR
        NavMeshBuilder.BuildNavMesh();
        #else
        surface.BuildNavMesh();
        #endif
        Debug.Log("Baked");
    }
   
   
}
