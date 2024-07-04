using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class AreaMesh : AreaMeshDrawerBase
{
    private Mesh CreateCircleMesh()
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[circleSegments + 1];
        int[] triangles = new int[circleSegments * 3];
        vertices[0] = Vector3.zero;

        float angle = 0f;
        for (int i = 1; i < circleSegments + 1; i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * _radius;
            float z = Mathf.Cos(Mathf.Deg2Rad * angle) * _radius;
            vertices[i] = new Vector3(x, 0, z);

            if (i < circleSegments)
            {
                triangles[(i - 1) * 3] = 0;
                triangles[(i - 1) * 3 + 1] = i;
                triangles[(i - 1) * 3 + 2] = i + 1;
            }
            else
            {
                triangles[(i - 1) * 3] = 0;
                triangles[(i - 1) * 3 + 1] = i;
                triangles[(i - 1) * 3 + 2] = 1;
            }

            angle += 360f / circleSegments;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        return mesh;
    }

    protected Mesh CircleAndAngleMesh()
    {
        Mesh mesh = new Mesh();
        int numVertices = circleSegments + 2;
        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[(circleSegments) * 3];

        vertices[0] = Vector3.zero;
        float currentAngle = -_angle / 2f;

        for (int i = 1; i < numVertices; i++)
        {
            float rad = Mathf.Deg2Rad * currentAngle;
            vertices[i] = new Vector3(Mathf.Sin(rad) * _radius, 0, Mathf.Cos(rad) * _radius);
            currentAngle += _angle / circleSegments;
        }

        for (int i = 0; i < circleSegments; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        return mesh;
    }

    protected Mesh DrawSomethingMesh()
    {
        Mesh mesh = new Mesh();

        // Create the arc
        float radians = Mathf.Deg2Rad * _angle;
        float halfRadians = radians / 2;
        int arcSegments = circleSegments;
        int verticesCount = arcSegments + 2; // Including the center and end points

        Vector3[] vertices = new Vector3[verticesCount + 2]; // Additional two points for vertical line
        int[] triangles = new int[arcSegments * 3 + 6]; // Additional two triangles for vertical line

        vertices[0] = Vector3.zero; // Center point

        for (int i = 0; i <= arcSegments; i++)
        {
            float t = i / (float)arcSegments;
            float currentAngle = Mathf.Lerp(-halfRadians, halfRadians, t);
            float x = Mathf.Sin(currentAngle) * _radius;
            float z = Mathf.Cos(currentAngle) * _radius;
            vertices[i + 1] = new Vector3(x, 0, z);
        }

        // Creating the arc triangles
        for (int i = 0; i < arcSegments; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        // Creating the line
        vertices[verticesCount] = Vector3.zero; // Bottom point of the line
        vertices[verticesCount + 1] = new Vector3(0, 0, _radius); // Top point of the line

        int lineIndex = arcSegments * 3;
        triangles[lineIndex] = verticesCount;
        triangles[lineIndex + 1] = verticesCount + 1;
        triangles[lineIndex + 2] = 0;

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    protected override Mesh DrawMesh()
    {
        Mesh mesh = new();
        // Create the arc
        float radians = Mathf.Deg2Rad * _angle;
        float halfRadians = radians / 2;
        int arcSegments = circleSegments;


        return mesh;
    }
}