using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMouse : MonoBehaviour
{
    // Start is called before the first frame update
    // Update is called once per frame
    void Update()
    {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
