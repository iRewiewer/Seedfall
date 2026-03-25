using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class TerrainGenerator : MonoBehaviour
{
	[SerializeField] private float noiseScale = 0.1f;
	[SerializeField] private float heightMultiplier = 30f;

	[SerializeField] private float flatRadius = 20f;
	[SerializeField] private float blendRadius = 60f;

	public Mesh GenerateMesh(int width, int depth, float cellSize)
	{
		Vector3[] vertices = new Vector3[(width + 1) * (depth + 1)];
		int[] triangleIndices = new int[width * depth * 6];

		int vertexIndex = 0;
		for (int z = 0; z <= depth; z++)
		{
			for (int x = 0; x <= width; x++)
			{
				int seed = GameManager.Instance.seed % 10000; // used as offset
				float y = Mathf.PerlinNoise((x + seed) * noiseScale, (z + seed) * noiseScale) * heightMultiplier;

				float centerX = width * cellSize * 0.5f;
				float centerZ = depth * cellSize * 0.5f;

				float worldX = x * cellSize;
				float worldZ = z * cellSize;

				float distanceToCenter= Vector2.Distance(new Vector2(worldX, worldZ), new Vector2(centerX, centerZ));

				float falloff = Mathf.InverseLerp(flatRadius, flatRadius + blendRadius, distanceToCenter);

				y *= falloff;

				vertices[vertexIndex] = new Vector3(x * cellSize, y, z * cellSize);
				vertexIndex++;
			}
		}

		int triangleIndex = 0;
		int rowLength = width + 1;

		for (int z = 0; z < depth; z++)
		{
			for (int x = 0; x < width; x++)
			{
				int topLeft = z * rowLength + x;
				int bottomLeft = (z + 1) * rowLength + x;

				triangleIndices[triangleIndex++] = topLeft;
				triangleIndices[triangleIndex++] = bottomLeft;
				triangleIndices[triangleIndex++] = topLeft + 1;

				triangleIndices[triangleIndex++] = topLeft + 1;
				triangleIndices[triangleIndex++] = bottomLeft;
				triangleIndices[triangleIndex++] = bottomLeft + 1;
			}
		}

		Mesh mesh = new Mesh()
		{
			name = "ProceduralTerrain",
			vertices = vertices,
			triangles = triangleIndices
		};

		mesh.RecalculateNormals();

		GetComponent<MeshFilter>().mesh = mesh;
		GetComponent<MeshCollider>().sharedMesh = mesh;

		this.gameObject.layer = LayerMask.NameToLayer("Ground");

		return mesh;
	}

	public float GetHeight(float x, float z)
	{
		float noiseX = x * noiseScale;
		float noiseZ = z * noiseScale;

		return Mathf.PerlinNoise(noiseX, noiseZ) * heightMultiplier;
	}
}