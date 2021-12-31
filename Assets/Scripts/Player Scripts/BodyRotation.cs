using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyRotation : MonoBehaviour
{
    // Update is called once per frame
    Vector3 mousePosition;
    Quaternion lastMousePositionRaw;
    public bool playerRotated = false;
    private void Start() {
        lastMousePositionRaw = transform.rotation;
    }
    private void FixedUpdate() {
        if(transform.rotation != lastMousePositionRaw){
            playerRotated = true;
            lastMousePositionRaw = transform.rotation;
        }else{
            playerRotated = false;
        }
    }
    void Update()
    {
        if (Time.timeScale == 1)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit))
                mousePosition = raycastHit.point;
            mousePosition.y = -1;
            Vector3 relativePos = mousePosition - transform.position;
            float rotation = Mathf.Atan2(relativePos.x, relativePos.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, rotation + 90, 0f);
        }
    }
}