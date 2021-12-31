using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scoring : MonoBehaviour {
    static public int score = 0;

    [SerializeField]
    private Text _scoreText;

    // Calculate the new score when enemies are killed
    public void addScore(int addPoints) {
        score += addPoints;
        updateScore(score);
    }

    // Update the score counter in Canvas
    public void updateScore(int newScore) {
        _scoreText.text = newScore.ToString();
    }
}
