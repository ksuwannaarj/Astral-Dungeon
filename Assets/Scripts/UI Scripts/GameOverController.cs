using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour {

    void Start() {
        GameObject.Find("Title Text").GetComponent<TMPro.TextMeshProUGUI>().text = "Game Over\nTotal Score: " + Scoring.score;
    }

    public void ReplayGame() {
        SceneManager.LoadScene("TestScene");
        Scoring.score = 0;
    }

    public void Return() {
         SceneManager.LoadScene("MainMenu");
    }
}
