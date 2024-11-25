using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    // [SerializeField] private SkinnedMeshRenderer headMeshRenderer;
    // [SerializeField] private SkinnedMeshRenderer bodyMeshRenderer;

    [SerializeField] private SkinnedMeshRenderer[] bodyparts;

    public void SetPlayerColor(Material color)
    {
        foreach (var bodypart in bodyparts)
        {
            bodypart.material = color;
        }
        
    }
}
