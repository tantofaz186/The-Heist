using System.Collections.Generic;
using UnityEngine;

public class FOV : MonoBehaviour
{
    public float viewAngle;
    public float viewRadius;
    public float viewHeight;
    public LayerMask playerMask;
    public LayerMask obstacleMask;

    public Color meshColor = Color.red;

    private Mesh mesh;

    private Collider[] colliders = new Collider[50];
    public float scanInterval;
    private float scanTimer;
    public int scanFrequency = 30;
    private int count;

    const float TimeToLosePlayer = 1.5f;
    private float timeToLosePlayer;

    public List<GameObject> Objects => objects;

    [SerializeField]
    private List<GameObject> objects = new();

    private void Awake()
    {
        scanInterval = 1.0f / scanFrequency;
        timeToLosePlayer = TimeToLosePlayer;
    }
    
    public void ClearObjects()
    {
        objects.Clear();
    }

    public void Scan()
    {
        timeToLosePlayer -= Time.deltaTime;
        if (timeToLosePlayer <= 0)
        {
            ClearObjects();
            timeToLosePlayer = TimeToLosePlayer;
        }

        count = Physics.OverlapSphereNonAlloc(transform.position, viewRadius, colliders, playerMask, QueryTriggerInteraction.Collide);
        for (int i = 0; i < count; i++)
        {
            GameObject obj = colliders[i].GetComponentInChildren<SkinnedMeshRenderer>().gameObject;
            if (InSight(obj))
            {
                if (!objects.Contains(obj))
                {
                    objects.Add(obj);
                }
            }
        }
    }

    public bool InSight(GameObject obj)
    {
        Vector3 origin = transform.position;
        origin.y -= 1f;
        Vector3 dest = obj.transform.position;
        Vector3 direction = dest - origin;
        if (Mathf.Abs(direction.y) > viewHeight)
        {
            return false;
        }

        direction.y = 0;
        float deltaAngle = Vector3.Angle(direction, transform.forward);
        if (deltaAngle > viewAngle)
        {
            return false;
        }

        origin.y += viewHeight / 2;
        dest.y = origin.y;
        return !Physics.Linecast(origin, dest, obstacleMask);
    }

    #if UNITY_EDITOR

    Mesh CreateWedgeMesh() //Criar triangulo
    {
        Mesh mesh = new Mesh();

        int segments = 10;
        int numTriangles = (segments * 4) + 2 + 2;
        int numVertices = numTriangles * 3;
        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[numVertices];
        Vector3 bottomCenter = Vector3.zero;
        Vector3 bottomLeft = Quaternion.Euler(0, -viewAngle, 0) * Vector3.forward * viewRadius;
        Vector3 bottomRight = Quaternion.Euler(0, viewAngle, 0) * Vector3.forward * viewRadius;

        Vector3 topCenter = bottomCenter + Vector3.up * viewHeight;
        Vector3 topLeft = bottomLeft + Vector3.up * viewHeight;
        Vector3 topRight = bottomRight + Vector3.up * viewHeight;

        int vert = 0;
        //left side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = bottomLeft;
        vertices[vert++] = topLeft;

        vertices[vert++] = topLeft;
        vertices[vert++] = topCenter;
        vertices[vert++] = bottomCenter;
        //right side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = topCenter;
        vertices[vert++] = topRight;

        vertices[vert++] = topRight;
        vertices[vert++] = bottomRight;
        vertices[vert++] = bottomCenter;

        float currentAngle = -viewAngle;
        float deltaAngle = (viewAngle * 2) / segments;
        for (int i = 0; i < segments; i++)
        {
            bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * viewRadius;
            bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * viewRadius;

            topRight = bottomRight + Vector3.up * viewHeight;
            topLeft = bottomLeft + Vector3.up * viewHeight;

            //far side
            vertices[vert++] = bottomLeft;
            vertices[vert++] = bottomRight;
            vertices[vert++] = topRight;

            vertices[vert++] = topRight;
            vertices[vert++] = topLeft;
            vertices[vert++] = bottomLeft;
            //top side
            vertices[vert++] = topCenter;
            vertices[vert++] = topLeft;
            vertices[vert++] = topRight;

            //bottom side
            vertices[vert++] = bottomCenter;
            vertices[vert++] = bottomRight;
            vertices[vert++] = bottomLeft;

            currentAngle += deltaAngle;
        }


        for (int i = 0; i < vertices.Length; i++)
        {
            triangles[i] = i;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        return mesh;
    }

    private void OnValidate()
    {
        mesh = CreateWedgeMesh();
        scanInterval = 1.0f / scanFrequency;
    }

    private void OnDrawGizmos()
    {
        if (mesh)
        {
            Gizmos.color = meshColor;
            Gizmos.DrawMesh(mesh, transform.position, transform.rotation);
        }


        Gizmos.color = Color.green;
        foreach (var obj in Objects)
        {
            Gizmos.DrawSphere(obj.transform.position, 0.2f);
        }
    }
    #endif
}