using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public GameObject createOnDestroy;
    public int startingHealth = 10;
    private int currentHealth;
    private Scoring _scoring;

    public bool isPlayerInvincible = false;

	// Use this for initialization
	void Start () {
        this.ResetHealthToStarting();
        _scoring = GameObject.Find("Score").GetComponent<Scoring>();
    }

    // Reset health to original starting health
    public void ResetHealthToStarting()
    {
        currentHealth = startingHealth;
    }

    public void IncreaseHealth()
    {
        if(currentHealth + 2 >= startingHealth){
            currentHealth = startingHealth;
        } else {
            currentHealth += 2;
        }
    }

    public void IncreaseScore()
    {
        _scoring.addScore(500);
    }

    // Reduce the health of the object by a certain amount
    // If health lte zero, destroy the object
    public void ApplyDamage(int damage)
    {
        currentHealth -= damage;
        if(gameObject.CompareTag("Player") && !isPlayerInvincible){
            gameObject.GetComponentInChildren<Animator>().SetTrigger("getHit");
        }
        if (currentHealth <= 0)
        {
            if(gameObject.CompareTag("Player")){
                gameObject.GetComponentInChildren<Animator>().SetTrigger("dead");
                gameObject.GetComponent<PlayerController>().isDead = 1;
                Debug.Log(gameObject.GetComponent<PlayerController>().isDead);
                gameObject.GetComponent<BoxCollider>().enabled = false;
                gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                gameObject.GetComponentInChildren<BodyRotation>().enabled = false;
                gameObject.GetComponentInChildren<Weapon>().enabled = false;
                // gameObject.GetComponent<HealthManager>().enabled = false;
            }
            else if(gameObject.CompareTag("Enemy")|| gameObject.CompareTag("Mimic")){
                Destroy(this.gameObject);
                GameObject obj = Instantiate(this.createOnDestroy);
                obj.transform.position = this.transform.position;
                Destroy(obj.gameObject, 2);
                if (gameObject.CompareTag("Enemy")) {
                    _scoring.addScore(100);
                }
                else{
                    _scoring.addScore(50);
                }
            }
        }
    }

    // Get the current health of the object
    public int GetHealth()
    {
        return this.currentHealth;
    }

    public int getStartingHealth()
    {
        return this.startingHealth;
    }
}
