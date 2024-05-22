using System;
using UnityEngine;
using Unity.AI.Navigation;
public class NavMeshBake : MonoBehaviour
{   
    public NavMeshSurface surface;

    private void Awake()
    {
        surface = GetComponent<NavMeshSurface>();
    }

    public void Bake()
    {   

        surface.BuildNavMesh();
        Debug.Log("Baked");
    }
   
   
}
