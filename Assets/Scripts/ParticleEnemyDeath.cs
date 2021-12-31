using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEnemyDeath : MonoBehaviour
{
    public ParticleSystem targetParticleSystem;

    // Update is called once per frame
    void Update()
    {
        if (!this.targetParticleSystem.IsAlive())
        {
            Destroy(targetParticleSystem.gameObject);
        }
    }
    void OnParticleCollision(GameObject other) {
        Debug.Log("Particle was hit!");
    }
}
