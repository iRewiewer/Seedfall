using UnityEngine;

public class OverworldManager : MonoBehaviour
{
	public void Start()
	{
		Random.InitState(GameManager.Instance.seed);

		TerrainGenerator terrainGen = GetComponent<TerrainGenerator>();

		Mesh terrainMesh = terrainGen.GenerateMesh();
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