using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyController : MonoBehaviour
{
    public int difficulty;
    public bool islevelCompleted = false;

    public void AddDifficulty(){
        difficulty += 2;
    }
}

