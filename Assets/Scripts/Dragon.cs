using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon : MonoBehaviour {

    public ParticleSystem particle;
  
    void Start () {
        particle.Stop();
    }

    void Update () {

    }

    private void OnTriggerEnter (Collider other) {
        Debug.Log ("enter");
    }

    private void OnTriggerStay (Collider other) {

        if (Input.GetMouseButton (0)) {
    

            if(!particle.isPlaying)
            {
                particle.Play();
            }
                
        }

        if (particle.isPlaying)
        {
            particle.Stop();
        }

    }
}