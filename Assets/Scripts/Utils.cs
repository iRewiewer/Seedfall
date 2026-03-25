using UnityEngine;
using UnityEngine.SceneManagement;

public class Utils
{
	public static void ChangeScene(string sceneName)
	{
		SceneManager.LoadScene(sceneName);
	}

	public static void QuitGame()
	{
		Application.Quit();
	}
}