using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour {

	public void quitGame() {
		Time.timeScale = 1;
		SceneManager.LoadScene("MainMenu");
	}
}
