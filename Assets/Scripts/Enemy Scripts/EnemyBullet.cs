using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{   
    public int damage;

    public GameObject createOnDestroy;

    public float speed = 20f;
    private Vector3 dir;
    private Transform player;
    Vector3 finalDir;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        dir = player.position - transform.position;
    }
    private void Update() {
        transform.position += (dir.normalized * speed * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Player")){
            GameObject player = other.gameObject;
            player.GetComponent<HealthManager>().ApplyDamage(damage);
            Destroy(gameObject);
            GameObject obj = Instantiate(this.createOnDestroy);
            obj.transform.position = this.transform.position;
            Destroy(obj.gameObject, 2);
        }else if(other.gameObject.CompareTag("Obstacles")){
            Destroy(gameObject);
            GameObject obj = Instantiate(this.createOnDestroy);
            obj.transform.position = this.transform.position;
            Destroy(obj.gameObject, 2);
        }
    }

}