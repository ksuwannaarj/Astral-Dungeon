using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyWhenOffScreen : MonoBehaviour
{
    // Update is called once per frame
	void OnBecameInvisible () {
        Destroy(this.gameObject);
	}
}
