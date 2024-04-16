using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEditor.AI;

public class NavMeshBake : MonoBehaviour
{   
   public void Bake()
    {   
        NavMeshBuilder.BuildNavMesh();
        Debug.Log("Baked");
    }
   
   
}
