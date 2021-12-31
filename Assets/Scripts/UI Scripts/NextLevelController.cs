using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class NextLevelController : MonoBehaviour {
    private GameObject GM;
    void Start() {
        GM = GameObject.Find("GM");
        GameObject.Find("Title Text").GetComponent<TMPro.TextMeshProUGUI>().text = "Victory!\nScore: " + Scoring.score;
    }

    public void NextLevel() {
        DontDestroyOnLoad(GM);
        SceneManager.LoadScene("TestScene");
    }
    public void Return() {
        SceneManager.LoadScene("MainMenu");
    }
}
