using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;
	public int seed = 1337;

	private void Awake()
	{
#if DEBUG
		seed = Random.Range(int.MinValue, int.MaxValue);
		Debug.Log($"Selected seed: {seed}");
#endif

		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(Instance);
			return;
		}

		Destroy(gameObject);
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void AutoCreate()
	{
		if(Instance == null)
		{
			GameObject go = new GameObject("GameManager");
			go.AddComponent<GameManager>();
		}
	}
}