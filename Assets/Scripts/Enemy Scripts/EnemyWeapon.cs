using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    public float fireRateMin = 0.7f;
    public float fireRateMax = 2;
    Vector3 dirToTarget;
    float distanceToPlayer;
    float fireRate;
    float timeToFire = 0;
    Transform firePoint;
    public GameObject bulletOrange;
    public GameObject bulletRed;
    public GameObject bulletYellow;
    public GameObject bulletGreen;
    public float speed = 20f;
    public Animator animator;
    // Start is called before the first frame update
    private int bulletColor;
    Transform player;
    public LayerMask toHit;
    private void Awake() {
        fireRate = Random.Range(fireRateMin, fireRateMax);
        firePoint = transform;
        if(firePoint == null){
            Debug.LogError("no firepoint");
        }
        bulletColor = Random.Range(0,4);
    }

    // Update is called once per frame
    void Update()
    {
        if(player == null){
            player = gameObject.GetComponent<EnemyAI>().target.transform;
        }
        if(gameObject.GetComponent<EnemyAI>().playerDetected){
            if(fireRate == 0){
                    Shoot();
            }
            else{
                if(Time.time > timeToFire){
                    timeToFire = Time.time + 1/fireRate;
                    dirToTarget = player.transform.position - transform.position;
                    distanceToPlayer = dirToTarget.magnitude;
                    Debug.DrawRay(transform.position, dirToTarget, Color.green);
                    if(!Physics.Raycast(transform.position, dirToTarget,distanceToPlayer, toHit)){
                        Shoot();
                    }
                }
            }  
        }
    }
    void Shoot(){
        // Quaternion.Euler(new Vector3(0,90,0))
        animator.SetInteger("attack", 1);
        GameObject bulletClone;
        switch (bulletColor)
        {
        case 0:
            bulletClone = Instantiate(bulletOrange, firePoint.position, firePoint.rotation);
            break;
        case 1:
            bulletClone = Instantiate(bulletRed, firePoint.position, firePoint.rotation);
            break;
        case 2:
            bulletClone = Instantiate(bulletYellow, firePoint.position, firePoint.rotation);
            break;
        case 3:
            bulletClone = Instantiate(bulletGreen, firePoint.position, firePoint.rotation);
            break;
        default:
            bulletClone = Instantiate(bulletOrange, firePoint.position, firePoint.rotation);
            break;
        }
        // bulletRb.velocity = transform.forward*1000;
    }
}