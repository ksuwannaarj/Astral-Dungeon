using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {

    void Start()
    {
        Scoring.score = 0;
    }
    
    public void PlayGame()
    {
        SceneManager.LoadScene("TestScene");
    }

    public void Instructions()
    {
        SceneManager.LoadScene("Instructions");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
