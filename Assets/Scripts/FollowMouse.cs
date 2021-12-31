using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    Vector3 mousePosition;
    public float smoothSpeed = 0.125f;
    private Vector3 velocity = Vector3.zero;
    Vector3 position = new Vector3(0,0,0);
    float range = 10;
    GameObject player; 
    // Start is called before the first frame update
    // Update is called once per frame
    private void Start() {
        player = GameObject.FindWithTag("Player");
    }
    void FixedUpdate()
    {
        if(player == null){
            player = GameObject.FindWithTag("Player");
        }
        if(Input.GetKey(KeyCode.LeftShift)){
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out RaycastHit raycastHit))
            mousePosition = raycastHit.point;
            mousePosition.y = Camera.main.transform.position.y;
            position = Vector3.SmoothDamp(transform.position, mousePosition, ref velocity, smoothSpeed);
            position.x = Mathf.Clamp(position.x, player.transform.position.x - range, player.transform.position.x + range);
            position.z = Mathf.Clamp(position.z, player.transform.position.z - range, player.transform.position.z + range);
            transform.position = position;
        }
    }
}