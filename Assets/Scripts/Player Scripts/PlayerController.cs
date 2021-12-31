using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody rb;
    Vector3 movement;

    Vector3 lastPos;
    float lastTime;

    public bool playerMoved = false;
    public Animator animator;
    public int isDead = 0;
    
    // Update is called once per frame
    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    private IEnumerator WaitForSceneLoad() {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("GameOver");
    }

    void Update()
    {
        if(isDead != 1){
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.z = Input.GetAxisRaw("Vertical");
            movement.y = 0;
            if(Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0){
                playerMoved = true;
                animator.SetInteger("isWalking", 1);
            }else{
                playerMoved = false;
                animator.SetInteger("isWalking", 0);
            }
        }
        else
        {
            StartCoroutine(WaitForSceneLoad());
        }
        // Debug.Log(playerMoved);
        rb.MovePosition(rb.position + movement * moveSpeed * Time.deltaTime);
    }

    // private void FixedUpdate()
    // {
    //     // Debug.Log(timeManager.Ts);
    // }

}