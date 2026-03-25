using UnityEngine;

public class OverworldManager : MonoBehaviour
{
	[SerializeField] private TerrainGenerator terrainGen;
	[SerializeField] private PlantGenerator plantGen;

	public int plantCount = 10;

	public void Start()
	{
		Random.InitState(GameManager.Instance.seed);

		int terrainWidth = 200, terrainDepth = 200;
		float cellSize = 1.0f;

		Mesh terrainMesh = terrainGen.GenerateMesh(terrainWidth, terrainDepth, cellSize);

		for(int i = 0; i < plantCount; i++)
		{
			float x = Random.Range(0f, terrainWidth);
			float z = Random.Range(0f, terrainDepth);
			float y = terrainGen.GetHeight(x, z);

			GameObject plant = plantGen.GeneratePlant();
			plant.transform.position = new Vector3(x, y, z);
			plant.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
			plant.transform.localScale = Vector3.one * Random.Range(0.5f, 1.5f);
		}
	}

	public void Update()
	{
		if(Input.GetKeyDown(KeyCode.P))
		{
			Utils.ChangeScene("Dungeon");
		}

		if(Input.GetKeyDown(KeyCode.M))
		{
			Utils.ChangeScene("MainMenu");
		}
	}
}