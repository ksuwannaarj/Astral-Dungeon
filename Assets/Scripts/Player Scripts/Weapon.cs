using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float fireRate = 0;
    float timeToFire = 0;
    Transform firePoint;
    public GameObject bullet;
    public Animator animator;
    // Start is called before the first frame update
    private void Awake() {
        firePoint = transform.Find("FirePoint");
        if(firePoint == null){
            Debug.LogError("no firepoint");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(fireRate == 0){
            if(Input.GetButtonDown("Fire1")){
                Shoot();
            }
        }
        else{
            if(Input.GetButtonDown("Fire1") && Time.time > timeToFire){
                timeToFire = Time.time + 1/fireRate;
                Shoot();
            }
        }  

    }
    void Shoot(){
        animator.SetInteger("attack", 1);
        // Quaternion.Euler(new Vector3(0,90,0))
        GameObject bulletClone = Instantiate(bullet, firePoint.position, firePoint.rotation);
        // bulletRb.velocity = transform.forward*1000;
    }
}