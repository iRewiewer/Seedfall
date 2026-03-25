using UnityEngine;

public class DungeonManager : MonoBehaviour
{
	public void Start()
	{

	}

	public void Update()
	{
		if(Input.GetKeyDown(KeyCode.P))
		{
			Utils.ChangeScene("Overworld");
		}

		if (Input.GetKeyDown(KeyCode.M))
		{
			Utils.ChangeScene("MainMenu");
		}
	}
}
