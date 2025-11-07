using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TriangleCreator : MonoBehaviour
{
    void Start()
    {
        Mesh mesh = new Mesh();

        // 3 đỉnh tạo tam giác vuông ở góc dưới trái
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(0, 0, 0),  // Đỉnh A
            new Vector3(0, 1, 0),  // Đỉnh B
            new Vector3(1, 0, 0),  // Đỉnh C (góc vuông ở A)
        };

        int[] triangles = new int[]
        {
            0, 1, 2 // thứ tự vẽ mặt tam giác (ABC)
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        GetComponent<MeshFilter>().mesh = mesh;
    }
}
