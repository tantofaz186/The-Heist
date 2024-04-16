using System;
using UnityEngine;

public class FOV : MonoBehaviour
{
    public float viewAngle;
    public float viewRadius;
    public float viewHeight;
    public LayerMask playerMask;
    public LayerMask obstacleMask;
    public bool playerInSight;

    public Color meshColor = Color.red;

    private Mesh mesh;


    Mesh CreateWedgeMesh()  //Criar triangulo
    {
        
        Mesh mesh = new Mesh();
        
        int segments = 10;
        int numTriangles = (segments*4)+2+2;
        int numVertices = numTriangles * 3;
        Vector3[] vertices = new Vector3[numVertices];
        int [] triangles = new int[numVertices];
        Vector3 bottomCenter = Vector3.zero;
        Vector3 bottomLeft = Quaternion.Euler(0,-viewAngle,0)*Vector3.forward*viewRadius;
        Vector3 bottomRight = Quaternion.Euler(0,viewAngle,0)*Vector3.forward*viewRadius;
        
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
        float deltaAngle = (viewAngle*2)/segments;
        for (int i = 0; i < segments; i++)
        {
            bottomLeft = Quaternion.Euler(0,currentAngle,0)*Vector3.forward*viewRadius;
            bottomRight = Quaternion.Euler(0,currentAngle+deltaAngle,0)*Vector3.forward*viewRadius;
            
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
        
       
        
        for(int i=0;i<vertices.Length;i++)
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
    }

    private void OnDrawGizmos()
    {
        if (mesh)
        {
            Gizmos.color = meshColor;
            Gizmos.DrawMesh(mesh, transform.position, transform.rotation);
        }
    }
}
