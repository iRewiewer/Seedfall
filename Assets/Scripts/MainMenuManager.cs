using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
	public TMP_InputField seedInput;
	public Toggle randomizeSeedToggle;

	public void Start()
	{
		seedInput.text = GameManager.Instance.seed.ToString();

		// set listeners
		seedInput.onValueChanged.AddListener(delegate { seedInputTextChanged(); });
		randomizeSeedToggle.onValueChanged.AddListener(delegate { randomizeSeedToggleChanged(); });
	}

	public void playBtnOnClick()
	{
		Debug.Log($"Selected seed: {GameManager.Instance.seed}");
		Utils.ChangeScene("Overworld");
	}

	public void quitBtnOnClick()
	{
		Utils.QuitGame();
	}

	public void seedInputTextChanged()
	{
		GameManager.Instance.seed = int.Parse(seedInput.text);
	}

	public void randomizeSeedToggleChanged()
	{
		if (randomizeSeedToggle.isOn)
		{
			GameManager.Instance.seed = Random.Range(int.MinValue, int.MaxValue);
			seedInput.text = GameManager.Instance.seed.ToString();
		}
	}
}
