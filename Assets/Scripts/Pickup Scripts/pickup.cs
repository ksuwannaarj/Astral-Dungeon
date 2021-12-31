using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class pickup : MonoBehaviour
{
    public enum pickupType {health, score, finish};
    public pickupType type;
    private LevelBuilder levelBuilder;
    private HealthManager player;
    private int playerHealth;
    private GameObject GM;
    
    void Start() {
        GM = GameObject.Find("GM");
        player = GameObject.FindWithTag("Player").GetComponent<HealthManager>();
        playerHealth = player.GetHealth();
        levelBuilder = GameObject.Find("LevelGenerator").GetComponent<LevelBuilder>();
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider player) // to see when the player enters the collider
    {
        if(player.gameObject.tag == "Player") //on the object you want to pick up set the tag to be anything, in this case "object"
        {
            if(this.type == pickupType.health){
                player.gameObject.GetComponent<HealthManager>().IncreaseHealth();
                Destroy(this.gameObject);
            }
            if(this.type == pickupType.score){
                player.gameObject.GetComponent<HealthManager>().IncreaseScore();
                Destroy(this.gameObject);
            }
            if(this.type == pickupType.finish){
                //finish level
                // Add score according to health remain
                Scoring.score += playerHealth * 100;
                levelBuilder.CompleteLevel();
                DontDestroyOnLoad(GM);
                SceneManager.LoadScene("NextLevel");
                Destroy(this.gameObject);
            }
        }
    }
}
