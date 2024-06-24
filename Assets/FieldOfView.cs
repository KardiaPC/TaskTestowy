using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    private Mesh mesh;
    private Vector3 origin;
    private float startingAngle;

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        origin = transform.position;
    }

    private void Update() {
        UpdateMesh();
    }

    private void LateUpdate()
    {
        // Update the origin to the current position of the GameObject
        origin = transform.position;
        startingAngle = transform.eulerAngles.z;
    }

    private void UpdateMesh()
    {
        mesh.Clear();
        float fov = 360f;
        int rayCount = 240;
        float angle = startingAngle - fov / 2f;
        float angleIncrease = fov / rayCount;
        float viewDistance = 7f;

        Vector3[] vertices = new Vector3[rayCount + 2];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = Vector3.zero;

        int vertexIndex = 1;
        int triangleIndex = 0;
        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 vertex;
            RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, GetVectorFromAngle(angle), viewDistance, layerMask);
            if (raycastHit2D.collider == null)
            {
                // no hit
                vertex = origin + GetVectorFromAngle(angle) * viewDistance;
            }
            else
            {
                // hit
                vertex = raycastHit2D.point;
            }
            vertices[vertexIndex] = transform.InverseTransformPoint(vertex);

            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }

            vertexIndex++;
            angle += angleIncrease;
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 1000f);
    }

    public static Vector3 GetVectorFromAngle(float angle)
    {
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }
}