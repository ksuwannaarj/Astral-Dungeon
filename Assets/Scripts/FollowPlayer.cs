using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private GameObject player;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;
    private Vector3 velocity = Vector3.zero;

    // Update is called once per frame

    private void Start() {
        player =  GameObject.FindWithTag("Player");
    }
    private void FixedUpdate()
    {
        if(player == null){
            player =  GameObject.FindWithTag("Player");
        }else
        {
            
            if(!Input.GetKey(KeyCode.LeftShift)){
            // Debug.Log("not pressing f");
            Vector3 desiredPosition = player.transform.position + offset;
            Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition,ref velocity, smoothSpeed);
            transform.position = smoothedPosition;
            }
        }
    }

}
