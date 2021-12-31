using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{   
    public int damage;
    public GameObject createOnDestroy;
    public int speed = 15;

    void FixedUpdate()
    {
        transform.Translate(Vector3.right * -speed * Time.fixedDeltaTime);
    }
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Mimic")){
            // Debug.Log("hit");
            Enemy enemy = other.GetComponent<Enemy>();
            enemy.GetComponent<HealthManager>().ApplyDamage(damage);
            Destroy(gameObject);
            GameObject obj = Instantiate(this.createOnDestroy);
            obj.transform.position = this.transform.position;
            Destroy(obj.gameObject, 2);

        }else if(other.gameObject.CompareTag("Obstacles")){
            // Debug.Log("hit wall");
            Destroy(gameObject);
            GameObject obj = Instantiate(this.createOnDestroy);
            obj.transform.position = this.transform.position;
            Destroy(obj.gameObject, 2);
        }
    }
}